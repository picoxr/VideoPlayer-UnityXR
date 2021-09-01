/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_EyeTracking
    {
        public static bool GetHeadPosMatrix(out Matrix4x4 matrix)
        {
            matrix = Matrix4x4.identity;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            Vector3 headPos = Vector3.zero;
            if (!device.TryGetFeatureValue(CommonUsages.devicePosition, out headPos))
            {
                Debug.Log("Failed at GetHeadPosMatrix 0");
                return false;
            }

            Quaternion headRot = Quaternion.identity;
            if (!device.TryGetFeatureValue(CommonUsages.deviceRotation, out headRot))
            {
                Debug.Log("Failed at GetHeadPosMatrix 1");
                return false;
            }

            matrix = Matrix4x4.TRS(headPos, headRot, Vector3.one);
            return true;
        }

        static bool GetEyeTrackingDevice(out InputDevice device)
        {
            device = default(InputDevice);

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count == 0)
            {
                Debug.Log("Failed at GetEyeTrackingDevice 0");
                return false;
            }
            device = devices[0];
            if(!device.isValid)
                Debug.Log("Failed at GetEyeTrackingDevice 1");
            return device.isValid;
        }

        public static bool GetCombineEyeGazePoint(out Vector3 point)
        {
            point = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combineEyePoint, out point))
            {
                Debug.Log("Failed at GetCombineEyeGazePoint 0");
                return false;
            }

            return true;
        }
        public static bool GetCombineEyeGazeVector(out Vector3 vector)
        {
            vector = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combineEyeVector, out vector))
            {
                Debug.Log("Failed at GetCombineEyeGazeVector 0");
                return false;
            }

            return true;
        }
        public static bool GetLeftEyeGazePoint(out Vector3 point)
        {
            point = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePoint, out point))
            {
                return false;
            }

            return true;
        }
        public static bool GetLeftEyeGazeVector(out Vector3 vector)
        {
            vector = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyeVector, out vector))
            {
                return false;
            }

            return true;
        }
        public static bool GetRightEyeGazePoint(out Vector3 point)
        {
            point = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePoint, out point))
            {
                return false;
            }

            return true;
        }
        public static bool GetRightEyeGazeVector(out Vector3 vector)
        {
            vector = Vector3.zero;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyeVector, out vector))
            {
                return false;
            }

            return true;
        }
        public static bool GetLeftEyeGazeOpenness(out float openness)
        {
            openness = 0;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyeOpenness, out openness))
            {
                return false;
            }

            return true;
        }

        public static bool GetRightEyeGazeOpenness(out float openness)
        {
            openness = 0;

            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyeOpenness, out openness))
            {
                return false;
            }

            return true;
        }

        public static bool GetLeftEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePoseStatus, out status))
            {
                return false;
            }
            return true;
        }

        public static bool GetRightEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePoseStatus, out status))
            {
                return false;
            }
            return true;
        }

        public static bool GetCombinedEyePoseStatus(out uint status)
        {
            status = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.combinedEyePoseStatus, out status))
            {
                return false;
            }
            return true;
        }

        public static bool GetLeftEyePupilDilation(out float value)
        {
            value = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePupilDilation, out value))
            {
                return false;
            }
            return true;
        }

        public static bool GetRightEyePupilDilation(out float value)
        {
            value = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePupilDilation, out value))
            {
                return false;
            }
            return true;
        }

        public static bool GetLeftEyePositionGuide(out Vector3 position)
        {
            position = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.leftEyePositionGuide, out position))
            {
                return false;
            }
            return true;
        }

        public static bool GetRightEyePositionGuide(out Vector3 position)
        {
            position = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.rightEyePositionGuide, out position))
            {
                return false;
            }
            return true;
        }

        public static bool GetFoveatedGazeDirection(out Vector3 direction)
        {
            direction = Vector3.zero;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.foveatedGazeDirection, out direction))
            {
                return false;
            }
            return true;
        }

        public static bool GetFoveatedGazeTrackingState(out uint state)
        {
            state = 0;
            if (!PXR_Manager.Instance.eyeTracking)
                return false;

            InputDevice device;
            if (!GetEyeTrackingDevice(out device))
                return false;

            if (!device.TryGetFeatureValue(PXR_Usages.foveatedGazeTrackingState, out state))
            {
                return false;
            }
            return true;
        }

    }
}