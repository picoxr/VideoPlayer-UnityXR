/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_System
    {
        /// <summary>
        /// Turn on power service
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static bool StartBatteryReceiver(string objName)
        {
            return PXR_Plugin.System.UPxr_StartBatteryReceiver(objName);
        }

        /// <summary>
        /// Turn off power service
        /// </summary>
        /// <returns></returns>
        public static bool StopBatteryReceiver()
        {
            return PXR_Plugin.System.UPxr_StopBatteryReceiver();
        }

        /// <summary>
        /// Set the brightness value of the current general device
        /// </summary>
        /// <param name="brightness">brightness value range is 0-255</param>
        /// <returns></returns>
        public static bool SetCommonBrightness(int brightness)
        {
            return PXR_Plugin.System.UPxr_SetBrightness(brightness);
        }

        /// <summary>
        /// Get the brightness value of the current general device
        /// </summary>
        /// <returns>brightness value range: 0-255</returns>
        public static int GetCommonBrightness()
        {
            return PXR_Plugin.System.UPxr_GetCurrentBrightness();
        }

        /// <summary>
        /// Gets the brightness level of the current screen
        /// </summary>
        /// <returns>int array. The first bit is the total brightness level supported, the second bit is the current brightness level, and it is the interval value of the brightness level from the third bit to the end bit</returns>
        public static int[] GetScreenBrightnessLevel()
        {
            int[] currentLight = { 0 };
            currentLight = PXR_Plugin.System.UPxr_GetScreenBrightnessLevel();
            return currentLight;
        }

        /// <summary>
        /// Set the brightness of the screen
        /// </summary>
        /// <param name="brightness">Brightness mode</param>
        /// <param name="level">Brightness value (brightness level value). If brightness passes in 1, level passes in brightness level; if brightness passes in 0, it means that the system default brightness setting mode is adopted. Level can be set to a value between 1 and 255.</param>
        public static void SetScreenBrightnessLevel(int brightness, int level)
        {
            PXR_Plugin.System.UPxr_SetScreenBrightnessLevel(brightness, level);
        }

        /// <summary>
        /// Init volume device
        /// </summary>
        /// <returns></returns>
        public static bool InitAudioDevice()
        {
            return PXR_Plugin.System.UPxr_InitBatteryVolClass();
        }

        /// <summary>
        /// Turn on volume service
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static bool StartAudioReceiver(string objName)
        {
            return PXR_Plugin.System.UPxr_StartAudioReceiver(objName);
        }

        /// <summary>
        /// Turn off volume service
        /// </summary>
        /// <returns></returns>
        public static bool StopAudioReceiver()
        {
            return PXR_Plugin.System.UPxr_StopAudioReceiver();
        }

        /// <summary>
        /// Get maximum volume
        /// </summary>
        /// <returns></returns>
        public static int GetMaxVolumeNumber()
        {
            return PXR_Plugin.System.UPxr_GetMaxVolumeNumber();
        }

        /// <summary>
        /// Get the current volume
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentVolumeNumber()
        {
            return PXR_Plugin.System.UPxr_GetCurrentVolumeNumber();
        }

        /// <summary>
        /// Increase volume
        /// </summary>
        /// <returns></returns>
        public static bool VolumeUp()
        {
            return PXR_Plugin.System.UPxr_VolumeUp();
        }

        /// <summary>
        /// Decrease volume
        /// </summary>
        /// <returns></returns>
        public static bool VolumeDown()
        {
            return PXR_Plugin.System.UPxr_VolumeDown();
        }

        /// <summary>
        /// Set volume
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static bool SetVolumeNum(int volume)
        {
            return PXR_Plugin.System.UPxr_SetVolumeNum(volume);
        }

        /// <summary>
        /// Judging whether current device’s permission is valid
        /// </summary>
        /// <returns></returns>
        public static PXR_PlatformSetting.simulationType IsCurrentDeviceValid()
        {
            return PXR_Plugin.PlatformSetting.UPxr_IsCurrentDeviceValid();
        }

        /// <summary>
        /// Use appid to get result whether entitlement required by app is present
        /// </summary>
        /// <param name="appid"></param>
        /// <returns>value: True: Succes; False: Fail</returns>
        public static bool AppEntitlementCheck(string appid)
        {
            return PXR_Plugin.PlatformSetting.UPxr_AppEntitlementCheck(appid);
        }

        /// <summary>
        /// Use publicKey to get error code of entitlement check result
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>value: True: Succes; False: Fail</returns>
        public static bool KeyEntitlementCheck(string publicKey)
        {
            return PXR_Plugin.PlatformSetting.UPxr_KeyEntitlementCheck(publicKey);
        }

        /// <summary>
        /// Use appid to get error code of entitlement check result
        /// </summary>
        /// <param name="appid"></param>
        /// <returns>value: 0:success -1:invalid params -2:service not exist (old versions of ROM have no Service. If the application needs to be limited to operating in old versions, this state needs processing) -3:time out</returns>
        public static int AppEntitlementCheckExtra(string appid)
        {
            return PXR_Plugin.PlatformSetting.UPxr_AppEntitlementCheckExtra(appid);
        }

        /// <summary>
        /// Use publicKey to get error code of entitlement check result
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns>value: 0:success -1:invalid params -2:service not exist (old versions of ROM have no Service. If the application needs to be limited to operating in old versions, this state needs processing) -3:time out</returns>
        public static int KeyEntitlementCheckExtra(string publicKey)
        {
            return PXR_Plugin.PlatformSetting.UPxr_KeyEntitlementCheckExtra(publicKey);
        }

        /// <summary>
        /// Get SDK version number
        /// </summary>
        /// <returns></returns>
        public static string GetSDKVersion()
        {
            return PXR_Plugin.System.UPxr_GetSDKVersion();
        }

        /// <summary>
        /// Set Cpu,Gpu Levels
        /// </summary>
        /// <param name="cpuLevel">0~5</param>
        /// <param name="gpuLevel">0~5</param>
        /// <returns></returns>
        public static int SetPerformanceLevels(int cpuLevel, int gpuLevel)
        {
            return PXR_Plugin.System.UPxr_SetPerformanceLevels(cpuLevel, gpuLevel);
        }

        /// <summary>
        /// Set ExtraLatency Mode
        /// </summary>
        /// <param name="mode">default value : ExtraLatencyModeDynamic</param>
        /// <returns></returns>
        public static bool SetExtraLatencyMode(ExtraLatencyMode mode)
        {
            return PXR_Plugin.System.UPxr_SetExtraLatencyMode(mode);
        }

        /// <summary>
        /// Get Predicted DisplayTime
        /// </summary>
        /// <returns></returns>
        public static float GetPredictedDisplayTime()
        {
            return PXR_Plugin.System.UPxr_GetPredictedDisplayTime();
        }

        /// <summary>
        /// Init System Service
        /// </summary>
        /// <param name="objectName">Receive callback object name</param>
        public static void InitSystemService(string objectName)
        {
            PXR_Plugin.System.UPxr_InitToBService();
            PXR_Plugin.System.UPxr_SetUnityObjectName(objectName);
            PXR_Plugin.System.UPxr_InitBatteryVolClass();
        }

        /// <summary>
        /// Bind System Service
        /// </summary>
        public static void BindSystemService()
        {
            PXR_Plugin.System.UPxr_BindSystemService();
        }

        /// <summary>
        /// UnBind System Service
        /// </summary>
        public static void UnBindSystemService()
        {
            PXR_Plugin.System.UPxr_UnBindSystemService();
        }

        /// <summary>
        /// Get Device's Info
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string StateGetDeviceInfo(SystemInfoEnum type)
        {
            return PXR_Plugin.System.UPxr_StateGetDeviceInfo(type);
        }

        /// <summary>
        /// Set Device's Action
        /// </summary>
        /// <param name="deviceControl"></param>
        /// <param name="callback"></param>
        public static void ControlSetDeviceAction(DeviceControlEnum deviceControl, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_ControlSetDeviceAction(deviceControl, callback);
        }

        /// <summary>
        /// APP Manger
        /// </summary>
        /// <param name="packageControl"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public static void ControlAPPManger(PackageControlEnum packageControl, string path, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_ControlAPPManger(packageControl, path, callback);
        }

        /// <summary>
        /// Set Auto Connect WIFI
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="pwd"></param>
        /// <param name="callback"></param>
        public static void ControlSetAutoConnectWIFI(string ssid, string pwd, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ControlSetAutoConnectWIFI(ssid, pwd, callback);
        }

        /// <summary>
        /// Clear Auto Connect WIFI
        /// </summary>
        /// <param name="callback"></param>
        public static void ControlClearAutoConnectWIFI(Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ControlClearAutoConnectWIFI(callback);
        }

        /// <summary>
        /// Set Home Key Event
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="function"></param>
        /// <param name="callback"></param>
        public static void PropertySetHomeKey(HomeEventEnum eventEnum, HomeFunctionEnum function, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_PropertySetHomeKey(eventEnum, function, callback);
        }

        /// <summary>
        /// Set Home Key All Event
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="function"></param>
        /// <param name="timesetup"></param>
        /// <param name="pkg"></param>
        /// <param name="className"></param>
        /// <param name="callback"></param>
        public static void PropertySetHomeKeyAll(HomeEventEnum eventEnum, HomeFunctionEnum function, int timesetup, string pkg, string className, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_PropertySetHomeKeyAll(eventEnum, function, timesetup, pkg, className, callback);
        }

        /// <summary>
        /// Disable Power Key
        /// </summary>
        /// <param name="isSingleTap"></param>
        /// <param name="enable"></param>
        /// <param name="callback"></param>
        public static void PropertyDisablePowerKey(bool isSingleTap, bool enable, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_PropertyDisablePowerKey(isSingleTap, enable, callback);
        }

        /// <summary>
        /// Set ScreenOff Delay Time
        /// </summary>
        /// <param name="timeEnum"></param>
        /// <param name="callback"></param>
        public static void PropertySetScreenOffDelay(ScreenOffDelayTimeEnum timeEnum, Action<int> callback)
        {
            PXR_Plugin.System.UPxr_PropertySetScreenOffDelay(timeEnum, callback);
        }

        /// <summary>
        /// Set SleepDelay Time
        /// </summary>
        /// <param name="timeEnum"></param>
        public static void PropertySetSleepDelay(SleepDelayTimeEnum timeEnum)
        {
            PXR_Plugin.System.UPxr_PropertySetSleepDelay(timeEnum);
        }

        /// <summary>
        /// Switch System Function
        /// </summary>
        /// <param name="systemFunction"></param>
        /// <param name="switchEnum"></param>
        public static void SwitchSystemFunction(SystemFunctionSwitchEnum systemFunction, SwitchEnum switchEnum)
        {
            PXR_Plugin.System.UPxr_SwitchSystemFunction(systemFunction, switchEnum);
        }

        /// <summary>
        /// Set UsbConfiguration Option
        /// </summary>
        /// <param name="uSBConfigModeEnum"></param>
        public static void SwitchSetUsbConfigurationOption(USBConfigModeEnum uSBConfigModeEnum)
        {
            PXR_Plugin.System.UPxr_SwitchSetUsbConfigurationOption(uSBConfigModeEnum);
        }

        /// <summary>
        /// Screen On
        /// </summary>
        public static void ScreenOn()
        {
            PXR_Plugin.System.UPxr_ScreenOn();
        }

        /// <summary>
        /// Screen Off 
        /// </summary>
        public static void ScreenOff()
        {
            PXR_Plugin.System.UPxr_ScreenOff();
        }

        /// <summary>
        /// Acquire WakeLock
        /// </summary>
        public static void AcquireWakeLock()
        {
            PXR_Plugin.System.UPxr_AcquireWakeLock();
        }

        /// <summary>
        /// Release WakeLock
        /// </summary>
        public static void ReleaseWakeLock()
        {
            PXR_Plugin.System.UPxr_ReleaseWakeLock();
        }

        /// <summary>
        /// Enable Enter Key
        /// </summary>
        public static void EnableEnterKey()
        {
            PXR_Plugin.System.UPxr_EnableEnterKey();
        }

        /// <summary>
        /// Disable Enter Key
        /// </summary>
        public static void DisableEnterKey()
        {
            PXR_Plugin.System.UPxr_DisableEnterKey();
        }

        /// <summary>
        /// Enable Volume Key
        /// </summary>
        public static void EnableVolumeKey()
        {
            PXR_Plugin.System.UPxr_EnableVolumeKey();
        }

        /// <summary>
        /// Disable Volume Key
        /// </summary>
        public static void DisableVolumeKey()
        {
            PXR_Plugin.System.UPxr_DisableVolumeKey();
        }

        /// <summary>
        /// Enable Back Key
        /// </summary>
        public static void EnableBackKey()
        {
            PXR_Plugin.System.UPxr_EnableBackKey();
        }

        /// <summary>
        /// Disable Back Key
        /// </summary>
        public static void DisableBackKey()
        {
            PXR_Plugin.System.UPxr_DisableBackKey();
        }

        /// <summary>
        /// Write ConfigFile To DataLocal
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        public static void WriteConfigFileToDataLocal(string path, string content, Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_WriteConfigFileToDataLocal(path, content, callback);
        }

        /// <summary>
        /// Reset All Key To Default
        /// </summary>
        /// <param name="callback"></param>
        public static void ResetAllKeyToDefault(Action<bool> callback)
        {
            PXR_Plugin.System.UPxr_ResetAllKeyToDefault(callback);
        }
    }
}

