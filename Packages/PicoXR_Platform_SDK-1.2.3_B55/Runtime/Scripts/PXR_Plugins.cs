/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

#if !UNITY_EDITOR && UNITY_ANDROID 
#define ANDROID_DEVICE
#endif

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public enum RenderEvent
    {
        CreateTexture,
        DeleteTexture,
        UpdateTexture
    }

    public enum FoveationLevel
    {
        None = -1,
        Low = 0,
        Med = 1,
        High = 2,
        TopHigh = 3
    }

    public enum BoundaryType
    {
        OuterBoundary,
        PlayArea
    }

    public struct BoundaryTestResult
    {
        public bool IsTriggering;
        public float ClosestDistance;
        public Vector3 ClosestPoint;
        public Vector3 ClosestPointNormal;
    }

    /// <summary>
    /// Boundary Test Node Type
    /// </summary>
    public enum BoundaryTrackingNode
    {
        HandLeft,
        HandRight,
        Head,
    }

    public enum GlobalIntConfigs
    {
        EyeTextureResolution0,
        EyeTextureResolution1,
        SensorCount,
        Ability6Dof,
        PlatformType,
        TrackingMode,
        LogLevel,
        EnableHand6DofByHead,
        Enable6DofGlobalTracking,
        TargetFrameRate,
        IsShowFps,
        SensorMode,
        LogicFlow,
        EyeTextureResHigh,
        EyeTextureResNormal,
        CtrlModelLoadingPri,
        IphoneHmdModeEnabled,
        IsEnableBoundary,
        EnableActivityRotation,
        GetDisplayOrientation,
        GetWaitFrameNum,
        GetResetFrameNum,
        EnableFFR,
    };

    public enum GlobalFloatConfigs
    {
        Ipd,
        Vfov,
        Hfov,
        NeckModelX,
        NeckModelY,
        NeckModelZ,
        DisplayRefreshRate
    };

    public enum ResUtilsType
    {
        TypeTextSize = 0,
        TypeColor = 1,
        TypeText = 2,
        TypeFont = 3,
        TypeValue = 4,
        TypeDrawable = 5,
        TypeObject = 6,
        TypeObjectArray = 7,
    }

    public enum SystemInfoEnum
    {
        ELECTRIC_QUANTITY,
        PUI_VERSION,
        EQUIPMENT_MODEL,
        EQUIPMENT_SN,
        CUSTOMER_SN,
        INTERNAL_STORAGE_SPACE_OF_THE_DEVICE,
        DEVICE_BLUETOOTH_STATUS,
        BLUETOOTH_NAME_CONNECTED,
        BLUETOOTH_MAC_ADDRESS,
        DEVICE_WIFI_STATUS,
        WIFI_NAME_CONNECTED,
        WLAN_MAC_ADDRESS,
        DEVICE_IP
    }
    public enum DeviceControlEnum
    {
        DEVICE_CONTROL_REBOOT,
        DEVICE_CONTROL_SHUTDOWN
    }
    public enum PackageControlEnum
    {
        PACKAGE_SILENCE_INSTALL,
        PACKAGE_SILENCE_UNINSTALL
    }
    public enum SwitchEnum
    {
        S_ON,
        S_OFF
    }
    public enum HomeEventEnum
    {
        SINGLE_CLICK,
        DOUBLE_CLICK,
        LONG_PRESS
    }
    public enum HomeFunctionEnum
    {
        VALUE_HOME_GO_TO_SETTING,
        VALUE_HOME_BACK,
        VALUE_HOME_RECENTER,
        VALUE_HOME_OPEN_APP,
        VALUE_HOME_DISABLE,
        VALUE_HOME_GO_TO_HOME,
        VALUE_HOME_SEND_BROADCAST,
        VALUE_HOME_CLEAN_MEMORY
    }
    public enum ScreenOffDelayTimeEnum
    {
        THREE = 3,
        TEN = 10,
        THIRTY = 30,
        SIXTY = 60,
        THREE_HUNDRED = 300,
        SIX_HUNDRED = 600,
        NEVER = -1
    }
    public enum SleepDelayTimeEnum
    {
        FIFTEEN = 15,
        THIRTY = 30,
        SIXTY = 60,
        THREE_HUNDRED = 300,
        SIX_HUNDRED = 600,
        ONE_THOUSAND_AND_EIGHT_HUNDRED = 1800,
        NEVER = -1
    }
    public enum SystemFunctionSwitchEnum
    {
        SFS_USB,
        SFS_AUTOSLEEP,
        SFS_SCREENON_CHARGING,
        SFS_OTG_CHARGING,
        SFS_RETURN_MENU_IN_2DMODE,
        SFS_COMBINATION_KEY,
        SFS_CALIBRATION_WITH_POWER_ON,
        SFS_SYSTEM_UPDATE,
        SFS_CAST_SERVICE,
        SFS_EYE_PROTECTION,
        SFS_SECURITY_ZONE_PERMANENTLY,
        SFS_GLOBAL_CALIBRATION,
        SFS_Auto_Calibration,
        SFS_USB_BOOT
    }
    public enum USBConfigModeEnum
    {
        MTP,
        CHARGE
    }

    public enum ExtraLatencyMode
    {
        ExtraLatencyModeOff = 0,
        ExtraLatencyModeOn = 1,
        ExtraLatencyModeDynamic = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeTrackingData
    {
        public int leftEyePoseStatus;           //!< Bit field (pxrEyePoseStatus) indicating left eye pose status
        public int rightEyePoseStatus;          //!< Bit field (pxrEyePoseStatus) indicating right eye pose status
        public int combinedEyePoseStatus;       //!< Bit field (pxrEyePoseStatus) indicating combined eye pose status

        public Vector3 leftEyeGazePoint;        //!< Left Eye Gaze Point
        public Vector3 rightEyeGazePoint;       //!< Right Eye Gaze Point
        public Vector3 combinedEyeGazePoint;    //!< Combined Eye Gaze Point (HMD center-eye point)

        public Vector3 leftEyeGazeVector;       //!< Left Eye Gaze Point
        public Vector3 rightEyeGazeVector;      //!< Right Eye Gaze Point
        public Vector3 combinedEyeGazeVector;   //!< Combined Eye Gaze Vector (HMD center-eye point)

        public float leftEyeOpenness;           //!< Left eye value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed.
        public float rightEyeOpenness;          //!< Right eye value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed.

        public float leftEyePupilDilation;      //!< Left eye value in millimeters indicating the pupil dilation
        public float rightEyePupilDilation;     //!< Right eye value in millimeters indicating the pupil dilation

        public Vector3 leftEyePositionGuide;    //!< Position of the inner corner of the left eye in meters from the HMD center-eye coordinate system's origin.
        public Vector3 rightEyePositionGuide;   //!< Position of the inner corner of the right eye in meters from the HMD center-eye coordinate system's origin.

        public Vector3 foveatedGazeDirection;   //!< Position of the gaze direction in meters from the HMD center-eye coordinate system's origin.
        public int foveatedGazeTrackingState;   //!< The current state of the foveated gaze direction signal.

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] reserved;                 //!< reserved
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeTrackingGazeRay
    {
        public Vector3 direction;               // Vector in world space with the gaze direction.
        public bool isValid;                    // IsValid is true when there is available gaze data.
        public Vector3 origin;                  // The middle of the eyes in world space.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeDeviceInfo
    {
        public ViewFrustum targetFrustumLeft;
        public ViewFrustum targetFrustumRight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ViewFrustum
    {
        public float left;                      //!< Left Plane of Frustum
        public float right;                     //!< Right Plane of Frustum
        public float top;                       //!< Top Plane of Frustum
        public float bottom;                    //!< Bottom Plane of Frustum
        public float near;                      //!< Near Plane of Frustum
        public float far;                       //!< Far Plane of Frustum (Arbitrary)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EyeSetting
    {
        public Transform eyeLocalPosition;
        public Rect eyeRect;
        public float eyeFov;
        public float eyeAspect;
        public Matrix4x4 eyeProjectionMatrix;
        public Shader eyeShader;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserDefinedSettings
    {
        public ushort stereoRenderingMode;
        public ushort colorSpace;
        public bool useDefaultRenderTexture;
        public Vector2 eyeRenderTextureResolution;
    }

    public static class PXR_Plugin
    {
        private const string PxrSDKVersion = "1.2.3.5";

        private static AndroidJavaClass sysClass, homeKeyClass, audioClass, batteryClass, controllerClass0, controllerClass1, controllerClass2, pxrClass0, pxrClass1, pxrClass2, unityPlayer;
        private static AndroidJavaObject activity;

        [DllImport("UnityPicoVR")]
        public static extern void UnityPicoVR_SetFoveationLevel(int level);
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_GetFoveationLevel();
        [DllImport("UnityPicoVR")]
        public static extern void UnityPicoVR_SetFoveationParamets(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_GetStencilMesh(int eye, ref int vertexCount, ref int triangleCount, ref IntPtr vertexData, ref IntPtr indexData);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pvr_EnableFoveation(bool enable);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Pvr_GetIntSysProc(string property, ref int res);

        //PassThrough
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_start();
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_stop();
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_destroy();
        [DllImport("UnityPicoVR")]
        public static extern IntPtr UnityPicoVR_camera_getRenderEventFunc();
        [DllImport("UnityPicoVR")]
        public static extern void UnityPicoVR_camera_setRenderEventPending();
        [DllImport("UnityPicoVR")]
        public static extern void UnityPicoVR_camera_waitForRenderEvent();
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_updateFrame(int eye);
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_createTexutresMainThread();
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_deleteTexutresMainThread();
        [DllImport("UnityPicoVR")]
        public static extern int UnityPicoVR_camera_updateTexutresMainThread();

        //Ipd
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_SetIPD(float distance);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern float Pvr_GetIPD();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_SetTrackingIPDEnabled(bool enable);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_GetTrackingIPDEnabled();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_GetEyeTrackingAutoIPD(ref float autoIPD);

        //Sensor
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_OptionalResetSensor(int index, int resetRot, int resetPos);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetPsensorState();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetMainSensorState(ref float x, ref float y, ref float z, ref float w, ref float px, ref float py, ref float pz, ref float fov, ref float fov2, ref int viewNumber);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Pvr_GetTrackingOriginDeltaY();

        //System
        [DllImport("UnityPicoVR")]
        public static extern bool LoadPicoPlugin();
        [DllImport("UnityPicoVR")]
        public static extern void UnloadPicoPlugin();
        [DllImport("UnityPicoVR")]
        public static extern void UnityPicoVR_SetUserDefinedSettings(UserDefinedSettings settings);
        [DllImport("UnityPicoVR")]
        public static extern void UnityPicoVR_Construct(PXR_Loader.ConvertRotationWith2VectorDelegate fromToRotation);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_GetHmdHardwareVersion();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_GetIntConfig(int configsenum, ref int res);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetFloatConfig(int configsenum, ref float res);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Pvr_GetSDKVersion();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PVR_setPerformanceLevels(int cpuLevel, int gpuLevel);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_SetExtraLatencyMode(int mode);

        //Boundary
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern float Pvr_GetFloorHeight();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_GetSeeThroughState();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_GetFrameRateLimit();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_IsBoundaryEnable();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_BoundaryGetConfigured();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_BoundaryTestNode(int node, bool isPlayArea, ref bool isTriggering, ref float closestDistance, ref float px, ref float py, ref float pz, ref float nx, ref float ny, ref float nz);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_BoundaryTestPoint(float x, float y, float z, bool isPlayArea, ref bool isTriggering, ref float closestDistance, ref float px, ref float py, ref float pz, ref float nx, ref float ny, ref float nz);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_BoundaryGetGeometry(out IntPtr handle, bool isPlayArea);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_BoundaryGetDimensions(ref float x, ref float y, ref float z, bool isPlayArea);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_BoundaryGetEnabled();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pvr_BoundarySetVisible(bool value);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_BoundaryGetVisible();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_EnableLWRP(bool enable);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Pvr_SetViewportSize(int w, int h);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pvr_BoundarySetSeeThroughVisible(bool value);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PVR_SetCameraImageRect(int width, int height);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Pvr_CreateLayerAndroidSurface(int layerType, int layerIndex);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pvr_SetOverlayModelViewMatrix(int overlayType, int overlayShape, int texId, int eyeSide, int layerIndex, bool isHeadLocked, int layerFlags,
            float[] mvMatrix, float[] modelS, float[] modelR, float[] modelT, float[] cameraR, float[] cameraT, float[] colorScaleAndOffset);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pvr_SetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags, float[] colorScaleAndOffset);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_SetReinPosition(float x, float y, float z, float w, float px, float py, float pz, int hand, bool valid, int key);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pvr_BoundarySetSTBackground(bool value);
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pvr_GetDialogState();
        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        private static extern float Pvr_GetPredictedDisplayTime();
        [DllImport("UnityPicoVR")]
        private static extern void UnityPicoVR_SetControllerOriginOffset(int controllerID, Vector3 offset);

        public static void UPxr_InitAndroidClass()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            sysClass = new AndroidJavaClass("com.psmart.aosoperation.SysActivity");
            homeKeyClass = new AndroidJavaClass("com.psmart.vrlib.HomeKeyReceiver");
            audioClass = new AndroidJavaClass("com.psmart.aosoperation.AudioReceiver");
            batteryClass = new AndroidJavaClass("com.psmart.aosoperation.BatteryReceiver");
            controllerClass0 = new AndroidJavaClass("com.picovr.picovrlib.hummingbirdclient.HbClientActivity");
            controllerClass1 = new AndroidJavaClass("com.picovr.picovrlib.cvcontrollerclient.ControllerClient");
            controllerClass2 = new AndroidJavaClass("com.picovr.picovrlib.hummingbirdclient.HbClientReceiver");
            pxrClass0 = new AndroidJavaClass("com.psmart.vrlib.PvrClient");
            pxrClass1 = new AndroidJavaClass("com.psmart.vrlib.PicovrSDK");
            pxrClass2 = new AndroidJavaClass("com.psmart.vrlib.VrActivity");
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        }

        public static class Render
        {
            public static void UPxr_EnableFoveation(bool enable)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                if (PXR_Manager.Instance.systemFFRLevel != -1 || PXR_Manager.Instance.systemDebugFFRLevel != -1)
                {
                    return;
                }
                Pvr_EnableFoveation(enable);
#endif
            }

            public static void UPxr_SetFoveationLevel(FoveationLevel level)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                if (PXR_Manager.Instance.systemDebugFFRLevel != -1)
                {
                    UnityPicoVR_SetFoveationLevel(PXR_Manager.Instance.systemDebugFFRLevel);
                    return;
                }
                if (PXR_Manager.Instance.systemFFRLevel != -1)
                {
                    UnityPicoVR_SetFoveationLevel(PXR_Manager.Instance.systemFFRLevel);
                    return;
                }
                UnityPicoVR_SetFoveationLevel((int)level);
#endif
            }

            public static FoveationLevel UPxr_GetFoveationLevel()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                int ret = UnityPicoVR_GetFoveationLevel();
                return (FoveationLevel)ret;
#else
                return FoveationLevel.None;
#endif
            }

            public static void UPxr_SetFoveationParameters(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                if (PXR_Manager.Instance.systemFFRLevel!= -1 || PXR_Manager.Instance.systemDebugFFRLevel !=-1)
                {
                    return;
                }
                UnityPicoVR_SetFoveationParamets(foveationGainX, foveationGainY, foveationArea, foveationMinimum);
#endif
            }

            public static void UPxr_GetStencilMesh(int eye, ref int vertexCount, ref int triangleCount, ref IntPtr vertexDataPtr, ref IntPtr indexDataPtr)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pvr_GetStencilMesh(eye, ref vertexCount, ref triangleCount, ref vertexDataPtr, ref indexDataPtr);
#endif
            }

            public static bool UPxr_SetIPD(float distance)
            {
                var state = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = Pvr_SetIPD(distance);
                if (state)
                {
                    Camera.main.stereoSeparation = distance;
                }
#endif
                return state;
            }

            public static float UPxr_GetIPD()
            {
                var ipd = 0.0f;
#if UNITY_ANDROID && !UNITY_EDITOR
                ipd = Pvr_GetIPD();
#endif
                return ipd;
            }

            public static bool UPxr_SetTrackingIPDEnabled(bool enable)
            {
                var state = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = Pvr_SetTrackingIPDEnabled(enable);
#endif
                return state;
            }

            public static bool UPxr_GetTrackingIPDEnabled()
            {
                var state = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = Pvr_GetTrackingIPDEnabled();
#endif
                return state;
            }

            public static bool UPxr_GetEyeTrackingAutoIPD(ref float autoipd)
            {
                var state = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = Pvr_GetEyeTrackingAutoIPD(ref autoipd);
#endif
                return state;
            }

            public static void UPxr_EnablePresentation(bool enable)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                System.UPxr_CallStaticMethod(pxrClass1, "enablePresentation", activity , enable);
#endif
            }

            public static bool UPxr_GetIntSysProc(string property, ref int res)
            {
                bool reslut = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                reslut = Pvr_GetIntSysProc(property,ref res);
#endif
                return reslut;
            }
        }

        public static class Sensor
        {
            public static int UPxr_OptionalResetSensor(int resetRot, int resetPos)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Pvr_OptionalResetSensor(0, resetRot, resetPos);
#else
                return 0;
#endif
            }

            public static void UPxr_InitPSensor()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "initPsensor", activity);
