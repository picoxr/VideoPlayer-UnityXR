/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class PXR_BuildAndRunEW : EditorWindow
{
#if UNITY_EDITOR_WIN && UNITY_ANDROID

    static int totalBuildSteps;
    static int currentStep;
    static string progressMessage;

    static string applicationIdentifier;
    static string productName;

    static string gradleTempExport;
    static string gradleExport;
    static bool showCancel;
    static bool buildFailed;

    static double totalBuildTime;

    static PXR_DirectorySyncer.CancellationTokenSource syncCancelToken;
    static Process gradleBuildProcess;
    static Thread buildThread;

    static bool? apkOutputSuccessful;

    const string REMOTE_APK_PATH = "/data/local/tmp";
    const float USB_TRANSFER_SPEED_THRES = 25.0f;
    const float USB_3_TRANSFER_SPEED = 32.0f;
    const float TRANSFER_SPEED_CHECK_THRESHOLD = 4.0f;
    const int NUM_BUILD_AND_RUN_STEPS = 9;
    const int BYTES_TO_MEGABYTES = 1048576;

    private void OnGUI()
    {
        this.titleContent.text = "Build And Run";
        minSize = new Vector2(500, 170);
        maxSize = new Vector2(500, 170);

        Rect progressRect = EditorGUILayout.BeginVertical();
        progressRect.height = 25.0f;
        float progress = currentStep / (float)totalBuildSteps;
        EditorGUI.ProgressBar(progressRect, progress, progressMessage);
        if (showCancel)
        {
            GUIContent btnTxt = new GUIContent("Cancel");
            var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.button, GUILayout.ExpandWidth(false));
            rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, progressRect.height * 2);
            if (GUI.Button(rt, btnTxt, GUI.skin.button))
            {
                CancelBuild();
            }
        }
        EditorGUILayout.EndVertical();

        if (progress >= 1.0f || buildFailed)
        {
            Close();
        }
    }

    private void Update()
    {
        var window = focusedWindow;
        if (window != null && window.ToString().Contains("PXR_BuildTools"))
        {
            Repaint();
        }
    }

    void CancelBuild()
    {
        SetProgressBarMessage("Canceling . . .");

        if (syncCancelToken != null)
        {
            syncCancelToken.Cancel();
        }

        if (apkOutputSuccessful.HasValue && apkOutputSuccessful.Value)
        {
            buildThread.Abort();
            buildFailed = true;
        }

        if (gradleBuildProcess != null && !gradleBuildProcess.HasExited)
        {
            var cancelThread = new Thread(delegate ()
            {
                CancelGradleBuild();
            });
            cancelThread.Start();
        }
    }

    void CancelGradleBuild()
    {
        Process cancelGradleProcess = new Process();
        string arguments = "-Xmx1024m -classpath \"" + PXR_ADBTool.GetInstance().GetGradlePath() +
                           "\" org.gradle.launcher.GradleMain --stop";
        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
            FileName = PXR_ADBTool.GetInstance().GetJDKPath(),
            Arguments = arguments,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };

        cancelGradleProcess.StartInfo = processInfo;
        cancelGradleProcess.EnableRaisingEvents = true;

        cancelGradleProcess.OutputDataReceived += new DataReceivedEventHandler(
            (s, e) =>
            {
                if (e != null && e.Data != null && e.Data.Length != 0)
                {
                    UnityEngine.Debug.LogFormat("Gradle: {0}", e.Data);
                }
            }
        );

        apkOutputSuccessful = false;

        cancelGradleProcess.Start();
        cancelGradleProcess.BeginOutputReadLine();
        cancelGradleProcess.WaitForExit();

        buildFailed = true;
    }

