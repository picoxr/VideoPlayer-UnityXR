/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.XR.PXR;

namespace Unity.XR.PXR.Editor
{
    [CustomEditor(typeof(PXR_Settings))]
    public class PXR_SettingsEditor : UnityEditor.Editor
    {
        private const string StereoRenderingModeAndroid = "stereoRenderingModeAndroid";
        private const string UseDefaultRenderTexture = "useDefaultRenderTexture";
        private const string EyeRenderTextureResolution = "eyeRenderTextureResolution";

        static GUIContent guiStereoRenderingMode = EditorGUIUtility.TrTextContent("Stereo Rendering Mode");
        static GUIContent guiUseDefaultRenderTexture = EditorGUIUtility.TrTextContent("Use Default Render Texture");
        static GUIContent guiEyeRenderTextureResolution = EditorGUIUtility.TrTextContent("Render Texture Resolution");


        private SerializedProperty stereoRenderingModeAndroid;
        private SerializedProperty useDefaultRenderTexture;
        private SerializedProperty eyeRenderTextureResolution;

        void OnEnable()
        {
            if (stereoRenderingModeAndroid == null) 
                stereoRenderingModeAndroid = serializedObject.FindProperty(StereoRenderingModeAndroid);
            if (useDefaultRenderTexture == null) 
                useDefaultRenderTexture = serializedObject.FindProperty(UseDefaultRenderTexture);
            if (eyeRenderTextureResolution == null) 
                eyeRenderTextureResolution = serializedObject.FindProperty(EyeRenderTextureResolution);
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            serializedObject.Update();

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("PicoXR settings cannot be changed when the editor is in play mode.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {
                EditorGUILayout.PropertyField(stereoRenderingModeAndroid, guiStereoRenderingMode);
                EditorGUILayout.PropertyField(useDefaultRenderTexture, guiUseDefaultRenderTexture);
                if (!((PXR_Settings)target).useDefaultRenderTexture)
                {
                    EditorGUILayout.PropertyField(eyeRenderTextureResolution, guiEyeRenderTextureResolution);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
