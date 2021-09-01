/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public static class PXR_Input
    {
        public enum ControllerDevice
        {
            G2 = 3,
            Neo2,
            Neo3,
            NewController = 10
        }

        public enum Controller
        {
            LeftController,
            RightController,
        }

        /// <summary>
        /// Get the current master control controller
        /// </summary>
        /// <returns></returns>
        public static Controller GetDominantHand()
        {
            return (Controller)PXR_Plugin.Controller.UPxr_GetMainController();
        }

        /// <summary>
        /// Set the current master control controller
        /// </summary>
        public static void SetDominantHand(Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetMainController((int)controller);
        }

        /// <summary>
        /// Set the controller vibrate 
        /// </summary>
        public static void SetControllerVibration(float strength, int time, Controller controller)
        {
            PXR_Plugin.Controller.UPxr_VibrateController(strength, time, (int)controller);
        }

        /// <summary>
        /// Get the controller device
        /// </summary>
        /// <returns></returns>
        public static ControllerDevice GetActiveController()
        {
            return (ControllerDevice)PXR_Plugin.Controller.UPxr_GetControllerType();
        }

        /// <summary>
        /// Get the connection status of Controller
        /// </summary>
        public static bool IsControllerConnected(Controller controller)
        {
            var state = false;
            switch (controller)
            {
                case Controller.LeftController:
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
                case Controller.RightController:
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
            }
            return state;
        }

        /// <summary>
        /// Get the controller predict rotation data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Quaternion UPxr_GetControllerPredictRotation(Controller controller, float predictTime)
        {
            var data = PXR_Plugin.Controller.GetControllerPredictSensorData((int)controller, predictTime);
            return new Quaternion(data[0], data[1], data[2], data[3]);
        }

        /// <summary>
        /// Get the controller predict position data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Vector3 UPxr_GetControllerPredictPosition(Controller controller, float predictTime)
        {
            var data = PXR_Plugin.Controller.GetControllerPredictSensorData((int)controller, predictTime);
            return new Vector3(data[4], data[5], data[6]);
        }

        /// <summary>
        /// Get the controller predict rotation data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Quaternion GetControllerPredictRotation(Controller controller, float predictTime)
        {
            var data = PXR_Plugin.Controller.GetControllerPredictSensorData((int)controller, predictTime);
            return new Quaternion(data[0], data[1], data[2], data[3]);
        }

        /// <summary>
        /// Get the controller predict position data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Vector3 GetControllerPredictPosition(Controller controller, float predictTime)
        {
            var data = PXR_Plugin.Controller.GetControllerPredictSensorData((int)controller, predictTime);
            return new Vector3(data[4], data[5], data[6]);
        }

        /// <summary>
        /// Set the controller origin offset data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="offset">m</param>
        public static void SetControllerOriginOffset(Controller controller, Vector3 offset)
        {
            PXR_Plugin.Controller.SetControllerOriginOffset((int)controller, offset);
        }
    }
}