#endif
            }

            public static int UPxr_GetPSensorState()
            {
                int state = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref state, sysClass, "getPsensorState");
                if (state != 0 && state != -1)
                {
                    state = 1;
                }
#endif
                return state;
            }

            public static int UPxr_GetMainSensorState(ref float x, ref float y, ref float z, ref float w, ref float px, ref float py, ref float pz, ref float vfov, ref float hfov, ref int viewNumber)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Pvr_GetMainSensorState(ref x,ref y,ref z,ref w,ref px,ref py,ref pz,ref vfov,ref hfov,ref viewNumber);
#else
                return 0;
#endif
            }

        }

        public static class System
        {
            public static bool UPxr_LoadPicoPlugin()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return LoadPicoPlugin();
#else  
                return false;
#endif
            }

            public static void UPxr_UnloadPicoPlugin()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UnloadPicoPlugin();
#endif
            }

            public static void UPxr_SetUserDefinedSettings(UserDefinedSettings settings)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UnityPicoVR_SetUserDefinedSettings(settings);
#endif
            }

            public static void UPxr_Construct(PXR_Loader.ConvertRotationWith2VectorDelegate fromToRotation)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UnityPicoVR_Construct(fromToRotation);
#endif
            }

            public static int UPxr_GetHmdHardwareVersion()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Pvr_GetHmdHardwareVersion();
