using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.XR.PXR
{
    public class PXR_OverlayManager : MonoBehaviour
    {
        private PXR_OverLay overLay;
        private int overlayLayerDepth = 1;
        private int underlayLayerDepth = 0;
        private bool isHeadLocked = false;
        private int layerFlags = 0;

        private void OnEnable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.endCameraRendering += MyPostRender;
            }
#endif
        }

        private void OnDisable()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.endCameraRendering -= MyPostRender;
            }
#endif
        }

        private void MyPostRender(ScriptableRenderContext arg1, Camera arg2)
        {
            OnPostRender();
        }
        private void OnPostRender()
        {
            // Composite Layers: if find Overlay then Open Composite Layers feature
            int boundaryState = PXR_Plugin.Boundary.UPxr_GetSeeThroughState();
            if (PXR_OverLay.Instances.Count > 0 && boundaryState != 2)
            {
                overlayLayerDepth = 1;
                underlayLayerDepth = 0;

                PXR_OverLay.Instances.Sort();

                for (int i = 0; i < PXR_OverLay.Instances.Count; i++)
                {
                    overLay = PXR_OverLay.Instances[i];
                    overLay.UpdateCoords();
                    if (!overLay.isActiveAndEnabled) continue;
                    if (overLay.layerTextures[0] == null && overLay.layerTextures[1] == null && !overLay.isExternalAndroidSurface) continue;
                    if (overLay.layerTransform != null && !overLay.layerTransform.gameObject.activeSelf) continue;

                    layerFlags = 0;

                    if (overLay.overlayShape == PXR_OverLay.OverlayShape.Quad || overLay.overlayShape == PXR_OverLay.OverlayShape.Cylinder)
                    {
                        if (overLay.overlayType == PXR_OverLay.OverlayType.Overlay)
                        {
                            isHeadLocked = false;
                            if (overLay.layerTransform != null && overLay.layerTransform.parent == transform)
                            {
                                isHeadLocked = true;
                            }

                            // external surface
                            if (overLay.isExternalAndroidSurface)
                            {
                                layerFlags = 1;
                                CreateExternalSurface(overLay, overlayLayerDepth);
                            }

                            PXR_Plugin.Boundary.UPxr_SetOverlayModelViewMatrix((int)overLay.overlayType, (int)overLay.overlayShape, overLay.layerTextureIds[0], 0, overlayLayerDepth, isHeadLocked, layerFlags, overLay.mvMatrixs[0],
                            overLay.modelScales[0], overLay.modelRotations[0], overLay.modelTranslations[0], overLay.cameraRotations[0], overLay.cameraTranslations[0], overLay.GetLayerColorScale(), overLay.GetLayerColorOffset());

                            PXR_Plugin.Boundary.UPxr_SetOverlayModelViewMatrix((int)overLay.overlayType, (int)overLay.overlayShape, overLay.layerTextureIds[1], 1, overlayLayerDepth, isHeadLocked, layerFlags, overLay.mvMatrixs[1],
                            overLay.modelScales[1], overLay.modelRotations[1], overLay.modelTranslations[1], overLay.cameraRotations[1], overLay.cameraTranslations[1], overLay.GetLayerColorScale(), overLay.GetLayerColorOffset());

                            overlayLayerDepth++;
                        }
                        else if (overLay.overlayType == PXR_OverLay.OverlayType.Underlay)
                        {
                            // external surface
                            if (overLay.isExternalAndroidSurface)
                            {
                                layerFlags = 1;
                                CreateExternalSurface(overLay, underlayLayerDepth);
                            }

                            PXR_Plugin.Boundary.UPxr_SetOverlayModelViewMatrix((int)overLay.overlayType, (int)overLay.overlayShape, overLay.layerTextureIds[0], 0, underlayLayerDepth, false, layerFlags, overLay.mvMatrixs[0],
                            overLay.modelScales[0], overLay.modelRotations[0], overLay.modelTranslations[0], overLay.cameraRotations[0], overLay.cameraTranslations[0], overLay.GetLayerColorScale(), overLay.GetLayerColorOffset());

                            PXR_Plugin.Boundary.UPxr_SetOverlayModelViewMatrix((int)overLay.overlayType, (int)overLay.overlayShape, overLay.layerTextureIds[1], 1, underlayLayerDepth, false, layerFlags, overLay.mvMatrixs[1],
                            overLay.modelScales[1], overLay.modelRotations[1], overLay.modelTranslations[1], overLay.cameraRotations[1], overLay.cameraTranslations[1], overLay.GetLayerColorScale(), overLay.GetLayerColorOffset());

                            underlayLayerDepth++;
                        }
                    }
                    else if (overLay.overlayShape == PXR_OverLay.OverlayShape.Equirect)
                    {
                        // external surface
                        if (overLay.isExternalAndroidSurface)
                        {
                            layerFlags = 1;
                            CreateExternalSurface(overLay, 0);
                        }

                        // 360 Overlay Equirectangular Texture
                        PXR_Plugin.Boundary.UPxr_SetupLayerData(0, 0, overLay.layerTextureIds[0], (int)overLay.overlayShape, layerFlags, overLay.GetLayerColorScale(), overLay.GetLayerColorOffset());
                        PXR_Plugin.Boundary.UPxr_SetupLayerData(0, 1, overLay.layerTextureIds[1], (int)overLay.overlayShape, layerFlags, overLay.GetLayerColorScale(), overLay.GetLayerColorOffset());
                    }
                }
            }
        }

        private void CreateExternalSurface(PXR_OverLay overlayInstance, int layerDepth)
        {
            if (overlayInstance.externalAndroidSurfaceObject == IntPtr.Zero)
            {
                overlayInstance.externalAndroidSurfaceObject = PXR_Plugin.Boundary.UPxr_CreateLayerAndroidSurface((int)overlayInstance.overlayType, layerDepth);

                if (overlayInstance.externalAndroidSurfaceObject != IntPtr.Zero && overlayInstance.externalAndroidSurfaceObjectCreated != null)
                {
                    overlayInstance.externalAndroidSurfaceObjectCreated();
                }
            }
        }
    }
}

