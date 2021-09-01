/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.UI;


namespace Unity.XR.PXR
{
    public enum EyesEnum
    {
        LeftEye,
        RightEye
    }
    public class PXR_PassThrough : MonoBehaviour
    {
        public RawImage viewImageLeft, viewImageRight;

        private PXR_PassThroughSystem passThroughSystem;
        private PXR_Loader loader;
        private Texture2D cameraTexLeft, cameraTexRight;
        private int width, height;

        private void Start()
        {
            width = 600;
            height = 600;
        }

        void OnEnable()
        {
            loader = XRGeneralSettings.Instance.Manager.activeLoader as PXR_Loader;
            if (loader == null)
            {
                Debug.LogError("Has no XR loader in the project!");
                return;
            }
            passThroughSystem = loader.GetLoadedSubsystem<PXR_PassThroughSystem>();
            if (passThroughSystem == null)
            {
                Debug.LogError("Has no XR Camera subsystem !");
                return;
            }
            passThroughSystem.Start();
        }

        private void OnDisable()
        {
            if (passThroughSystem != null)
            {
                passThroughSystem.Stop();
            }
            loader = null;
            passThroughSystem = null;
            cameraTexLeft = null;
            cameraTexRight = null;
        }

        void Update()
        {
#if UNITY_INPUT_SYSTEM
            //new confirm button
#else
            //Confirm button  
            if (Input.GetKey(KeyCode.JoystickButton0))
            {
                DrawTexture();
            }
#endif
        }

        private void DrawTexture()
        {
            if (passThroughSystem == null) return;

            passThroughSystem.UpdateTextures();
            int eye = (int)EyesEnum.LeftEye;
            //left eye
            IntPtr textureId = (IntPtr)passThroughSystem.UpdateCameraTextureID(eye);
            if (cameraTexLeft == null)
                cameraTexLeft = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, QualitySettings.activeColorSpace == ColorSpace.Linear, textureId);
            else
            {
                cameraTexLeft.UpdateExternalTexture(textureId);
            }

            //right eye
            eye = (int)EyesEnum.RightEye;
            textureId = (IntPtr)passThroughSystem.UpdateCameraTextureID(eye);
            if (cameraTexRight == null)
                cameraTexRight = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, QualitySettings.activeColorSpace == ColorSpace.Linear, textureId);
            else
            {
                cameraTexRight.UpdateExternalTexture(textureId);
            }

            viewImageLeft.texture = cameraTexLeft;
            viewImageRight.texture = cameraTexRight;
        }
    }
}