#else
                return 0;
#endif
            }

            public static int UPxr_GetIntConfig(int config, ref int res)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Pvr_GetIntConfig(config, ref res);
#else
                return 0;
#endif
            }

            public static int UPxr_GetFloatConfig(int configsenum, ref float res)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Pvr_GetFloatConfig(configsenum, ref res);
#else
                return 0;
#endif
            }

            public static string UPxr_GetSDKVersion()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                IntPtr ptr = Pvr_GetSDKVersion();
                if (ptr != IntPtr.Zero)
                {
                    return Marshal.PtrToStringAnsi(ptr);
                }
#endif
                return "";
            }

            public static string UPxr_GetUnitySDKVersion()
            {
                return PxrSDKVersion;
            }

            public static void UPxr_StartReceiver()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod(controllerClass2, "startReceiver",activity,"");
#endif
            }

            public static bool UPxr_CallStaticMethod<T>(ref T result, AndroidJavaClass jclass, string name, params object[] args)
            {
                try
                {
                    result = jclass.CallStatic<T>(name, args);
                    return true;
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogError(e.ToString());
                    return false;
                }
            }

            public static bool UPxr_CallStaticMethod(AndroidJavaObject jobj, string name, params object[] args)
            {
                try
                {
                    jobj.CallStatic(name, args);
                    return true;
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogError(e.ToString());
                    return false;
                }
            }

            public static bool UPxr_CallMethod<T>(ref T result, AndroidJavaObject jobj, string name, params object[] args)
            {
                try
                {
                    result = jobj.Call<T>(name, args);
                    return true;
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogError(e.ToString());
                    return false;
                }
            }

            public static bool UPxr_CallMethod(AndroidJavaObject jobj, string name, params object[] args)
            {
                try
                {
                    jobj.Call(name, args);
                    return true;
                }
                catch (AndroidJavaException e)
                {
                    Debug.LogError(e.ToString());
                    return false;
                }
            }

            public static string UPxr_GetDeviceSN()
            {
                string serialNum = "Unknown";
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<string>(ref serialNum, sysClass, "getDeviceSN");
#endif
                return serialNum;
            }

            public static void UPxr_SetSecure(bool isOpen)
            {
                Debug.Log("PXRLog UPxr_SetSecure" + isOpen);
#if UNITY_ANDROID && !UNITY_EDITOR
                UPxr_CallStaticMethod(pxrClass2, "SetSecure", activity, isOpen);
#endif
            }

            public static int UPxr_GetColorRes(string name)
            {
                int value = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<int>(ref value, pxrClass2, "getColorRes", name);
#endif
                return value;
            }

            public static int UPxr_GetConfigInt(string name)
            {
                int value = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<int>(ref value, pxrClass2, "getConfigInt", name);
#endif
                return value;
            }

            public static string UPxr_GetConfigString(string name)
            {
                string value = "";
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<string>(ref value, pxrClass2, "getConfigString", name);
#endif
                return value;
            }

            public static string UPxr_GetDrawableLocation(string name)
            {
                string value = "";
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<string>(ref value, pxrClass2, "getDrawableLocation", name);
#endif
                return value;
            }

            public static int UPxr_GetTextSize(string name)
            {
                int value = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<int>(ref value, pxrClass2, "getTextSize", name);
#endif
                return value;
            }

            public static string UPxr_GetLangString(string name)
            {
                string value = "";
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<string>(ref value, pxrClass2, "getLangString", name);
#endif
                return value;
            }

            public static string UPxr_GetStringValue(string id, int type)
            {
                string value = "";
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<string>(ref value, pxrClass2, "getStringValue", id, type);
#endif
                return value;
            }

            public static int UPxr_GetIntValue(string id, int type)
            {
                int value = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<int>(ref value, pxrClass2, "getIntValue", id, type);
#endif
                return value;
            }

            public static float UPxr_GetFloatValue(string id)
            {
                float value = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<float>(ref value, pxrClass2, "getFloatValue", id);
#endif
                return value;
            }

            public static string UPxr_GetObjectOrArray(string id, int type)
            {
                string value = "";
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<string>(ref value, pxrClass2, "getObjectOrArray", id, type);
#endif
                return value;
            }

            public static int UPxr_GetCharSpace(string id)
            {
                int value = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<int>(ref value, pxrClass2, "getCharSpace", id);
#endif
                return value;
            }

            public static void UPxr_InitKeyEventManager()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod(pxrClass2, "initKeyEventManager", activity);
