/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public static class PXR_Usages
    {
        public static InputFeatureUsage<Vector3> combineEyePoint = new InputFeatureUsage<Vector3>("CombinedEyeGazePoint");
        public static InputFeatureUsage<Vector3> combineEyeVector = new InputFeatureUsage<Vector3>("CombinedEyeGazeVector");
        public static InputFeatureUsage<Vector3> leftEyePoint = new InputFeatureUsage<Vector3>("LeftEyeGazePoint");
        public static InputFeatureUsage<Vector3> leftEyeVector = new InputFeatureUsage<Vector3>("LeftEyeGazeVector");
        public static InputFeatureUsage<Vector3> rightEyePoint = new InputFeatureUsage<Vector3>("RightEyeGazePoint");
        public static InputFeatureUsage<Vector3> rightEyeVector = new InputFeatureUsage<Vector3>("RightEyeGazeVector");
        public static InputFeatureUsage<float> leftEyeOpenness = new InputFeatureUsage<float>("LeftEyeOpenness");
        public static InputFeatureUsage<float> rightEyeOpenness = new InputFeatureUsage<float>("RightEyeOpenness");
        public static InputFeatureUsage<uint> leftEyePoseStatus = new InputFeatureUsage<uint>("LeftEyePoseStatus");
        public static InputFeatureUsage<uint> rightEyePoseStatus = new InputFeatureUsage<uint>("RightEyePoseStatus");
        public static InputFeatureUsage<uint> combinedEyePoseStatus = new InputFeatureUsage<uint>("CombinedEyePoseStatus");
        public static InputFeatureUsage<float> leftEyePupilDilation = new InputFeatureUsage<float>("LeftEyePupilDilation");
        public static InputFeatureUsage<float> rightEyePupilDilation = new InputFeatureUsage<float>("RightEyePupilDilation");
        public static InputFeatureUsage<Vector3> leftEyePositionGuide = new InputFeatureUsage<Vector3>("LeftEyePositionGuide");
        public static InputFeatureUsage<Vector3> rightEyePositionGuide = new InputFeatureUsage<Vector3>("RightEyePositionGuide");
        public static InputFeatureUsage<Vector3> foveatedGazeDirection = new InputFeatureUsage<Vector3>("FoveatedGazeDirection");
        public static InputFeatureUsage<uint> foveatedGazeTrackingState = new InputFeatureUsage<uint>("FoveatedGazeTrackingState");


        public static InputFeatureUsage<bool> triggerTouch = new InputFeatureUsage<bool>("TriggerTouch");
        public static InputFeatureUsage<bool> thumbRestTouch = new InputFeatureUsage<bool>("ThumbRestTouch");
        public static InputFeatureUsage<float> grip1DAxis = new InputFeatureUsage<float>("Grip1DAxis");

        public static InputFeatureUsage<bool> controllerStatus = new InputFeatureUsage<bool>("ControllerStatus");

    }
}

