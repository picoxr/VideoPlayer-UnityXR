/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
using Object = UnityEngine.Object;

namespace Unity.XR.PXR.Editor
{
    public class PXR_BuildProcessor : XRBuildHelper<PXR_Settings>
    {
        public override string BuildSettingsKey { get {return "Unity.XR.PXR.Settings";} }
    }

    public static class PXR_BuildTools
    {
        public static bool LoaderPresentInSettingsForBuildTarget(BuildTargetGroup btg)
        {
            var generalSettingsForBuildTarget = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(btg);
            if (!generalSettingsForBuildTarget)
                return false;
            var settings = generalSettingsForBuildTarget.AssignedSettings;
            if (!settings)
                return false;
            List<XRLoader> loaders = settings.loaders;
            return loaders.Exists(loader => loader is PXR_Loader);
        }

        public static PXR_Settings GetSettings()
        {
            PXR_Settings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<PXR_Settings>("Unity.XR.PXR.Settings", out settings);
#else
            settings = PXR_Settings.settings;
#endif
            return settings;
        }
    }

    internal class PXR_PrebuildSettings : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)

        {
            if(!PXR_BuildTools.LoaderPresentInSettingsForBuildTarget(report.summary.platformGroup))
                return;
            if (report.summary.platformGroup == BuildTargetGroup.Android)
            {
                GraphicsDeviceType firstGfxType = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0];
                if (firstGfxType != GraphicsDeviceType.OpenGLES3 && firstGfxType != GraphicsDeviceType.Vulkan && firstGfxType != GraphicsDeviceType.OpenGLES2)
                {
                    throw new BuildFailedException("OpenGLES2, OpenGLES3, and Vulkan are currently the only graphics APIs compatible with the PicoVR XR Plugin on mobile platforms.");
                }
                if (PXR_BuildTools.GetSettings().stereoRenderingModeAndroid == PXR_Settings.StereoRenderingModeAndroid.Multiview && firstGfxType == GraphicsDeviceType.OpenGLES2)
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3 });
                }
                if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel26)
                {
                    throw new BuildFailedException("Minimum API must be set to 26 or higher for Pico XR Plugin.");
                }
                PlayerSettings.Android.forceSDCardPermission = true;
            }
        }
    }

#if UNITY_ANDROID
    internal class PXR_Manifest : IPostGenerateGradleAndroidProject
    {
        static readonly string androidURI = "http://schemas.android.com/apk/res/android";

        static readonly string androidManifestPath = "/src/main/AndroidManifest.xml";

        void UpdateOrCreateAttributeInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
        {
            var xmlNode = doc.SelectSingleNode(parentPath + "/" + tag);

            if (xmlNode != null)
            {
                ((XmlElement)xmlNode).SetAttribute(name,androidURI, value);
            }
        }

        void UpdateOrCreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
            string firstName, string firstValue, string secondName, string secondValue)
        {
            var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

            foreach (XmlNode node in xmlNodeList)
            {
                var attributeList = ((XmlElement)node).Attributes;

                foreach (XmlAttribute attrib in attributeList)
                {
                    if (attrib.Value == firstValue)
                    {
                        XmlAttribute valueAttrib = attributeList[secondName, androidURI];
                        if (valueAttrib != null)
                        {
                            valueAttrib.Value = secondValue;
                        }
                        else
                        {
                            ((XmlElement)node).SetAttribute(secondName, androidURI, secondValue);
                        }
                        return;
                    }
                }
            }
            
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(firstName, androidURI, firstValue);
            childElement.SetAttribute(secondName, androidURI, secondValue);

            var xmlParentNode = doc.SelectSingleNode(parentPath);

            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        void CreateNameValueElementInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
        {
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(name, androidURI, value);
            var xmlParentNode = doc.SelectSingleNode(parentPath);
            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        void CreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
            string firstName, string firstValue, string secondName = null, string secondValue = null, string thirdName=null, string thirdValue=null)
        {
            var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

            // don't create if the firstValue matches
            foreach (XmlNode node in xmlNodeList)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (attrib.Value == firstValue)
                    {
                        return;
                    }
                }
            }
            
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(firstName, androidURI, firstValue);

            if (secondValue != null)
            {
                childElement.SetAttribute(secondName, androidURI, secondValue);
            }
            if (thirdValue != null)
            {
                childElement.SetAttribute(thirdName, androidURI, thirdValue);
            }
            var xmlParentNode = doc.SelectSingleNode(parentPath);

            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if(!PXR_BuildTools.LoaderPresentInSettingsForBuildTarget(BuildTargetGroup.Android))
               return;

            var manifestPath = path + androidManifestPath;
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);
            var sdkVersion = (int)PlayerSettings.Android.minSdkVersion;
            var nodePath = "/manifest/application";
			UpdateOrCreateAttributeInTag(manifestDoc, "/manifest","application", "requestLegacyExternalStorage","true");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "pvr.app.type", "value", "vr");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "pvr.sdk.version", "value", "XR Platform_1.2.3.5");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "enable_cpt", "value", PXR_ProjectSetting.GetProjectConfig().useContentProtect ? "1" : "0");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "enable_entitlementcheck", "value", PXR_PlatformSetting.Instance.startTimeEntitlementCheck ? "1" : "0");
            CreateNameValueElementsInTag(manifestDoc, "/manifest", "uses-permission","name", "android.permission.WRITE_SETTINGS");
			CreateNameValueElementsInTag(manifestDoc, "/manifest", "uses-permission","name", "android.permission.READ_EXTERNAL_STORAGE");
			CreateNameValueElementsInTag(manifestDoc, "/manifest", "uses-permission","name", "android.permission.WRITE_EXTERNAL_STORAGE");			

			nodePath = "/manifest";
            manifestDoc.Save(manifestPath);
        }

        public int callbackOrder { get { return 10000; } }
    }
#endif

    public static class PXR_BuildHelper
    {
        public static void AddBackgroundShaderToProject(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName))
            {
                Debug.LogWarning("Incompatible render pipeline in GraphicsSettings.currentRenderPipeline. Background "
                                 + "rendering may not operate properly.");
            }
            else
            {
                Shader shader = FindShaderOrFailBuild(shaderName);
                Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
                var shaderAssets = (from preloadedAsset in preloadedAssets
                                    where shader.Equals(preloadedAsset)
                                    select preloadedAsset);
                if ((shaderAssets == null) || !shaderAssets.Any())
                {
                    List<Object> preloadedAssetsList = preloadedAssets.ToList();
                    preloadedAssetsList.Add(shader);
                    PlayerSettings.SetPreloadedAssets(preloadedAssetsList.ToArray());
                }
            }
        }

        public static void RemoveShaderFromProject(string shaderName)
        {
            if (!string.IsNullOrEmpty(shaderName))
            {
                Shader shader = FindShaderOrFailBuild(shaderName);

                Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

                var nonShaderAssets = (from preloadedAsset in preloadedAssets
                                       where !shader.Equals(preloadedAsset)
                                       select preloadedAsset);
                PlayerSettings.SetPreloadedAssets(nonShaderAssets.ToArray());
            }
        }

        static Shader FindShaderOrFailBuild(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                throw new BuildFailedException($"Cannot find shader '{shaderName}'");
            }
            return shader;
        }
    }
}