#endif
            }

            public static bool UPxr_Check6DofAppResume()
            {
                bool state = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod<bool>(ref state, pxrClass2, "check6DofAppResume");
#endif
                return state;
            }

            public static bool UPxr_InitBatteryVolClass()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "Pvr_InitAudioDevice", activity);
#endif
                return true;
            }

            public static bool UPxr_StartBatteryReceiver(string objectName)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(batteryClass, "Pvr_StartReceiver", activity, objectName);
#endif
                return true;
            }

            public static bool UPxr_StopBatteryReceiver()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(batteryClass, "Pvr_StopReceiver", activity);
#endif
                return true;
            }

            public static bool UPxr_SetBrightness(int brightness)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "Pvr_SetScreen_Brightness", brightness, activity);
#endif
                return true;
            }

            public static int UPxr_GetCurrentBrightness()
            {
                int currentLight = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref currentLight, sysClass, "Pvr_GetScreen_Brightness", activity);
#endif
                return currentLight;
            }

            public static int[] UPxr_GetScreenBrightnessLevel()
            {
                int[] currentLight = { 0 };
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int[]>(ref currentLight, sysClass, "getScreenBrightnessLevel");
#endif
                return currentLight;
            }

            public static void UPxr_SetScreenBrightnessLevel(int vrBrightness, int level)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "setScreenBrightnessLevel", vrBrightness, level);
