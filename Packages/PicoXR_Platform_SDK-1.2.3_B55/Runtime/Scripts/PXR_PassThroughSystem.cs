/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using Unity.XR.PXR;
using UnityEngine;

[UnityEngine.Scripting.Preserve]
public class PXR_PassThroughSystem : Subsystem<PXR_PassThroughDescriptor>
{
    bool m_Running = false;
    public override bool running => m_Running;

    PXR_PassThroughProvider m_Provider = new PXR_PassThroughProvider();

#if !UNITY_2020_1_OR_NEWER
    bool m_Destroyed;
#endif

    public override void Start()
    {
        if (!m_Running)
        {
            m_Provider.Start();
        }
        m_Running = true;
    }

    public override void Stop()
    {
        if (m_Running)
        {
            m_Provider.Stop();
        }
        m_Running = false;
    }

    public void UpdateTextures()
    {
        m_Provider.UpdateTextures();
    }

    protected override void OnDestroy()
    {
#if !UNITY_2020_1_OR_NEWER
        if (m_Destroyed)
            return;
        m_Destroyed = true;
#endif
        Stop();
        m_Running = false;
        m_Provider.Destroy();
    }

    public int UpdateCameraTextureID(int eye)
    {
        return PXR_Plugin.PassThrough.UPxr_PassThroughUpdateFrame(eye);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Register()
    {
        PXR_PassThroughDescriptor.Cinfo info = new PXR_PassThroughDescriptor.Cinfo();
        info.id = "PicoXR Camera";
        info.ImplementaionType = typeof(PXR_PassThroughSystem);
        PXR_PassThroughDescriptor descriptor = PXR_PassThroughDescriptor.Create(info);
        SubsystemRegistration.CreateDescriptor(descriptor);
    }


    class PXR_PassThroughProvider
    {
        IntPtr m_RenderEventFunc;

        public PXR_PassThroughProvider()
        {
            if (SystemInfo.graphicsMultiThreaded)
            {
                m_RenderEventFunc = PXR_Plugin.PassThrough.UPxr_PassThroughGetRenderEventFunc();
            }
        }

        public void Start()
        {
            CreateTexture();
            PXR_Plugin.PassThrough.UPxr_PassThroughStart();
        }

        public void Stop()
        {
            PXR_Plugin.PassThrough.UPxr_PassThroughStop();
        }

        public void Destroy()
        {
            PXR_Plugin.PassThrough.UPxr_PassThroughDestroy();
        }

        void IssueRenderEventAndWaitForCompletion(RenderEvent renderEvent)
        {
            // NB: If m_RenderEventFunc is zero, it means
            //     1. We are running in the Editor.
            //     2. The UnityARCore library could not be loaded or similar catastrophic failure.
            if (m_RenderEventFunc != IntPtr.Zero)
            {
                PXR_Plugin.PassThrough.UPxr_PassThroughSetRenderEventPending();
                GL.IssuePluginEvent(m_RenderEventFunc, (int)renderEvent);
                PXR_Plugin.PassThrough.UPxr_PassThroughWaitForRenderEvent();
            }
        }


        void CreateTexture()
        {
            if (SystemInfo.graphicsMultiThreaded)
            {
                IssueRenderEventAndWaitForCompletion(RenderEvent.CreateTexture);
            }
            else
            {
                PXR_Plugin.PassThrough.UPxr_PassThroughCreateTexutresMainThread();
            }
        }

        void DeleteTexture()
        {
            if (SystemInfo.graphicsMultiThreaded)
            {
                IssueRenderEventAndWaitForCompletion(RenderEvent.DeleteTexture);
            }
            else
            {
                PXR_Plugin.PassThrough.UPxr_PassThroughDeleteTexutresMainThread();
            }
        }

        public void UpdateTextures()
        {
            if (SystemInfo.graphicsMultiThreaded)
            {
                IssueRenderEventAndWaitForCompletion(RenderEvent.UpdateTexture);
            }
            else
            {
                PXR_Plugin.PassThrough.UPxr_PassThroughUpdateTexutresMainThread();
            }
        }
    }
}
