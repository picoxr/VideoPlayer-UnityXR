/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_Manager))]
    public class PXR_ManagerEditor : UnityEditor.Editor
    {
        private GameObject fpsObject = null;

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            DrawDefaultInspector();

            PXR_Manager manager = (PXR_Manager)target;
            PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();

            if (Camera.main != null)
            {
                if (!Camera.main.transform.Find("[FPS]"))
                {
                    fpsObject = Instantiate(Resources.Load<GameObject>("Prefabs/[FPS]"), Camera.main.transform, false);
                    fpsObject.name = "[FPS]";
                    fpsObject.SetActive(false);
                }
                else
                {
                    fpsObject = Camera.main.transform.Find("[FPS]").gameObject;
                }
            }
            // Fps and Screen Fade
            manager.showFPS = EditorGUILayout.Toggle("Show FPS", manager.showFPS);

            manager.useDefaultFps = EditorGUILayout.Toggle("Use Default FPS", manager.useDefaultFps);
            if (!manager.useDefaultFps)
            {
                manager.customFps = EditorGUILayout.IntField("    FPS", manager.customFps);
            }

            manager.screenFade = EditorGUILayout.Toggle("Open Screen Fade", manager.screenFade);
            if (Camera.main != null)
            {
                var head = Camera.main.transform;
                if (head)
                {
                    var fade = head.GetComponent<PXR_ScreenFade>();
                    if (manager.screenFade)
                    {
                        if (!fade)
                        {
                            head.gameObject.AddComponent<PXR_ScreenFade>();
                            Selection.activeObject = head;
                        }
                    }
                    else
                    {
                        if (fade) DestroyImmediate(fade);
                    }
                }
            }
            //ffr
            manager.foveationLevel = (FoveationLevel)EditorGUILayout.EnumPopup("Foveation Level", manager.foveationLevel);

            //eye tracking
            GUIStyle firstLevelStyle = new GUIStyle(GUI.skin.label);
            firstLevelStyle.alignment = TextAnchor.UpperLeft;
            firstLevelStyle.fontStyle = FontStyle.Bold;
            firstLevelStyle.fontSize = 12;
            firstLevelStyle.wordWrap = true;
            var guiContent = new GUIContent();
            guiContent.text = "Eye Tracking";
            guiContent.tooltip = "Before calling EyeTracking API, enable this option first, only for Neo 2 Eye device.";
            manager.eyeTracking = EditorGUILayout.Toggle(guiContent, manager.eyeTracking);
            if (manager.eyeTracking)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Note:", firstLevelStyle);
                EditorGUILayout.LabelField("EyeTracking is supported only on the Neo2 Eye");
                EditorGUILayout.EndVertical();
            }

            // content protect
            projectConfig.useContentProtect = EditorGUILayout.Toggle("Use Content Protect", projectConfig.useContentProtect);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(projectConfig);
                EditorUtility.SetDirty(manager);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}


