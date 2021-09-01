/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public class PXR_OverLay : MonoBehaviour, IComparable<PXR_OverLay>
    {
        public static List<PXR_OverLay> Instances = new List<PXR_OverLay>();
        public int layerIndex = 0;
        public OverlayType overlayType = OverlayType.Overlay;
        public OverlayShape overlayShape = OverlayShape.Quad;
        public Transform layerTransform;

        public Texture[] layerTextures = new Texture[2];

        public int[] layerTextureIds = new int[2];
        public Matrix4x4[] mvMatrixs = new Matrix4x4[2];
        public Vector3[] modelScales = new Vector3[2];
        public Quaternion[] modelRotations = new Quaternion[2];
        public Vector3[] modelTranslations = new Vector3[2];
        public Quaternion[] cameraRotations = new Quaternion[2];
        public Vector3[] cameraTranslations = new Vector3[2];
        public Camera[] layerEyeCamera = new Camera[2];

        public bool overrideColorScaleAndOffset = false;
        public Vector4 colorScale = Vector4.one;
        public Vector4 colorOffset = Vector4.zero;

        private Vector4 overlayLayerColorScaleDefault = Vector4.one;
        private Vector4 overlayLayerColorOffsetDefault = Vector4.zero;

        public bool isExternalAndroidSurface = false;
        public IntPtr externalAndroidSurfaceObject = IntPtr.Zero;
        public delegate void ExternalAndroidSurfaceObjectCreated();
        public ExternalAndroidSurfaceObjectCreated externalAndroidSurfaceObjectCreated = null;

        public Camera xrRig;
        public enum OverlayShape
        {
            Quad = 0,
            Cylinder = 1,
            Equirect = 2,
        }

        public enum OverlayType
        {
            Overlay = 0,
            Underlay = 1
        }

        public enum OverlayTexFilterMode
        {
            NotCare = 0,
            Nearest = 1,
            Linear = 2,
            NearestMipmapNearest = 3,
            LinearMipmapNearest = 4,
            NearestMipmapLinear = 5,
            LinearMipmapLinear = 6
        }

        public int CompareTo(PXR_OverLay other)
        {
            return layerIndex.CompareTo(other.layerIndex);
        }

        private void Awake()
        {
            xrRig = Camera.main;
            Instances.Add(this);

            layerEyeCamera[0] = xrRig;
            layerEyeCamera[1] = xrRig;
          
            layerTransform = GetComponent<Transform>();
#if !UNITY_EDITOR && UNITY_ANDROID
            if (layerTransform != null)
            {
                MeshRenderer render = layerTransform.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.enabled = false;
                }
            }
#endif
            InitializeBuffer();
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
        }
        
        public void RefreshCamera()
        {
            xrRig = Camera.main;
            layerEyeCamera[0] = xrRig; 
            layerEyeCamera[1] = xrRig;  
        }

        private void InitializeBuffer()
        {
            switch (overlayShape)
            {
                case OverlayShape.Quad:
                case OverlayShape.Cylinder:
                case OverlayShape.Equirect:
                    //case OverlayShape.CubeMap:
                    for (int i = 0; i < layerTextureIds.Length; i++)
                    {
                        if (layerTextures[i] != null)
                        {
                            layerTextureIds[i] = layerTextures[i].GetNativeTexturePtr().ToInt32();
                        }
                    }
                    break;
            }
        }

        public void UpdateCoords()
        {
            if (layerTransform == null || !layerTransform.gameObject.activeSelf)
            {
                return;
            }

            if (layerEyeCamera[0] == null || layerEyeCamera[1] == null)
            {
                return;
            }

            // update MV matrix
            for (int i = 0; i < mvMatrixs.Length; i++)
            {
                mvMatrixs[i] = layerEyeCamera[i].worldToCameraMatrix * layerTransform.localToWorldMatrix;

                modelScales[i] = layerTransform.localScale;
                modelRotations[i] = layerTransform.rotation;
                modelTranslations[i] = layerTransform.position;
                cameraRotations[i] = layerEyeCamera[i].transform.rotation;
                cameraTranslations[i] = layerEyeCamera[i].transform.position;
            }
        }

        public void SetTexture(Texture texture)
        {
            for (int i = 0; i < layerTextures.Length; i++)
            {
                layerTextures[i] = texture;
            }
            InitializeBuffer();
        }

        public void SetLayerColorScaleAndOffset(Vector4 scale, Vector4 offset)
        {
            colorScale = scale;
            colorOffset = offset;
        }

        public Vector4 GetLayerColorScale()
        {
            return !overrideColorScaleAndOffset ? overlayLayerColorScaleDefault : colorScale;
        }

        public Vector4 GetLayerColorOffset()
        {
            return !overrideColorScaleAndOffset ? overlayLayerColorOffsetDefault : colorOffset;
        }
    }
}

