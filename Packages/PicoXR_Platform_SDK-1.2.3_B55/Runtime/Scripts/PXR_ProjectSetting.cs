/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using UnityEngine;

namespace Unity.XR.PXR
{
    [System.Serializable]
    public class PXR_ProjectSetting: ScriptableObject
    {
        public bool useContentProtect;
        public static PXR_ProjectSetting GetProjectConfig()
        {
            PXR_ProjectSetting projectConfig = Resources.Load<PXR_ProjectSetting>("ProjectSetting");
#if UNITY_EDITOR
            if (projectConfig == null)
            {
                projectConfig = CreateInstance<PXR_ProjectSetting>();
                projectConfig.useContentProtect = false;
                UnityEditor.AssetDatabase.CreateAsset(projectConfig, "Packages/com.unity.xr.picoxr/Assets/Resources/ProjectSetting.asset");
            }
#endif
            return projectConfig;
        }
    }
}
