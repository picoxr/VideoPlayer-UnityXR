using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public static class PXR_Callback
    {
        #region Adding and running request handlers
        internal static void AddRequest(Request request)
        {
            if (request.RequestID <= 2)
            {
                switch (request.RequestID)
                {
                    case 0:
                        Debug.LogError("An PARAM_INVALIDE error occurred. Request failed.");
                        break;
                    case 1:
                        Debug.LogError("An NETWORK_INVALIDE error occurred. Request failed.");
                        break;
                    case 2:
                        Debug.LogError("An NOT_INTIALIZE error occurred. Request failed.");
                        break;
                    default:
                        Debug.LogError("An unknown error occurred. Request failed.");
                        break;
                }
                return;
            }
            requestIDsToRequests[request.RequestID] = request;
        }

        internal static void RunCallbacks()
        {
            while (true)
            {
                var msg = PXR_Message.PopMessage();
                if (msg == null)
                {
                    break;
                }

                HandleMessage(msg);
            }

        }

        internal static void RunLimitedCallbacks(uint limit)
        {
            for (var i = 0; i < limit; ++i)
            {
                var msg = PXR_Message.PopMessage();
                if (msg == null)
                {
                    break;
                }

                HandleMessage(msg);
            }
        }

        internal static void OnApplicationQuit()
        {
            requestIDsToRequests.Clear();
            notificationCallbacks.Clear();
        }

        #endregion

        #region Callback Internals
        private static Dictionary<long, Request> requestIDsToRequests = new Dictionary<long, Request>();
        private static Dictionary<PXR_Message.MessageType, RequestCallback> notificationCallbacks = new Dictionary<PXR_Message.MessageType, RequestCallback>();



        private class RequestCallback
        {
            private PXR_Message.Callback messageCallback;

            public RequestCallback() { }

            public RequestCallback(PXR_Message.Callback callback)
            {
                this.messageCallback = callback;
            }

            public virtual void HandleMessage(PXR_Message msg)
            {
                if (messageCallback != null)
                {
                    messageCallback(msg);
                }
            }
        }

        private sealed class RequestCallback<T> : RequestCallback
        {
            private PXR_Message<T>.Callback callback;
            public RequestCallback(PXR_Message<T>.Callback callback)
            {
                this.callback = callback;
            }

            public override void HandleMessage(PXR_Message msg)
            {
                if (callback != null)
                {

                    if (msg is PXR_Message<T>)
                    {
                        callback((PXR_Message<T>)msg);
                    }
                    else
                    {
                        Debug.LogError("Unable to handle message: " + msg.GetType());
                    }
                }
            }
        }

        internal static void HandleMessage(PXR_Message msg)
        {
            Request request;
            if (msg.RequestID != 0 && requestIDsToRequests.TryGetValue(msg.RequestID, out request))
            {
                try
                {
                    request.HandleMessage(msg);
                }
                finally
                {
                    requestIDsToRequests.Remove(msg.RequestID);
                }
                return;
            }

            RequestCallback callbackHolder;
            if (notificationCallbacks.TryGetValue(msg.Type, out callbackHolder))
            {
                callbackHolder.HandleMessage(msg);
            }
        }

        #endregion
    }
}
