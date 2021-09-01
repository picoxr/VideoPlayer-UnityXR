#if UNITY_EDITOR_WIN && UNITY_ANDROID
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

public class PXR_BuildToolManager
{
    // Scene Quick Preview
    private const string SQP_APK_VERSION = "PxrSQPAPKVersion";
    private const string SQP_INDEX_NAME = "PxrSQPIndex.unity";
    private const string SQP_BUNDLE_PATH = "PxrSQPBundles";
    private const string SQP_SCENE_BUNDLE = "PxrSceneBundle";
    private const string SQP_APK_NAME = "PxrSQP.apk";
    private const string EXTERNAL_STORAGE_PATH = "/sdcard/Android/data";
    public const string SCENE_LOAD_DATA_NAME = "SceneLoadData.txt";
    private const int BUNDLE_CHUNK_SIZE = 30;

    public static List<string> buildSceneNameList = new List<string>();
    public static List<SceneInfo> buildSceneInfoList = new List<SceneInfo>();

    private static AndroidArchitecture targetArchitecture;
    private static ScriptingImplementation scriptBackend;
    private static ManagedStrippingLevel managedStrippingLevel;
    private static string remoteSceneCache;
    private static string previewIndexPath;
    private static string appVersion;
    private static bool stripEngineCode;

    public class SceneInfo
    {
        public string scenePath;
        public string sceneName;

        public SceneInfo(string path, string name)
        {
            scenePath = path;
            sceneName = name;
        }
    }

    public static void BuildScenes(bool forceRestart)
    {
        if (!PXR_ADBTool.GetInstance().IsReady())
        {
            return;
        }

        if (!IsInstalledAPP())
        {
            InstallAPP();
        }

        GetScenesEnabled();

        remoteSceneCache = EXTERNAL_STORAGE_PATH + "/" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)
           + "/cache/scenes";

        Dictionary<string, string> assetInSceneBundle = new Dictionary<string, string>();
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        Dictionary<string, List<string>> extToAssetList = new Dictionary<string, List<string>>();