#if UNITY_2018_4_OR_NEWER
    [MenuItem("PXR_SDK/Build Tool/Build And Run")]
    static void BuildAndRun()
    {
        GetWindow(typeof(PXR_BuildAndRunEW));
        showCancel = false;
        buildFailed = false;
        totalBuildTime = 0;

        InitializeProgressBar(NUM_BUILD_AND_RUN_STEPS);
        IncrementProgressBar("Exporting Unity Project . . .");

        if (!PXR_ADBTool.GetInstance().CheckADBDevices(log => {}))
        {
            buildFailed = true;
            return;
        }

        apkOutputSuccessful = null;
        syncCancelToken = null;
        gradleBuildProcess = null;

        UnityEngine.Debug.Log("PXRBuild: Starting Unity build ...");

        SetupDirectories();

        // 1. Get scenes to build in Unity, and export gradle project
        var buildResult = UnityBuildPlayer();

        if (buildResult.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            totalBuildTime += buildResult.summary.totalTime.TotalSeconds;

            // Set static variables so build thread has updated data
            showCancel = true;
            applicationIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#if UNITY_2019_3_OR_NEWER
            productName = "launcher";
#else
            productName = Application.productName;
#endif

            BuildRun();
            return;
        }
        else if (buildResult.summary.result == UnityEditor.Build.Reporting.BuildResult.Cancelled)
        {
            UnityEngine.Debug.Log("Build canceled.");
        }
        else
        {
            UnityEngine.Debug.Log("Build failed.");
        }
        buildFailed = true;
    }

    private static UnityEditor.Build.Reporting.BuildReport UnityBuildPlayer()
    {
#if UNITY_2020_1_OR_NEWER || UNITY_2019_4_OR_NEWER
        bool previousExportAsGoogleAndroidProject = EditorUserBuildSettings.exportAsGoogleAndroidProject;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
#endif
        var sceneList = GetScenesToBuild();
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = sceneList.ToArray(),
            locationPathName = gradleTempExport,
            target = BuildTarget.Android,
            options = BuildOptions.Development
                      | BuildOptions.AllowDebugging
#if !UNITY_2020_1_OR_NEWER && !UNITY_2019_4_OR_NEWER
                      | BuildOptions.AcceptExternalModificationsToPlayer
#endif
        };

        var buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions);

        UnityEngine.Debug.Log(UnityBuildPlayerSummary(buildResult));

        return buildResult;
    }

    private static string UnityBuildPlayerSummary(UnityEditor.Build.Reporting.BuildReport report)
    {
        var sb = new System.Text.StringBuilder();

        sb.Append($"Unity Build Player: Build {report.summary.result} ({report.summary.totalSize} bytes) in {report.summary.totalTime.TotalSeconds:0.00}s");

        foreach (var step in report.steps)
        {
            sb.AppendLine();
            if (step.depth > 0)
            {
                sb.Append(new String('-', step.depth));
                sb.Append(' ');
            }
            sb.Append($"{step.name}: {step.duration:g}");
        }

        return sb.ToString();
    }