#endif
            }

            public static bool UPxr_StartAudioReceiver(string objectName)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(audioClass, "Pvr_StartReceiver", activity, objectName);
#endif
                return true;
            }

            public static bool UPxr_StopAudioReceiver()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(audioClass, "Pvr_StopReceiver", activity);
#endif
                return true;
            }

            public static int UPxr_GetMaxVolumeNumber()
            {
                int value = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref value, sysClass, "Pvr_GetMaxAudionumber");
#endif
                return value;
            }

            public static int UPxr_GetCurrentVolumeNumber()
            {
                int value = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref value, sysClass, "Pvr_GetAudionumber");
#endif
                return value;
            }

            public static bool UPxr_VolumeUp()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "Pvr_UpAudio");
#endif
                return true;
            }

            public static bool UPxr_VolumeDown()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "Pvr_DownAudio");
#endif
                return true;
            }

            public static bool UPxr_SetVolumeNum(int volume)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(sysClass, "Pvr_ChangeAudio", volume);
#endif
                return true;
            }

            public static bool UPxr_StartHomeKeyReceiver(string objeceName)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod(homeKeyClass, "Pvr_StartReceiver", activity, objeceName);
#endif
                return true;
            }

            public static bool UPxr_StopHomeKeyReceiver()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UPxr_CallStaticMethod(homeKeyClass, "Pvr_StopReceiver", activity);
#endif
                return true;
            }

            public static Action<bool> BoolCallback;
            public static Action<int> IntCallback;
            public static Action<long> LongCallback;

#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaObject tobHelper;
            private static AndroidJavaClass tobHelperClass;
#endif

            public static void UPxr_InitToBService()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelperClass = new AndroidJavaClass("com.pvr.tobservice.ToBServiceHelper");
                tobHelper = tobHelperClass.CallStatic<AndroidJavaObject>("getInstance");
#endif
            }

            public static void UPxr_SetUnityObjectName(string obj)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("setUnityObjectName", obj);
#endif
            }

            public static void UPxr_BindSystemService()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("bindTobService", activity);
#endif
            }

            public static void UPxr_UnBindSystemService()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("unBindTobService", activity);
#endif
            }

            private static AndroidJavaObject GetEnumType(Enum enumType)
            {
                AndroidJavaClass enumjs = new AndroidJavaClass("com.pvr.tobservice.enums" + enumType.GetType().ToString().Replace("Unity.XR.PXR.", ".PBS_"));
                AndroidJavaObject enumjo = enumjs.GetStatic<AndroidJavaObject>(enumType.ToString());
                return enumjo;
            }

            public static string UPxr_StateGetDeviceInfo(SystemInfoEnum type)
            {
                string result = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                result = tobHelper.Call<string>("pbsStateGetDeviceInfo", GetEnumType(type), 0);
#endif
                return result;
            }

            public static void UPxr_ControlSetDeviceAction(DeviceControlEnum deviceControl, Action<int> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) IntCallback = callback;
                tobHelper.Call("pbsControlSetDeviceAction", GetEnumType(deviceControl), null);
#endif
            }

            public static void UPxr_ControlAPPManger(PackageControlEnum packageControl, string path, Action<int> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) IntCallback = callback;
                tobHelper.Call("pbsControlAPPManger", GetEnumType(packageControl), path, 0, null);
#endif
            }

            public static void UPxr_ControlSetAutoConnectWIFI(string ssid, string pwd, Action<bool> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) BoolCallback = callback;
                tobHelper.Call("pbsControlSetAutoConnectWIFI", ssid, pwd, 0, null);
#endif
            }

            public static void UPxr_ControlClearAutoConnectWIFI(Action<bool> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) BoolCallback = callback;
                tobHelper.Call("pbsControlClearAutoConnectWIFI", null);
#endif
            }

            public static void UPxr_PropertySetHomeKey(HomeEventEnum eventEnum, HomeFunctionEnum function, Action<bool> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) BoolCallback = callback;
                tobHelper.Call("pbsPropertySetHomeKey", GetEnumType(eventEnum), GetEnumType(function), null);
#endif
            }

            public static void UPxr_PropertySetHomeKeyAll(HomeEventEnum eventEnum, HomeFunctionEnum function, int timesetup, string pkg, string className, Action<bool> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) BoolCallback = callback;
                tobHelper.Call("pbsPropertySetHomeKeyAll", GetEnumType(eventEnum), GetEnumType(function), timesetup, pkg, className, null);
#endif
            }

            public static void UPxr_PropertyDisablePowerKey(bool isSingleTap, bool enable, Action<int> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) IntCallback = callback;
                tobHelper.Call("pbsPropertyDisablePowerKey", isSingleTap, enable, null);
#endif
            }

            public static void UPxr_PropertySetScreenOffDelay(ScreenOffDelayTimeEnum timeEnum, Action<int> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (callback != null) IntCallback = callback;
                tobHelper.Call("pbsPropertySetScreenOffDelay", GetEnumType(timeEnum), null);
#endif
            }

            public static void UPxr_PropertySetSleepDelay(SleepDelayTimeEnum timeEnum)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("pbsPropertySetSleepDelay", GetEnumType(timeEnum));
#endif
            }

            public static void UPxr_SwitchSystemFunction(SystemFunctionSwitchEnum systemFunction, SwitchEnum switchEnum)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("pbsSwitchSystemFunction", GetEnumType(systemFunction), GetEnumType(switchEnum), 0);
#endif
            }

            public static void UPxr_SwitchSetUsbConfigurationOption(USBConfigModeEnum uSBConfigModeEnum)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("pbsSwitchSetUsbConfigurationOption", GetEnumType(uSBConfigModeEnum), 0);
#endif
            }

            public static void UPxr_ScreenOn()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                tobHelper.Call("pbsScreenOn");
