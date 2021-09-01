/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_TouchPadEffects : MonoBehaviour
    {
        private MeshRenderer touchRenderer;
        [HideInInspector]
        public PXR_Input.ControllerDevice currentDevice;

        private Vector2 touchPos = new Vector2();

        void Start()
        {
            touchRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out touchPos);

            if (touchPos != Vector2.zero)
            {
                touchRenderer.enabled = true;
                transform.localPosition = new Vector3(-touchPos.x * 1.28f, 1.6f, -2.98f - touchPos.y * 1.28f);
            }
            else
            {
                touchRenderer.enabled = false;
            }
        }
    }
}
