/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

using System.Runtime.InteropServices;
using Unity.XR.PXR;
using UnityEngine.Networking.Types;
using Debug = UnityEngine.Debug;

namespace Unity.XR.PXR.Editor
{
    [InitializeOnLoad]
    public static class PXR_SDKBuildCheck
    {
        private static bool doNotShowAgain = false;

        static PXR_SDKBuildCheck()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);
            doNotShowAgain = GetDoNotShowBuildWarning();
            Debug.Log("[Build Check] RegisterBuildPlayerHandler,Already Do not show: " + doNotShowAgain);
        }
        static bool GetDoNotShowBuildWarning()
        {
            string path = PXR_SDKSettingEditor.assetPath + typeof(PXR_SDKSettingAsset).ToString() + ".asset";
            if (File.Exists(path))
            {
                PXR_SDKSettingAsset asset = AssetDatabase.LoadAssetAtPath<PXR_SDKSettingAsset>(path);
                if (asset != null)
                {
                    return asset.doNotShowBuildWarning;
                }

            }
            return false;
        }

        public static void SetDoNotShowBuildWarning()
        {
            string path = PXR_SDKSettingEditor.assetPath + typeof(PXR_SDKSettingAsset).ToString() + ".asset";
            PXR_SDKSettingAsset asset = AssetDatabase.LoadAssetAtPath<PXR_SDKSettingAsset>(path);
            if (File.Exists(path))
            {

                asset.doNotShowBuildWarning = true;
            }
            else
            {
                asset = new PXR_SDKSettingAsset();
                ScriptableObjectUtility.CreateAsset<PXR_SDKSettingAsset>(asset, PXR_SDKSettingEditor.assetPath);
            }
            asset.doNotShowBuildWarning = true;
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();//must Refresh
        }

        public static void OnBuild(BuildPlayerOptions options)
        {
            if (!doNotShowAgain)
            {
                if (!PXR_PlatformSetting.Instance.startTimeEntitlementCheck)
                {
                    int result = EditorUtility.DisplayDialogComplex("Start-time Entitlement Check",
                        "EntitlementCheck is highly recommend which can\nprotect the copyright of app. Enable it now?",
                        "OK", "Ignore", "Ignore, Don't remind again");

                    switch (@result)
                    {
                        // ok
                        case 0:
                            PXR_PlatformSettingEditor.Edit();
                            throw new System.OperationCanceledException("Build was canceled by the user.");
                        //cancel
                        case 1:
                            Debug.LogWarning("Warning: EntitlementCheck is highly recommended which can protect the copyright of app. You can enable it when App start-up in the Inspector of \"Menu/PXR_SDK/PlatformSettings\" and Enter your APPID. If you want to call the APIs as needed, please refer to the development Document.");
                            Debug.Log("[Build Check] Start-time Entitlement Check Cancel The StartTime Entitlement Check status: " + PXR_PlatformSetting.Instance.startTimeEntitlementCheck.ToString());

                            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
                            break;
                        //alt
                        case 2:
                            doNotShowAgain = true;
                            SetDoNotShowBuildWarning();
                            Debug.LogWarning("Warning: EntitlementCheck is highly recommended which can protect the copyright of app. You can enable it when App start-up in the Inspector of \"Menu/PXR_SDK/PlatformSettings\" and Enter your APPID. If you want to call the APIs as needed, please refer to the development Document.");
                            Debug.Log("[Build Check] Start-time Entitlement Check Do not show again The StartTime Entitlement Check status: " + PXR_PlatformSetting.Instance.startTimeEntitlementCheck.ToString());

                            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
                            break;
                    }
                }
                else
                {
                    Debug.Log("[Build Check]1 Enable Start-time Entitlement Check:" + PXR_PlatformSetting.Instance.startTimeEntitlementCheck + ", your AppID :" + PXR_PlatformSetting.Instance.appID);
                    BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
                }
            }
            else
            {
                Debug.Log("[Build Check]2 Enable Start-time Entitlement Check:" + PXR_PlatformSetting.Instance.startTimeEntitlementCheck + ", your AppID :" + PXR_PlatformSetting.Instance.appID);
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
            }
        }

    }
}