#endif
            }

            public static void UPxr_ScreenOff()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsScreenOff");
#endif
            }

            public static void UPxr_AcquireWakeLock()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsAcquireWakeLock");
#endif
            }

            public static void UPxr_ReleaseWakeLock()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsReleaseWakeLock");
#endif
            }

            public static void UPxr_EnableEnterKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsEnableEnterKey");
#endif
            }

            public static void UPxr_DisableEnterKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsDisableEnterKey");
#endif
            }

            public static void UPxr_EnableVolumeKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsEnableVolumeKey");
#endif
            }

            public static void UPxr_DisableVolumeKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsDisableVolumeKey");
#endif
            }

            public static void UPxr_EnableBackKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsEnableBackKey");
#endif
            }

            public static void UPxr_DisableBackKey()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            tobHelper.Call("pbsDisableBackKey");
#endif
            }

            public static void UPxr_WriteConfigFileToDataLocal(string path, string content, Action<bool> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (callback != null) BoolCallback = callback;
            tobHelper.Call("pbsWriteConfigFileToDataLocal", path, content, null);
#endif
            }

            public static void UPxr_ResetAllKeyToDefault(Action<bool> callback)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (callback != null) BoolCallback = callback;
            tobHelper.Call("pbsResetAllKeyToDefault", null);
#endif
            }

            public static int UPxr_SetPerformanceLevels(int cpuLevel, int gpuLevel)
            {
                int result = -1;
#if ANDROID_DEVICE
            result = PVR_setPerformanceLevels(cpuLevel, gpuLevel);
#endif
                return result;
            }

            public static bool UPxr_SetExtraLatencyMode(ExtraLatencyMode mode)
            {
                bool result = false;
#if ANDROID_DEVICE
                result = Pvr_SetExtraLatencyMode((int)mode);
#endif
                return result;
            }

            public static float UPxr_GetPredictedDisplayTime()
            {
                float time = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                try
                {
                    time = Pvr_GetPredictedDisplayTime();
                }
                catch (Exception e)
                {
                    Debug.Log("UPxr_GetPredictedDisplayTime :" + e.ToString());
                }
#endif
                return time;
            }
        }

        public static class Boundary
        {
            public static void UPxr_SetReinPosition(float x, float y, float z, float w, float px, float py, float pz, int hand, bool valid, int key)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pvr_SetReinPosition(x,y,z,w,px,py,pz,hand,valid,key);
#endif
            }

            public static float UPxr_GetFloorHeight()
            {
                float floorHeight = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                floorHeight = Pvr_GetFloorHeight();
#endif
                return floorHeight;
            }

            public static int UPxr_GetSeeThroughState()
            {
                int state = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                state = Pvr_GetSeeThroughState();
#endif
                return state;
            }

            public static IntPtr UPxr_CreateLayerAndroidSurface(int layerType, int layerIndex)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return Pvr_CreateLayerAndroidSurface(layerType, layerIndex);
#else
                return IntPtr.Zero;
#endif
            }

            public static void UPxr_SetOverlayModelViewMatrix(int overlayType, int overlayShape, int texId, int eyeSide, int layerIndex, bool isHeadLocked, int layerFlags, Matrix4x4 mvMatrix, Vector3 modelS, Quaternion modelR, Vector3 modelT, Quaternion cameraR, Vector3 cameraT, Vector4 colorScale, Vector4 colorOffset)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                float[] mvMat = new float[16];
                mvMat[0] = mvMatrix.m00; mvMat[1] = mvMatrix.m01; mvMat[2] = mvMatrix.m02; mvMat[3] = mvMatrix.m03;
                mvMat[4] = mvMatrix.m10; mvMat[5] = mvMatrix.m11; mvMat[6] = mvMatrix.m12; mvMat[7] = mvMatrix.m13;
                mvMat[8] = mvMatrix.m20; mvMat[9] = mvMatrix.m21; mvMat[10] = mvMatrix.m22; mvMat[11] = mvMatrix.m23;
                mvMat[12] = mvMatrix.m30; mvMat[13] = mvMatrix.m31; mvMat[14] = mvMatrix.m32; mvMat[15] = mvMatrix.m33;
                float[] scaleM = new float[3];
                scaleM[0] = modelS.x; scaleM[1] = modelS.y; scaleM[2] = modelS.z;

                float[] rotationM = new float[4];
                rotationM[0] = modelR.x; rotationM[1] = modelR.y; rotationM[2] = modelR.z; rotationM[3] = modelR.w;

                float[] translationM = new float[3];
                translationM[0] = modelT.x; translationM[1] = modelT.y; translationM[2] = modelT.z;

                float[] rotationC = new float[4];
                rotationC[0] = cameraR.x; rotationC[1] = cameraR.y; rotationC[2] = cameraR.z; rotationC[3] = cameraR.w;

                float[] translationC = new float[3];
                translationC[0] = cameraT.x; translationC[1] = cameraT.y; translationC[2] = cameraT.z;

                float[] colorScaleAndOffset = new float[8];
                colorScaleAndOffset[0] = colorScale.x;
                colorScaleAndOffset[1] = colorScale.y;
                colorScaleAndOffset[2] = colorScale.z;
                colorScaleAndOffset[3] = colorScale.w;

                colorScaleAndOffset[4] = colorOffset.x;
                colorScaleAndOffset[5] = colorOffset.y;
                colorScaleAndOffset[6] = colorOffset.z;
                colorScaleAndOffset[7] = colorOffset.w;

                Pvr_SetOverlayModelViewMatrix(overlayType, overlayShape, texId, eyeSide, layerIndex, isHeadLocked, layerFlags, mvMat, scaleM, rotationM, translationM, rotationC, translationC, colorScaleAndOffset);
#endif
            }

            public static void UPxr_SetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags, Vector4 colorScale, Vector4 colorOffset)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                float[] colorScaleAndOffset = new float[8];
                colorScaleAndOffset[0] = colorScale.x;
                colorScaleAndOffset[1] = colorScale.y;
                colorScaleAndOffset[2] = colorScale.z;
                colorScaleAndOffset[3] = colorScale.w;

                colorScaleAndOffset[4] = colorOffset.x;
                colorScaleAndOffset[5] = colorOffset.y;
                colorScaleAndOffset[6] = colorOffset.z;
                colorScaleAndOffset[7] = colorOffset.w;

                Pvr_SetupLayerData(layerIndex, sideMask, textureId, textureType, layerFlags, colorScaleAndOffset);
