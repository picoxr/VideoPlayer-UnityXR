// Copyright  2015-2020 Pico Technology Co., Ltd. All Rights Reserved.


using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class PXR_OverlayExternalSurfaceDemo : MonoBehaviour {

    public string movieName;

    public PXR_OverLay.OverlayType overlayType;
    public PXR_OverLay.OverlayShape overlayShape;

    private PXR_OverLay overlay = null;

    private void Awake()
    {
        overlay = GetComponent<PXR_OverLay>();
        if (overlay == null)
        {
            Debug.LogError("PXRLog Overlay is null!");
            overlay = gameObject.AddComponent<PXR_OverLay>();
        }

        overlay.overlayType = overlayType;
        overlay.overlayShape = overlayShape;
        overlay.isExternalAndroidSurface = true;
    }

    // Use this for initialization
    void Start()
    {
        if (!string.IsNullOrEmpty(movieName))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            StartPlay(Application.streamingAssetsPath + "/" + movieName, null);
#endif
        }
    }


    void StartPlay(string moviePath, string licenceUrl)
    {
        if (moviePath != string.Empty)
        {
            if (overlay.isExternalAndroidSurface)
            {
                PXR_OverLay.ExternalAndroidSurfaceObjectCreated surfaceObjectCreatedCallback = () =>
                {
                    Debug.Log("PXRLog SurfaceObject created callback is Invoke().");
                    // TODO:
                    // You need pass externalAndroidSurfaceObject to one android video player for video texture updates.
                    // eg.if you use Android ExoPlayer,you can call exoPlayer.setVideoSurface( surface );
                };

                if (overlay.externalAndroidSurfaceObject == System.IntPtr.Zero)
                {
                    Debug.Log("PXRLog Register surfaceObject created callback");
                    overlay.externalAndroidSurfaceObjectCreated = surfaceObjectCreatedCallback;
                }
                else
                {
                    Debug.Log("PXRLog SurfaceObject is already created! Invoke callback");
                    surfaceObjectCreatedCallback.Invoke();
                }
            }
        }
        else
        {
            Debug.LogError("PXRLog Movie path is null!");
        }
    }
}
