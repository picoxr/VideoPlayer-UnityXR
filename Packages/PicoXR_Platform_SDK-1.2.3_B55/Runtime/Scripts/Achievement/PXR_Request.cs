using UnityEngine;

namespace Unity.XR.PXR
{
    public sealed class PXR_Request<T> : Request
    {
        private PXR_Message<T>.Callback callback_ = null;

        public PXR_Request(long requestID) : base(requestID) { }

        public PXR_Request<T> OnComplete(PXR_Message<T>.Callback callback)
        {
            if (callback_ != null)
            {
                throw new UnityException("Attempted to attach multiple handlers to a Request.  This is not allowed.");
            }

            callback_ = callback;
            PXR_Callback.AddRequest(this);
            return this;
        }

        override public void HandleMessage(PXR_Message msg)
        {
            if (!(msg is PXR_Message<T>))
            {
                Debug.LogError("Unable to handle message: " + msg.GetType());
                return;
            }

            if (callback_ != null)
            {
                callback_((PXR_Message<T>)msg);
                return;
            }

            throw new UnityException("Request with no handler.  This should never happen.");
        }
    }

    public class Request
    {
        private PXR_Message.Callback callback_;

        public Request(long requestID) { RequestID = requestID; }
        public long RequestID { get; set; }

        public Request OnComplete(PXR_Message.Callback callback)
        {
            callback_ = callback;
            PXR_Callback.AddRequest(this);
            return this;
        }

        virtual public void HandleMessage(PXR_Message msg)
        {
            if (callback_ != null)
            {
                callback_(msg);
                return;
            }

            throw new UnityException("Request with no handler.  This should never happen.");
        }

        public static void RunCallbacks(uint limit = 0)
        {
            // default of 0 will run callbacks on all messages on the queue
            if (limit == 0)
            {
                PXR_Callback.RunCallbacks();
            }
            else
            {
                PXR_Callback.RunLimitedCallbacks(limit);
            }
        }
    }
}