#endif
            }

            public static bool UPxr_GetFrameRateLimit()
            {
                bool ret = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                ret = Pvr_GetFrameRateLimit();
#endif
                return ret;
            }

            public static bool UPxr_IsBoundaryEnable()
            {
                bool state = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                state = Pvr_IsBoundaryEnable();
#endif
                return state;
            }

            public static bool UPxr_GetBoundaryConfigured()
            {
                bool ret = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                ret = Pvr_BoundaryGetConfigured();
#endif
                return ret;
            }

            public static BoundaryTestResult UPxr_TestBoundaryNode(BoundaryTrackingNode node, BoundaryType boundaryType)
            {
                BoundaryTestResult testResult = new BoundaryTestResult();
#if !UNITY_EDITOR && UNITY_ANDROID
                bool ret = Pvr_BoundaryTestNode((int)node, boundaryType == BoundaryType.PlayArea, ref testResult.IsTriggering, ref testResult.ClosestDistance,
                    ref testResult.ClosestPoint.x, ref testResult.ClosestPoint.y, ref testResult.ClosestPoint.z,
                    ref testResult.ClosestPointNormal.x, ref testResult.ClosestPointNormal.y, ref testResult.ClosestPointNormal.z);

                testResult.ClosestPoint.z = -testResult.ClosestPoint.z;
                testResult.ClosestPointNormal.z = -testResult.ClosestPointNormal.z;
                if (!ret)
                {
                    Debug.LogError(string.Format("UPxr_BoundaryTestNode({0}, {1}) API call failed!", node, boundaryType));
                }
#endif
                return testResult;
            }

            public static BoundaryTestResult UPxr_TestBoundaryPoint(Vector3 point, BoundaryType boundaryType)
            {
                BoundaryTestResult testResult = new BoundaryTestResult();
#if !UNITY_EDITOR && UNITY_ANDROID
                bool ret = Pvr_BoundaryTestPoint(point.x, point.y, -point.z, boundaryType == BoundaryType.PlayArea, ref testResult.IsTriggering, ref testResult.ClosestDistance,
                    ref testResult.ClosestPoint.x, ref testResult.ClosestPoint.y, ref testResult.ClosestPoint.z,
                    ref testResult.ClosestPointNormal.x, ref testResult.ClosestPointNormal.y, ref testResult.ClosestPointNormal.z);

                if (!ret)
                {
                    Debug.LogError(string.Format("UPxr_BoundaryTestPoint({0}, {1}) API call failed!", point, boundaryType));
                }
#endif
                return testResult;
            }

            public static Vector3[] UPxr_GetBoundaryGeometry(BoundaryType boundaryType)
            {
                Vector3[] points = new Vector3[1];
#if !UNITY_EDITOR && UNITY_ANDROID
                IntPtr pointHandle = IntPtr.Zero;
                int pointsCount = Pvr_BoundaryGetGeometry(out pointHandle, boundaryType == BoundaryType.PlayArea);
                if (pointsCount <= 0)
                {
                    Debug.LogError("Boundary geometry point count is " + pointsCount);
                    return null;
                }
                // managed buffer
                int pointBufferSize = pointsCount * 3;
                float[] pointsBuffer = new float[pointBufferSize];
                Marshal.Copy(pointHandle, pointsBuffer, 0, pointBufferSize);

                points = new Vector3[pointsCount];
                for (int i = 0; i < pointsCount; i++)
                {
                    points[i] = new Vector3()
                    {
                        x = pointsBuffer[3 * i + 0],
                        y = pointsBuffer[3 * i + 1],
                        z = -pointsBuffer[3 * i + 2],
                    };
                }
#endif
                return points;
            }

            public static Vector3 UPxr_GetBoundaryDimensions(BoundaryType boundaryType)
            {
                float x = 0, y = 0, z = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                Pvr_BoundaryGetDimensions(ref x, ref y, ref z, boundaryType == BoundaryType.PlayArea);
#endif
                return new Vector3(x, y, z);
            }

            public static bool UPxr_GetBoundaryEnabled()
            {
                bool ret = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                ret = Pvr_BoundaryGetEnabled();
#endif
                return ret;
            }

            public static void UPxr_SetBoundaryVisible(bool value)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pvr_BoundarySetVisible(value);
#endif
            }

            public static bool UPxr_GetBoundaryVisible()
            {
                var state = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                state = Pvr_BoundaryGetVisible();
#endif
                return state;
            }

            public static bool UPxr_EnableLWRP(bool enable)
            {
                var state = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                state = Pvr_EnableLWRP(enable);
#endif
                return state;
            }

            public static bool UPxr_SetViewportSize(int width, int height)
            {
                var state = false;
#if !UNITY_EDITOR && UNITY_ANDROID
                state = Pvr_SetViewportSize(width, height);
#endif
                return state;
            }

            public static void UPxr_EnableSeeThroughManual(bool enable)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                Pvr_BoundarySetSTBackground(enable);