        string[] resDirectories = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories).ToArray();

        if (resDirectories.Length > 0)
        {
            string[] resAssetPaths = AssetDatabase.FindAssets("", resDirectories).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            ProcessAssets(resAssetPaths, "resources", ref assetInSceneBundle, ref extToAssetList);

            AssetBundleBuild resBundle = new AssetBundleBuild();
            resBundle.assetNames = assetInSceneBundle.Keys.ToArray();
            resBundle.assetBundleName = PXR_SQPLoader.RESOURCE_BUNDLE_NAME;
            assetBundleBuilds.Add(resBundle);
        }

        foreach (var scene in buildSceneInfoList)
        {
            string[] assetDependencies = AssetDatabase.GetDependencies(scene.scenePath);
            ProcessAssets(assetDependencies, scene.sceneName, ref assetInSceneBundle, ref extToAssetList);

            string[] sceneAsset = new string[1] { scene.scenePath };
            AssetBundleBuild sceneBuild = new AssetBundleBuild();
            sceneBuild.assetBundleName = "scene_" + scene.sceneName;
            sceneBuild.assetNames = sceneAsset;
            assetBundleBuilds.Add(sceneBuild);
        }

        foreach (string ext in extToAssetList.Keys)
        {
            int assetCount = extToAssetList[ext].Count;
            int numChunks = (assetCount + BUNDLE_CHUNK_SIZE - 1) / BUNDLE_CHUNK_SIZE;
            for (int i = 0; i < numChunks; i++)
            {
                List<string> assetChunkList;
                if (i == numChunks - 1)
                {
                    int size = BUNDLE_CHUNK_SIZE - (numChunks * BUNDLE_CHUNK_SIZE - assetCount);
                    assetChunkList = extToAssetList[ext].GetRange(i * BUNDLE_CHUNK_SIZE, size);
                }
                else
                {
                    assetChunkList = extToAssetList[ext].GetRange(i * BUNDLE_CHUNK_SIZE, BUNDLE_CHUNK_SIZE);
                }
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = "asset_" + ext + i;
                build.assetNames = assetChunkList.ToArray();
                assetBundleBuilds.Add(build);
            }
        }

        // Build asset bundles
        BuildPipeline.BuildAssetBundles(PXR_DirectorySyncer.CreateDirectory(SQP_BUNDLE_PATH, SQP_SCENE_BUNDLE), assetBundleBuilds.ToArray(),
                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);

        string tempDirectory = PXR_DirectorySyncer.CreateDirectory(SQP_BUNDLE_PATH, "Temp");

        string absoluteTempPath = Path.Combine(Path.Combine(Application.dataPath, ".."), tempDirectory);

        if (!PullSceneBundles(absoluteTempPath, remoteSceneCache))
        {
            return;
        }

        // Create file to tell SQP index scene APP which scene to load and push it to the device
        string sceneLoadDataPath = Path.Combine(tempDirectory, SCENE_LOAD_DATA_NAME);
        if (File.Exists(sceneLoadDataPath))
        {
            File.Delete(sceneLoadDataPath);
        }

        StreamWriter writer = new StreamWriter(sceneLoadDataPath, true);
        // Write version and scene names
        long unixTime = (int)(DateTimeOffset.UtcNow.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        writer.WriteLine(unixTime.ToString());
        for (int i = 0; i < buildSceneInfoList.Count; i++)
        {
            writer.WriteLine(Path.GetFileNameWithoutExtension(buildSceneInfoList[i].scenePath));
        }

        writer.Close();

        string absoluteSceneLoadDataPath = Path.Combine(absoluteTempPath, SCENE_LOAD_DATA_NAME);
        string[] pushCommand = { "-d push", "\"" + absoluteSceneLoadDataPath + "\"", "\"" + remoteSceneCache + "\"" };
        string output, error;
        if (PXR_ADBTool.GetInstance().RunCommand(pushCommand, null, out output, out error) != 0)
        {
            PXR_SceneQucikPreviewEW.PrintError(string.IsNullOrEmpty(error) ? output : error);
            return;
        }

        if (forceRestart)
        {
            RestartApp();
            return;
        }

        PXR_SceneQucikPreviewEW.PrintSuccess("Build Scenes.");
    }

    public static bool IsInstalledAPP()
    {
        if (!PXR_ADBTool.GetInstance().IsReady())
        {
            return false;
        }

        string matchedPackageList, error;
        var packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

        string[] packageCheckCommand = new string[] { "-d shell pm list package", packageName };
        if (PXR_ADBTool.GetInstance().RunCommand(packageCheckCommand, null, out matchedPackageList, out error) == 0)
        {
            if (string.IsNullOrEmpty(matchedPackageList))
            {
                return false;
            }

            if (!matchedPackageList.Contains("package:" + packageName + "\r\n"))
            {
                return false;
            }

            // get package info to check for TRANSITION_APK_VERSION_NAME
            string[] dumpPackageInfoCommand = new string[] { "-d shell dumpsys package", packageName };
            string packageInfo;
            if (PXR_ADBTool.GetInstance().RunCommand(dumpPackageInfoCommand, null, out packageInfo, out error) == 0 &&
                    !string.IsNullOrEmpty(packageInfo) &&
                    packageInfo.Contains(SQP_APK_VERSION))
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public static bool RestartApp()
    {
        if (!PXR_ADBTool.GetInstance().IsReady())
        {
            return false;
        }

        string output, error;
        string[] appStartCommand = { "-d shell", "am start -a android.intent.action.MAIN -c android.intent.category.LAUNCHER -S -f 0x10200000 -n", PXR_PathHelper.GetPlayerActivityName() };
        if (PXR_ADBTool.GetInstance().RunCommand(appStartCommand, null, out output, out error) == 0)
        {
            PXR_SceneQucikPreviewEW.PrintSuccess("App " + " Restart Success!");
            return true;
        }

        string completeError = "Failed to restart App. Try restarting it manually through the device.\n" + (string.IsNullOrEmpty(error) ? output : error);
        Debug.LogError(completeError);
        return false;
    }

    public static bool UninstallAPP()
    {
        PXR_SceneQucikPreviewEW.PrintLog("Uninstalling Application . . .");

        if (!PXR_ADBTool.GetInstance().IsReady())
        {
            return false;
        }

        string output, error;
        string appPackagename = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        string[] appStartCommand = { "-d shell", "pm uninstall", appPackagename };
        if (PXR_ADBTool.GetInstance().RunCommand(appStartCommand, null, out output, out error) == 0)
        {
            PXR_SceneQucikPreviewEW.PrintSuccess("App package " + appPackagename + " is uninstalled.");
            return true;
        }

        PXR_SceneQucikPreviewEW.PrintError("Failed to uninstall APK.");
        return false;
    }

    public static void DeleteCacheBundles()
    {
        try
        {
            if (Directory.Exists(SQP_BUNDLE_PATH))
            {
                Directory.Delete(SQP_BUNDLE_PATH, true);
            }
            DeleteCachePreviewIndexFile();
        }
        catch (Exception e)
        {
            PXR_SceneQucikPreviewEW.PrintError(e.Message);
        }
        PXR_SceneQucikPreviewEW.PrintSuccess("Deleted Cache Bundles.");
    }

    public static void DeleteCachePreviewIndexFile()
    {
        try
        {
            if (File.Exists(previewIndexPath))
            {
                File.Delete(previewIndexPath);
                previewIndexPath = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static void GetScenesEnabled()
    {
        buildSceneNameList.Clear();
        buildSceneInfoList.Clear();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                if (Path.GetFileName(scene.path) != SQP_INDEX_NAME)
                {
                    buildSceneNameList.Add(scene.path);

                    SceneInfo sceneInfo = new SceneInfo(scene.path, Path.GetFileNameWithoutExtension(scene.path));
                    buildSceneInfoList.Add(sceneInfo);
                }
            }
        }

        if (buildSceneInfoList.Count == 0)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneInfo sceneInfo = new SceneInfo(scene.path, Path.GetFileNameWithoutExtension(scene.path));
            buildSceneInfoList.Add(sceneInfo);
        }
    }

    private static void InstallAPP()
    {
        PXR_SceneQucikPreviewEW.PrintLog("installing APP  . . .");

        PXR_DirectorySyncer.CreateDirectory(SQP_BUNDLE_PATH);

        PrebuildProjectSettingUpdate();

        if (string.IsNullOrEmpty(previewIndexPath) || !File.Exists(previewIndexPath))
        {
            string[] editorScenePaths = Directory.GetFiles(PXR_PathHelper.GetPicoXRPluginPath(), SQP_INDEX_NAME, SearchOption.AllDirectories);

            if (editorScenePaths.Length == 0 || editorScenePaths.Length > 1)
            {
                PXR_SceneQucikPreviewEW.PrintError(editorScenePaths.Length + " " + SQP_INDEX_NAME + " has been found, please double check your PicoXR Plugin import.");
                return;
            }
            previewIndexPath = Path.Combine(Application.dataPath, SQP_INDEX_NAME);
            if (File.Exists(previewIndexPath))
            {
                File.Delete(previewIndexPath);
            }
            File.Copy(editorScenePaths[0], previewIndexPath);
        }

        string[] buildScenes = new string[1] { previewIndexPath };
        string apkOutputPath = Path.Combine(SQP_BUNDLE_PATH, SQP_APK_NAME);

        if (File.Exists(apkOutputPath))
        {
            File.Delete(apkOutputPath);
        }

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = buildScenes,
            locationPathName = apkOutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.Development |
                BuildOptions.AutoRunPlayer
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            PXR_SceneQucikPreviewEW.PrintSuccess("App is installed.");
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            PXR_SceneQucikPreviewEW.PrintError();
        }
        PostbuildProjectSettingUpdate();
    }

    private static void ProcessAssets(string[] assetPaths,
        string assetParent,
        ref Dictionary<string, string> sceneBundle,
        ref Dictionary<string, List<string>> assetList)
    {
        foreach (string asset in assetPaths)
        {
            string ext = Path.GetExtension(asset);
            if (string.IsNullOrEmpty(ext))
            {
                continue;
            }

            ext = ext.Substring(1);
            if (ext.Equals("cs") || ext.Equals("unity"))
            {
                continue;
            }

            if (sceneBundle.ContainsKey(asset))
            {
                continue;
            }

            var assetObject = AssetDatabase.LoadAssetAtPath(asset, typeof(UnityEngine.Object));
            if (assetObject == null || (assetObject.hideFlags & HideFlags.DontSaveInBuild) == 0)
            {
                sceneBundle[asset] = assetParent;

                if (assetParent != "resources")
                {
                    if (!assetList.ContainsKey(ext))
                    {
                        assetList[ext] = new List<string>();
                    }
                    assetList[ext].Add(asset);
                }
            }
        }
    }

    private static bool PullSceneBundles(string absoluteTempPath, string externalSceneCache)
    {
        List<string> bundlesToTransfer = new List<string>();
        string manifestFilePath = externalSceneCache + "/" + SQP_SCENE_BUNDLE;

        string[] pullManifestCommand = { "-d pull", "\"" + manifestFilePath + "\"", "\"" + absoluteTempPath + "\"" };

        string output, error;
        if (PXR_ADBTool.GetInstance().RunCommand(pullManifestCommand, null, out output, out error) == 0)
        {
            // Load hashes from remote manifest
            AssetBundle remoteBundle = AssetBundle.LoadFromFile(Path.Combine(absoluteTempPath, SQP_SCENE_BUNDLE));
            if (remoteBundle == null)
            {
                PXR_SceneQucikPreviewEW.PrintError("Failed to load remote asset bundle manifest file.");
                return false;
            }
            AssetBundleManifest remoteManifest = remoteBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            Dictionary<string, Hash128> remoteBundleToHash = new Dictionary<string, Hash128>();
            if (remoteManifest != null)
            {
                string[] assetBundles = remoteManifest.GetAllAssetBundles();
                foreach (string bundleName in assetBundles)
                {
                    remoteBundleToHash[bundleName] = remoteManifest.GetAssetBundleHash(bundleName);
                }
            }
            remoteBundle.Unload(true);

            AssetBundle localBundle = AssetBundle.LoadFromFile(SQP_BUNDLE_PATH + "\\" + SQP_SCENE_BUNDLE
                    + "\\" + SQP_SCENE_BUNDLE);
            if (localBundle == null)
            {
                PXR_SceneQucikPreviewEW.PrintError("Failed to load local asset bundle manifest file.");
                return false;
            }
            AssetBundleManifest localManifest = localBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            if (localManifest != null)
            {
                Hash128 zeroHash = new Hash128(0, 0, 0, 0);

                // Build a list of dirty bundles that will have to be transfered
                string relativeSceneBundlesPath = Path.Combine(SQP_BUNDLE_PATH, SQP_SCENE_BUNDLE);
                bundlesToTransfer.Add(Path.Combine(relativeSceneBundlesPath, SQP_SCENE_BUNDLE));
                string[] assetBundles = localManifest.GetAllAssetBundles();
                foreach (string bundleName in assetBundles)
                {
                    if (!remoteBundleToHash.ContainsKey(bundleName))
                    {
                        bundlesToTransfer.Add(Path.Combine(relativeSceneBundlesPath, bundleName));
                    }
                    else
                    {
                        if (remoteBundleToHash[bundleName] != localManifest.GetAssetBundleHash(bundleName))
                        {
                            bundlesToTransfer.Add(Path.Combine(relativeSceneBundlesPath, bundleName));
                        }
                        remoteBundleToHash[bundleName] = zeroHash;
                    }
                }

                PXR_SceneQucikPreviewEW.PrintLog(bundlesToTransfer.Count + " dirty bundle(s) will be transfered.\n");
            }
        }
        else
        {
            if (output.Contains("does not exist") || output.Contains("No such file or directory"))
            {
                PXR_SceneQucikPreviewEW.PrintLog("Manifest file not found. Transfering all bundles . . . ");

                string[] mkdirCommand = { "-d shell", "mkdir -p", "\"" + externalSceneCache + "\"" };
                if (PXR_ADBTool.GetInstance().RunCommand(mkdirCommand, null, out output, out error) == 0)
                {
                    string absoluteSceneBundlePath = Path.Combine(Path.Combine(Application.dataPath, ".."),
                            Path.Combine(SQP_BUNDLE_PATH, SQP_SCENE_BUNDLE));

                    string[] assetBundlePaths = Directory.GetFiles(absoluteSceneBundlePath);
                    if (assetBundlePaths.Length == 0)
                    {
                        PXR_SceneQucikPreviewEW.PrintError("Failed to locate scene bundles to transfer.");
                        return false;
                    }
                    foreach (string path in assetBundlePaths)
                    {
                        if (!path.Contains(".manifest"))
                        {
                            bundlesToTransfer.Add(path);
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(error) || output.Contains("error"))
        {
            PXR_SceneQucikPreviewEW.PrintError(string.IsNullOrEmpty(error) ? output : error);
            return false;
        }

        foreach (string bundle in bundlesToTransfer)
        {
            string absoluteBundlePath = Path.Combine(Path.Combine(Application.dataPath, ".."), bundle);
            string[] pushBundleCommand = { "-d push", "\"" + absoluteBundlePath + "\"", "\"" + externalSceneCache + "\"" };
            PXR_ADBTool.GetInstance().RunCommandAsync(pushBundleCommand, null);
        }

        return true;
    }

    private static void PrebuildProjectSettingUpdate()
    {
        // Save existing settings as some modifications can change other settings
        appVersion = PlayerSettings.bundleVersion;
        targetArchitecture = PlayerSettings.Android.targetArchitectures;
        scriptBackend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
        managedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android);
        stripEngineCode = PlayerSettings.stripEngineCode;

        PlayerSettings.bundleVersion = SQP_APK_VERSION;

        if (targetArchitecture != AndroidArchitecture.ARMv7)
        {
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
        }

        if (scriptBackend != ScriptingImplementation.Mono2x)
        {
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.Mono2x);
        }

        if (managedStrippingLevel != ManagedStrippingLevel.Disabled)
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
        }

        if (stripEngineCode)
        {
            PlayerSettings.stripEngineCode = false;
        }
    }

    private static void PostbuildProjectSettingUpdate()
    {
        // Restore project setting
        PlayerSettings.bundleVersion = appVersion;
        if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) != scriptBackend)
        {
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, scriptBackend);
        }

        if (PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android) != managedStrippingLevel)
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, managedStrippingLevel);
        }

        if (PlayerSettings.stripEngineCode != stripEngineCode)
        {
            PlayerSettings.stripEngineCode = stripEngineCode;
        }

        if (PlayerSettings.Android.targetArchitectures != targetArchitecture)
        {
            PlayerSettings.Android.targetArchitectures = targetArchitecture;
        }
    }
}
#endif