/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.XR.PXR
{
    public class PXR_Manager : MonoBehaviour
    {
        private static PXR_Manager instance = null;
        public static PXR_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PXR_Manager>();
                    if (instance == null)
                    {
                        Debug.LogError("PXR_Manager instance is not initialized!");
                    }
                }
                return instance;
            }
        }

        private int lastBoundaryState = 0;

        private float[] fixedData = new float[7] { 0, 0, 0, 1, 0, 0, 0 };
        private Camera eyeCamera;

        private int eyeCameraOriginCullingMask;
        private CameraClearFlags eyeCameraOriginClearFlag;
        private Color eyeCameraOriginBackgroundColor;

        [HideInInspector]
        public bool showFPS;
        [HideInInspector]
        public bool useDefaultFps = true;
        [HideInInspector]
        public int customFps;
        [HideInInspector]
        public bool screenFade;
        [HideInInspector]
        public bool eyeTracking;
        [HideInInspector]
        public bool supportEyeTracking;
        [HideInInspector]
        public FoveationLevel foveationLevel = FoveationLevel.None;

        private bool isNeedResume = false;
        [HideInInspector]
        public int systemDebugFFRLevel = -1;
        [HideInInspector]
        public int systemFFRLevel = -1;

        //Entitlement Check Result
        [HideInInspector]
        public int appCheckResult = 100;
        public delegate void EntitlementCheckResult(int ReturnValue);
        public static event EntitlementCheckResult EntitlementCheckResultEvent;

        // Start is called before the first frame update
        void Awake()
        {
            PXR_Plugin.UPxr_InitAndroidClass();
            PXR_Plugin.System.UPxr_SetSecure(PXR_ProjectSetting.GetProjectConfig().useContentProtect);
            PXR_Plugin.PlatformSetting.UPxr_BindVerifyService(gameObject.name);
            eyeCamera = Camera.main;
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
#if !UNITY_EDITOR
            int fps = -1;
            int rate = (int)GlobalIntConfigs.TargetFrameRate;
            PXR_Plugin.System.UPxr_GetIntConfig(rate, ref fps);
            float freshRate = 0.0f;
            int frame = (int)GlobalFloatConfigs.DisplayRefreshRate;
            PXR_Plugin.System.UPxr_GetFloatConfig(frame, ref freshRate);
            Application.targetFrameRate = fps > 0 ? fps : (int)freshRate;
            if (!useDefaultFps)
            {
                if (customFps <= freshRate)
                {
                    Application.targetFrameRate = customFps;
                }
                else
                {
                    Application.targetFrameRate = (int) freshRate;
                }
            }
#endif
            UpateSystemFFRLevel();
            if (foveationLevel != FoveationLevel.None)
            {
                PXR_Plugin.Render.UPxr_EnableFoveation(true);
                PXR_Plugin.Render.UPxr_SetFoveationLevel(foveationLevel);
            }
            PXR_Plugin.System.UPxr_InitKeyEventManager();
            PXR_Plugin.System.UPxr_StartReceiver();
            //version log
            Debug.Log("XR Platform----SDK Version:" + PXR_Plugin.System.UPxr_GetSDKVersion() + " Unity Script Version:" + PXR_Plugin.System.UPxr_GetUnitySDKVersion());
        }

        void OnEnable()
        {
            if (PXR_OverLay.Instances.Count > 0)
            {
                if (Camera.main.gameObject.GetComponent<PXR_OverlayManager>() == null)
                {
                    Camera.main.gameObject.AddComponent<PXR_OverlayManager>();
                }

                foreach (var layer in PXR_OverLay.Instances)
                {
                    layer.RefreshCamera();
                }
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                PXR_Plugin.System.UPxr_StopHomeKeyReceiver();
            }
            else
            {
                PXR_Plugin.System.UPxr_StartHomeKeyReceiver(gameObject.name);
                if (PXR_Plugin.System.UPxr_Check6DofAppResume())
                {
                    StopXR();
                    isNeedResume = true;
                }
                else
                {
                    if (isNeedResume)
                    {
                        StartCoroutine("StartXR");
                        isNeedResume = false;
                    } 
                }

                UpateSystemFFRLevel();
            }
        }

        private void UpateSystemFFRLevel()
        {
            if (PXR_Plugin.Render.UPxr_GetIntSysProc("pvrsist.foveation.level", ref systemDebugFFRLevel))
            {
                PXR_Plugin.Render.UPxr_SetFoveationLevel((FoveationLevel)(systemDebugFFRLevel));
                Debug.Log("XR Platform OnResume Get System Debug ffr level is : " + systemDebugFFRLevel);
            }
            else
            {
                Debug.Log("XR Platform OnResume Get System Debug ffr level Error,ffr level is : " + systemDebugFFRLevel);
            }

            if (systemDebugFFRLevel == -1)
            {
                PXR_Plugin.System.UPxr_GetIntConfig((int)GlobalIntConfigs.EnableFFR, ref systemFFRLevel);
                if (systemFFRLevel != -1 && systemFFRLevel > (int)foveationLevel)
                {
                    PXR_Plugin.Render.UPxr_SetFoveationLevel((FoveationLevel)(systemFFRLevel));
                    Debug.Log("XR Platform OnResume Get System ffr level is : " + systemFFRLevel);
                }
            }
        }

        public IEnumerator StartXR()
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed. Check log for details.");
            }
            else
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        void StopXR()
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        void Start()
        {
#if !UNITY_EDITOR
            var loader = XRGeneralSettings.Instance.Manager.activeLoader as PXR_Loader;
            var w = 2048;
            var h = 2048;
            if (loader != null)
            {
                if (loader.GetSettings().useDefaultRenderTexture)
                {
                    int enumindex = (int)GlobalIntConfigs.EyeTextureResolution0;
                    PXR_Plugin.System.UPxr_GetIntConfig(enumindex, ref w);
                    enumindex = (int)GlobalIntConfigs.EyeTextureResolution1;
                    PXR_Plugin.System.UPxr_GetIntConfig(enumindex, ref h);
                }
                else
                {
                    w = (int)loader.GetSettings().eyeRenderTextureResolution.x;
                    h = (int)loader.GetSettings().eyeRenderTextureResolution.y;
                }
            }
            PXR_Plugin.Boundary.UPxr_SetViewportSize(w, h);
#endif

            PXR_Plugin.System.UPxr_StartHomeKeyReceiver(gameObject.name);
            int fps = 0;
            int rate = (int)GlobalIntConfigs.IsShowFps;
            PXR_Plugin.System.UPxr_GetIntConfig(rate,  ref fps);
            if (Convert.ToBoolean(fps) || showFPS)
            {
                Camera.main.transform.Find("[FPS]").gameObject.SetActive(true);
            }

            if (PXR_PlatformSetting.Instance.startTimeEntitlementCheck)
            {
                if (PXR_Plugin.PlatformSetting.UPxr_IsCurrentDeviceValid() != PXR_PlatformSetting.simulationType.Valid)
                {
                    Debug.Log("PXRLog Entitlement Check Simulation DO NOT PASS");
                    string appID = PXR_PlatformSetting.Instance.appID;
                    Debug.Log("PXRLog Entilement Check Enable");
                    // 0:success -1:invalid params -2:service not exist -3:time out
                    PXR_Plugin.PlatformSetting.UPxr_AppEntitlementCheckExtra(appID);
                }
                else
                {
                    Debug.Log("PXRLog Entitlement Check Simulation PASS");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            //set controller data to boundary
            fixedData = PXR_Plugin.Controller.UPxr_GetControllerFixedSensorState(0);
            PXR_Plugin.Boundary.UPxr_SetReinPosition(fixedData[0], fixedData[1], fixedData[2], fixedData[3], fixedData[4], fixedData[5], fixedData[6], 0, PXR_Input.IsControllerConnected(PXR_Input.Controller.LeftController), 0);
            fixedData = PXR_Plugin.Controller.UPxr_GetControllerFixedSensorState(1);
            PXR_Plugin.Boundary.UPxr_SetReinPosition(fixedData[0], fixedData[1], fixedData[2], fixedData[3], fixedData[4], fixedData[5], fixedData[6], 1, PXR_Input.IsControllerConnected(PXR_Input.Controller.RightController), 0);

            // boundary
            if (eyeCamera != null && eyeCamera.enabled)
            {
                int currentBoundaryState = PXR_Plugin.Boundary.UPxr_GetSeeThroughState();

                if (currentBoundaryState != lastBoundaryState)
                {
                    if (currentBoundaryState == 2) // close camera render(close camera render) and limit framerate(if needed)
                    {
                        // record
                        eyeCameraOriginCullingMask = eyeCamera.cullingMask;
                        eyeCameraOriginClearFlag = eyeCamera.clearFlags;
                        eyeCameraOriginBackgroundColor = eyeCamera.backgroundColor;

                        // close render
                        eyeCamera.cullingMask = 0;
                        eyeCamera.clearFlags = CameraClearFlags.SolidColor;
                        eyeCamera.backgroundColor = Color.black;
                    }
                    else if (currentBoundaryState == 1) // open camera render, but limit framerate(if needed)
                    {
                        if (lastBoundaryState == 2)
                        {
                            if (eyeCamera.cullingMask == 0)
                            {
                                eyeCamera.cullingMask = eyeCameraOriginCullingMask;
                            }
                            if (eyeCamera.clearFlags == CameraClearFlags.SolidColor)
                            {
                                eyeCamera.clearFlags = eyeCameraOriginClearFlag;
                            }
                            if (eyeCamera.backgroundColor == Color.black)
                            {
                                eyeCamera.backgroundColor = eyeCameraOriginBackgroundColor;
                            }
                        }
                    }
                    else // open camera render(recover)
                    {
                        if ((lastBoundaryState == 2 || lastBoundaryState == 1))
                        {
                            if (eyeCamera.cullingMask == 0)
                            {
                                eyeCamera.cullingMask = eyeCameraOriginCullingMask;
                            }
                            if (eyeCamera.clearFlags == CameraClearFlags.SolidColor)
                            {
                                eyeCamera.clearFlags = eyeCameraOriginClearFlag;
                            }
                            if (eyeCamera.backgroundColor == Color.black)
                            {
                                eyeCamera.backgroundColor = eyeCameraOriginBackgroundColor;
                            }
                        }
                    }
                    lastBoundaryState = currentBoundaryState;
                }
            }
        }
        void OnDisable()
        {
            StopAllCoroutines();
        }

        //bind verify service success call back
        void BindVerifyServiceCallback()
        {
            
        }

        private void verifyAPPCallback(string callback)
        {
            Debug.Log("PXRLog verifyAPPCallback" + callback);
            appCheckResult = Convert.ToInt32(callback);
            if (EntitlementCheckResultEvent != null)
            {
                EntitlementCheckResultEvent(appCheckResult);
            }
        }

        public void IpdRefreshCallBack(string ipd)
        {
            Camera.main.stereoSeparation = Convert.ToSingle(ipd);
        }

        //home key long pressed call back
        public void setLongHomeKey()
        {
            PXR_Plugin.Sensor.UPxr_OptionalResetSensor(1, 1);
        }

    }
}

