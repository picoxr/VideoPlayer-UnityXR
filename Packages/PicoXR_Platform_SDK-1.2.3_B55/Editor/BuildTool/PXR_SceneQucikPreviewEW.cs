#if UNITY_EDITOR_WIN && UNITY_ANDROID

using UnityEngine;
using UnityEditor;

public class PXR_SceneQucikPreviewEW : EditorWindow
{
    private static GUIStyle logStyle;
    private static string log;
    private bool forceRestart = false;

    
    [MenuItem("PXR_SDK/Build Tool/Scene Quick Preview", false, 10)]
    static void OpenSceneQuickPreviewUI()
    {
        GetWindow<PXR_SceneQucikPreviewEW>();
        PXR_ADBTool.GetInstance().CheckADBDevices(log =>
        {
            if (!string.IsNullOrEmpty(log))
            {
                PrintError(log);
            }
        });
        EditorBuildSettings.sceneListChanged += PXR_BuildToolManager.GetScenesEnabled;
    }

    private void OnEnable()
    {
        PXR_BuildToolManager.GetScenesEnabled();
        EditorBuildSettings.sceneListChanged += PXR_BuildToolManager.GetScenesEnabled;
    }

    private void OnDestroy()
    {
        PXR_BuildToolManager.DeleteCachePreviewIndexFile();
        log = null;
    }

    private void OnGUI()
    {
        this.titleContent.text = "Scene Quick Preview";
        InitLogUI();
        GUILayout.Space(10.0f);
        GUILayout.Label("Scenes", EditorStyles.boldLabel);
        GUIContent selectScenesBtn = new GUIContent("Select Scenes");

        foreach (PXR_BuildToolManager.SceneInfo scene in PXR_BuildToolManager.buildSceneInfoList)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(scene.sceneName, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUIContent sceneBtnTxt = new GUIContent("Build Scene(s)");
            var sceneBtnRt = GUILayoutUtility.GetRect(sceneBtnTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(sceneBtnRt, sceneBtnTxt))
            {
                PXR_BuildToolManager.BuildScenes(forceRestart);
                GUIUtility.ExitGUI();
            }

            GUIContent forceRestartLabel = new GUIContent("Force Restart [?]", "Restart the app after scene bundles are finished deploying.");
            forceRestart = GUILayout.Toggle(forceRestart, forceRestartLabel, GUILayout.Width(200));

            var buildSettingBtnRt = GUILayoutUtility.GetRect(selectScenesBtn, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(buildSettingBtnRt, selectScenesBtn))
            {
                GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10.0f);
        GUILayout.Label("Utilities", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        {
            GUIContent restartBtnTxt = new GUIContent("Restart App");
            var restartBtnRt = GUILayoutUtility.GetRect(restartBtnTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(restartBtnRt, restartBtnTxt))
            {
                if (!PXR_BuildToolManager.IsInstalledAPP())
                {
                    PrintError("App is not install.");
                    return;
                }

                PXR_BuildToolManager.RestartApp();
            }

            GUIContent uninstallTxt = new GUIContent("Uninstall APP");
            var uninstallBtnRt = GUILayoutUtility.GetRect(uninstallTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(uninstallBtnRt, uninstallTxt))
            {
                PXR_BuildToolManager.UninstallAPP();
                PXR_BuildToolManager.IsInstalledAPP();
            }

            GUIContent deleteCacheBundlesTxt = new GUIContent("Delete Cache Bundles");
            var deleteCacheBundlesBtnRt = GUILayoutUtility.GetRect(deleteCacheBundlesTxt, GUI.skin.button, GUILayout.Width(200));
            if (GUI.Button(deleteCacheBundlesBtnRt, deleteCacheBundlesTxt))
            {
                PXR_BuildToolManager.DeleteCacheBundles();
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10.0f);
        GUILayout.Label("Log", EditorStyles.boldLabel);

        if (!string.IsNullOrEmpty(log))
        {
            GUILayout.Label(log, logStyle, GUILayout.Height(30.0f));
        }
    }

    private static void InitLogUI()
    {
        if (logStyle == null)
        {
            logStyle = new GUIStyle();
            logStyle.margin.left = 5;
            logStyle.wordWrap = true;
            logStyle.normal.textColor = logStyle.focused.textColor = EditorStyles.label.normal.textColor;
            logStyle.richText = true;
        }
    }

    public static void PrintLog(string message)
    {
        log = message;
    }

    public static void PrintError(string error = "")
    {
        log = "<color=red>Failed! " + error + "</color>\n";
    }

    public static void PrintWarning(string warning)
    {
        log = "<color=yellow>Warning! " + warning + "</color>\n";
    }

    public static void PrintSuccess(string msg = "")
    {
        log = "<color=green>Success! " + msg + "</color>\n";
    }
}
#endif