#endif
            }

            //Struct to IntPtr
            public static IntPtr UPxr_StructToIntPtr<T>(T info)
            {
                int size = Marshal.SizeOf(info);
                IntPtr intPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(info, intPtr, true);
                return intPtr;
            }

            //IntPtr to Struct
            public static T UPxr_IntPtrToStruct<T>(IntPtr info)
            {
                return (T)Marshal.PtrToStructure(info, typeof(T));
            }

            public static int UPxr_GetDialogState()
            {
                var state = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                try
                {
                    state = Pvr_GetDialogState();
                }
                catch (Exception e)
                {
                    Debug.Log("UPxr_GetDialogStateError :" + e.ToString());
                }
#endif
                return state;
            }
        }

        public static class PlatformSetting
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            private static AndroidJavaClass verifyTool = new AndroidJavaClass("com.psmart.vrlib.VerifyTool");
            private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            private static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif
            public static PXR_PlatformSetting.simulationType UPxr_IsCurrentDeviceValid()
            {
                if (PXR_PlatformSetting.Instance.entitlementCheckSimulation)
                {
                    if (PXR_PlatformSetting.Instance.deviceSN.Count <= 0)
                    {
                        return PXR_PlatformSetting.simulationType.Null;
                    }
                    else
                    {
                        foreach (var t in PXR_PlatformSetting.Instance.deviceSN)
                        {
                            if (System.UPxr_GetDeviceSN() == t)
                            {
                                return PXR_PlatformSetting.simulationType.Valid;
                            }
                        }

                        return PXR_PlatformSetting.simulationType.Invalid;
                    }
                }
                else
                {
                    return PXR_PlatformSetting.simulationType.Invalid;
                }
            }

            public static void UPxr_BindVerifyService(string gameobjectname)
            {
                Debug.Log("PXRLog UPxr_BindVerifyService");
                AndroidJavaObject VerifyTool = new AndroidJavaObject("com.psmart.vrlib.VerifyTool");
#if UNITY_ANDROID && !UNITY_EDITOR
                VerifyTool.Call<bool>("bindVerifyService", currentActivity, gameobjectname);
#endif
            }

            public static bool UPxr_AppEntitlementCheck(string appid)
            {
                bool state = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = verifyTool.CallStatic<bool>("verifyAPP", currentActivity, appid, "");
#endif
                return state;
            }

            public static bool UPxr_KeyEntitlementCheck(string publicKey)
            {
                bool state = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = verifyTool.CallStatic<bool>("verifyAPP", currentActivity, "", publicKey);
#endif
                return state;
            }

            //0:success -1:invalid params -2:service not exist -3:time out
            public static int UPxr_AppEntitlementCheckExtra(string appid)
            {
                int state = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = verifyTool.CallStatic<int>("verifyAPPExt", currentActivity, appid, "");
#endif
                return state;
            }

            //0:success -1:invalid params -2:service not exist -3:time out
            public static int UPxr_KeyEntitlementCheckExtra(string publicKey)
            {
                int state = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
                state = verifyTool.CallStatic<int>("verifyAPPExt", currentActivity, "", publicKey);
#endif
                return state;
            }
        }

        public static class Controller
        {
            public static void UPxr_SetUnityVersionToJar(string version)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(controllerClass1, "setUnityVersion", version);
#endif
            }

            public static int UPxr_GetMainController()
            {
                int index = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref index, controllerClass1, "getMainControllerIndex");
#endif
                return index;
            }

            public static void UPxr_SetMainController(int index)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(controllerClass1, "setMainController", index);
#endif
            }

            public static int UPxr_GetControllerConnectionState(int num)
            {
                int state = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref state, controllerClass1, "getControllerConnectionState", num);
#endif
                return state;
            }

            public static void UPxr_VibrateController(float strength, int time, int hand)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(controllerClass1, "vibrateCV2ControllerStrength", strength, time, hand);
#endif
            }

            public static int UPxr_GetDeviceType()
            {
                int type = -1;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod<int>(ref type, controllerClass0, "getDeviceType");
#endif
                return type;
            }

            public static int UPxr_GetControllerType()
            {
                int type = -1;
#if ANDROID_DEVICE
                System.UPxr_CallStaticMethod<int>(ref type, controllerClass0, "getControllerType");
#endif
                return type;
            }


            private static float[] fixedState = new float[7] { 0, 0, 0, 1, 0, 0, 0 };
            public static float[] UPxr_GetControllerFixedSensorState(int hand)
            {
                var data = fixedState;
#if !UNITY_EDITOR && UNITY_ANDROID
                System.UPxr_CallStaticMethod(ref data, controllerClass1, "getControllerFixedSensorState", hand);
#endif
                return data;
            }

            private static float[] predictData = new float[7];

            public static float[] GetControllerPredictSensorData(int controllerID, float predictTime)
            {
                float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
#if ANDROID_DEVICE
                System.UPxr_CallStaticMethod(controllerClass1, "SetHeadDataAndPreTime",headData, predictTime);
                System.UPxr_CallStaticMethod(ref predictData, controllerClass1, "getControllerSensorStateWithHeadDataAndPreTime", controllerID);
#endif

                var offset = Pvr_GetTrackingOriginDeltaY();
                predictData[4] = predictData[4] / 1000.0f;
                predictData[5] = predictData[5] / 1000.0f + offset;
                predictData[6] = -predictData[6] / 1000.0f;
                return predictData;
            }

            public static void SetControllerOriginOffset(int controllerID, Vector3 offset)
            {
#if ANDROID_DEVICE
                UnityPicoVR_SetControllerOriginOffset(controllerID, offset);
#endif
            }
        }

        public static class PassThrough
        {
            public static int UPxr_PassThroughStart()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_start();
#else
                return 0;
#endif
            }

            public static int UPxr_PassThroughStop()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_stop();
#else
                return 0;
#endif
            }

            public static int UPxr_PassThroughDestroy()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_destroy();
#else
                return 0;
#endif
            }

            public static IntPtr UPxr_PassThroughGetRenderEventFunc()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_getRenderEventFunc();
#else
                return IntPtr.Zero;
#endif
            }

            public static void UPxr_PassThroughSetRenderEventPending()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UnityPicoVR_camera_setRenderEventPending();
#endif
            }

            public static void UPxr_PassThroughWaitForRenderEvent()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                UnityPicoVR_camera_waitForRenderEvent();
#endif
            }

            public static int UPxr_PassThroughUpdateFrame(int eye)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_updateFrame(eye);
#else
                return 0;
#endif
            }

            public static int UPxr_PassThroughCreateTexutresMainThread()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_createTexutresMainThread();
#else
                return 0;
#endif
            }

            public static int UPxr_PassThroughDeleteTexutresMainThread()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_deleteTexutresMainThread();
#else
                return 0;
#endif
            }

            public static int UPxr_PassThroughUpdateTexutresMainThread()
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                return UnityPicoVR_camera_updateTexutresMainThread();
#else
                return 0;
#endif
            }
        }
    }

}