#endif

    private static void BuildRun()
    {
        // 2. Process gradle project
        IncrementProgressBar("Processing gradle project . . .");
        if (ProcessGradleProject())
        {
            // 3. Build gradle project
            IncrementProgressBar("Starting gradle build . . .");
            if (BuildGradleProject())
            {
                // 4. Deploy and run
                if (DeployAPK())
                {
                    return;
                }
            }
        }
        buildFailed = true;
    }

    private static bool BuildGradleProject()
    {
        gradleBuildProcess = new Process();
        string arguments = "-Xmx4096m -classpath \"" + PXR_ADBTool.GetInstance().GetGradlePath() +
            "\" org.gradle.launcher.GradleMain assembleDebug -x validateSigningDebug --profile";
#if UNITY_2019_3_OR_NEWER
        var gradleProjectPath = gradleExport;
#else
        var gradleProjectPath = Path.Combine(gradleExport, productName);
#endif

        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            WorkingDirectory = gradleProjectPath,
            WindowStyle = ProcessWindowStyle.Normal,
            FileName = PXR_ADBTool.GetInstance().GetJDKPath(),
            Arguments = arguments,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };

        gradleBuildProcess.StartInfo = processInfo;
        gradleBuildProcess.EnableRaisingEvents = true;

        DateTime gradleStartTime = System.DateTime.Now;
        DateTime gradleEndTime = System.DateTime.MinValue;

        gradleBuildProcess.Exited += new System.EventHandler(
            (s, e) =>
            {
                UnityEngine.Debug.Log("Gradle: Exited");
            }
        );

        gradleBuildProcess.OutputDataReceived += new DataReceivedEventHandler(
            (s, e) =>
            {
                if (e != null && e.Data != null &&
                    e.Data.Length != 0 &&
                    (e.Data.Contains("BUILD") || e.Data.StartsWith("See the profiling report at:")))
                {
                    UnityEngine.Debug.LogFormat("Gradle: {0}", e.Data);
                    if (e.Data.Contains("SUCCESSFUL"))
                    {
                        UnityEngine.Debug.LogFormat("APK Build Completed: {0}",
                            Path.Combine(Path.Combine(gradleProjectPath, "build\\outputs\\apk\\debug"), productName + "-debug.apk").Replace("/", "\\"));
                        if (!apkOutputSuccessful.HasValue)
                        {
                            apkOutputSuccessful = true;
                        }
                        gradleEndTime = System.DateTime.Now;
                    }
                    else if (e.Data.Contains("FAILED"))
                    {
                        apkOutputSuccessful = false;
                    }
                }
            }
        );

        gradleBuildProcess.ErrorDataReceived += new DataReceivedEventHandler(
            (s, e) =>
            {
                if (e != null && e.Data != null &&
                    e.Data.Length != 0)
                {
                    UnityEngine.Debug.LogErrorFormat("Gradle: {0}", e.Data);
                }
                apkOutputSuccessful = false;
            }
        );

        gradleBuildProcess.Start();
        gradleBuildProcess.BeginOutputReadLine();
        IncrementProgressBar("Building gradle project . . .");

        gradleBuildProcess.WaitForExit();

        // Add a timeout for if gradle unexpectedlly exits or errors out
        Stopwatch timeout = new Stopwatch();
        timeout.Start();
        while (apkOutputSuccessful == null)
        {
            if (timeout.ElapsedMilliseconds > 5000)
            {
                UnityEngine.Debug.LogError("Gradle has exited unexpectedly.");
                apkOutputSuccessful = false;
            }
            Thread.Sleep(100);
        }

        // Record time it takes to build gradle project only if we had a successful build
        double gradleTime = (gradleEndTime - gradleStartTime).TotalSeconds;
        if (gradleTime > 0)
        {
            totalBuildTime += gradleTime;
        }

        return apkOutputSuccessful.HasValue && apkOutputSuccessful.Value;
    }

    private static bool ProcessGradleProject()
    {
        DateTime syncStartTime = System.DateTime.Now;
        DateTime syncEndTime = System.DateTime.MinValue;

        try
        {
            var ps = System.Text.RegularExpressions.Regex.Escape("" + Path.DirectorySeparatorChar);
            // ignore files .gradle/** build/** foo/.gradle/** and bar/build/**   
            var ignorePattern = string.Format("^([^{0}]+{0})?(\\.gradle|build){0}", ps);

            var syncer = new PXR_DirectorySyncer(gradleTempExport,
                gradleExport, ignorePattern);

            syncCancelToken = new PXR_DirectorySyncer.CancellationTokenSource();
            var syncResult = syncer.Synchronize(syncCancelToken.Token);
            syncEndTime = System.DateTime.Now;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("PXRBuild: Processing gradle project failed with exception: " +
                                  e.Message);
            return false;
        }

        if (syncCancelToken.IsCancellationRequested)
        {
            return false;
        }

        // Record time it takes to sync gradle projects only if the sync was successful
        double syncTime = (syncEndTime - syncStartTime).TotalSeconds;
        if (syncTime > 0)
        {
            totalBuildTime += syncTime;
        }

        return true;
    }
    private static List<string> GetScenesToBuild()
    {
        var sceneList = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            // Enumerate scenes in project and check if scene is enabled to build
            if (scene.enabled)
            {
                sceneList.Add(scene.path);
            }
        }
        return sceneList;
    }

    public static bool DeployAPK()
    {
        // Create new instance of ADB Tool
        if (!PXR_ADBTool.GetInstance().IsReady())
        {
            UnityEngine.Debug.LogError("Could not find the ADB executable in the specified Android SDK directory.");
            return false;
        }
        string apkPathLocal;
        string gradleExportFolder = Path.Combine(Path.Combine(gradleExport, productName), "build\\outputs\\apk\\debug");

        // Check to see if gradle output directory exists
        gradleExportFolder = gradleExportFolder.Replace("/", "\\");
        if (!Directory.Exists(gradleExportFolder))
        {
            UnityEngine.Debug.LogError("Could not find the gradle project at the expected path: " + gradleExportFolder);
            return false;
        }

        // Search for output APK in gradle output directory
        apkPathLocal = Path.Combine(gradleExportFolder, productName + "-debug.apk");
        if (!File.Exists(apkPathLocal))
        {
            UnityEngine.Debug.LogError(string.Format("Could not find {0} in the gradle project.", productName + "-debug.apk"));
            return false;
        }

        string output, error;
        DateTime timerStart;

        // Ensure that the temp directory is on the device by making it
        IncrementProgressBar("Making Temp directory on device");
        string[] mkdirCommand = { "-d shell", "mkdir -p", REMOTE_APK_PATH };
        if (PXR_ADBTool.GetInstance().RunCommand(mkdirCommand, null, out output, out error) != 0) return false;

        // Push APK to device, also time how long it takes
        timerStart = DateTime.Now;
        IncrementProgressBar("Pushing APK to device . . .");
        string[] pushCommand = { "-d push", "\"" + apkPathLocal + "\"", REMOTE_APK_PATH };
        if (PXR_ADBTool.GetInstance().RunCommand(pushCommand, null, out output, out error) != 0) return false;

        // Calculate the transfer speed and determine if user is using USB 2.0 or 3.0
        TimeSpan pushTime = System.DateTime.Now - timerStart;
        bool trivialPush = pushTime.TotalSeconds < TRANSFER_SPEED_CHECK_THRESHOLD;
        long? apkSize = (trivialPush ? (long?)null : new System.IO.FileInfo(apkPathLocal).Length);
        double? transferSpeed = (apkSize / pushTime.TotalSeconds) / BYTES_TO_MEGABYTES;
        bool informLog = transferSpeed.HasValue && transferSpeed.Value < USB_TRANSFER_SPEED_THRES;
        UnityEngine.Debug.Log("PXR ADB Tool: Push Success");

        // Install the APK package on the device
        IncrementProgressBar("Installing APK . . .");
        string apkPath = REMOTE_APK_PATH + "/" + productName + "-debug.apk";
        apkPath = apkPath.Replace(" ", "\\ ");
        string[] installCommand = { "-d shell", "pm install -r", apkPath };

        timerStart = DateTime.Now;
        if (PXR_ADBTool.GetInstance().RunCommand(installCommand, null, out output, out error) != 0) return false;
        TimeSpan installTime = System.DateTime.Now - timerStart;
        UnityEngine.Debug.Log("PXR ADB Tool: Install Success");

        // Start the application on the device
        IncrementProgressBar("Launching application on device . . .");
#if UNITY_2019_3_OR_NEWER
        string playerActivityName = "\"" + applicationIdentifier + "/com.unity3d.player.UnityPlayerActivity\"";
#else
            string playerActivityName = "\"" + applicationIdentifier + "/" + applicationIdentifier + ".UnityPlayerActivity\"";
#endif
        string[] appStartCommand = { "-d shell", "am start -a android.intent.action.MAIN -c android.intent.category.LAUNCHER -S -f 0x10200000 -n", playerActivityName };
        if (PXR_ADBTool.GetInstance().RunCommand(appStartCommand, null, out output, out error) != 0) return false;
        UnityEngine.Debug.Log("PXR ADB Tool: Application Start Success");

        // Send back metrics on push and install steps
        IncrementProgressBar("Success!");

        // If the user is using a USB 2.0 cable, inform them about improved transfer speeds and estimate time saved
        if (informLog)
        {
            var usb3Time = apkSize.Value / (USB_3_TRANSFER_SPEED * BYTES_TO_MEGABYTES);
            UnityEngine.Debug.Log(string.Format("Build has detected slow transfer speeds. A USB 3.0 cable is recommended to reduce the time it takes to deploy your project by approximatly {0:0.0} seconds", pushTime.TotalSeconds - usb3Time));
            return true;
        }
        return false;
    }

    private static void SetupDirectories()
    {
        gradleTempExport = Path.Combine(Path.Combine(Application.dataPath, "../Temp"), "PXRGradleTempExport");
        gradleExport = Path.Combine(Path.Combine(Application.dataPath, "../Temp"), "PXRGradleExport");
        if (!Directory.Exists(gradleExport))
        {
            Directory.CreateDirectory(gradleExport);
        }
    }

    private static void InitializeProgressBar(int stepCount)
    {
        currentStep = 0;
        totalBuildSteps = stepCount;
    }

    private static void IncrementProgressBar(string message)
    {
        currentStep++;
        progressMessage = message;
        UnityEngine.Debug.Log("PXR Build: " + message);
    }

    private static void SetProgressBarMessage(string message)
    {
        progressMessage = message;
        UnityEngine.Debug.Log("PXR Build: " + message);
    }
#endif
}