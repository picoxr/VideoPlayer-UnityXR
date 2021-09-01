/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public sealed class PXR_PlatformSetting : ScriptableObject
    {
        public enum simulationType
        {
            Null,
            Invalid,
            Valid,
        }

        [SerializeField]
        public bool entitlementCheckSimulation;
        [SerializeField]
        public bool startTimeEntitlementCheck;
        [SerializeField]
        public string appID;

        public List<string> deviceSN = new List<string>();

        private static PXR_PlatformSetting instance;
        public static PXR_PlatformSetting Instance
        {
            get
            {
                if (instance == null)
                {

                    instance = Resources.Load<PXR_PlatformSetting>("PlatformSetting");
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<PXR_PlatformSetting>();
                        UnityEditor.AssetDatabase.CreateAsset(instance, "Packages/com.unity.xr.picoxr/Assets/Resources/PlatformSetting.asset");
                    }
#endif
                }
                return instance;
            }

            set { instance = value; }
        }

    }
}


