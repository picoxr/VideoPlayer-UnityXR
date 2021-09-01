using UnityEngine;

namespace Unity.XR.PXR
{
    public abstract class PXR_Message<T> : PXR_Message
    {
        public new delegate void Callback(PXR_Message<T> message);
        public PXR_Message(AndroidJavaObject msg) : base(msg)
        {
            if (!IsError)
            {
                data = GetDataFromMessage(msg);
            }
        }

        public T Data { get { return data; } }
        protected abstract T GetDataFromMessage(AndroidJavaObject msg);
        private T data;
    }

    public class PXR_Message
    {
        public delegate void Callback(PXR_Message message);
        public PXR_Message(AndroidJavaObject msg)
        {
            type = (MessageType)PXR_AchievementAPI.pvr_Message_GetType(msg);
            var isError = PXR_AchievementAPI.pvr_Message_IsError(msg);
            requestID = PXR_AchievementAPI.pvr_Message_GetRequestID(msg);
            if (isError)
            {
                error = new Error(
                  PXR_AchievementAPI.pvr_Error_GetCode(msg),
                  PXR_AchievementAPI.pvr_Error_GetMessage(msg),
                  PXR_AchievementAPI.pvr_Error_GetHttpCode(msg));
            }
            else if (PXR_AchievementCore.LogMessages)
            {
                var message = PXR_AchievementAPI.pvr_Message_GetString(msg);
                if (message != null)
                {
                    Debug.Log(message);
                }
                else
                {
                    Debug.Log(string.Format("null message string {0}", msg));
                }
            }
        }
        public enum MessageType : uint
        {
            Unknown,

            Achievements_AddCount = 0x03E76231,
            Achievements_AddFields = 0x14AA2129,
            Achievements_GetAllDefinitions = 0x03D3458D,
            Achievements_GetAllProgress = 0x4F9FDE1D,
            Achievements_GetDefinitionsByName = 0x629101BC,
            Achievements_GetNextAchievementDefinitionArrayPage = 0x2A7DD255,
            Achievements_GetNextAchievementProgressArrayPage = 0x2F42E727,
            Achievements_GetProgressByName = 0x152663B1,
            Achievements_Unlock = 0x593CCBDD,
            Achievements_WriteAchievementProgress = 0x736BBDD,
            Achievements_VerifyAccessToken = 0x032D103C
        };

        public MessageType Type { get { return type; } }
        public bool IsError { get { return error != null; } }
        public long RequestID { get { return requestID; } }

        private MessageType type;
        private long requestID;
        private Error error;

        public virtual Error GetError() { return error; }
        public virtual PXR_AchievementDefinitionList GetAchievementDefinitions() { return null; }
        public virtual PXR_AchievementProgressList GetAchievementProgressList() { return null; }
        public virtual PXR_AchievementUpdate GetAchievementUpdate() { return null; }
        public virtual string GetString() { return null; }

        internal static PXR_Message ParseMessageHandle(AndroidJavaObject messageHandle)
        {
            if (messageHandle == null)
            {
                return null;
            }

            PXR_Message message = null;
            MessageType message_type = (MessageType)PXR_AchievementAPI.pvr_Message_GetType(messageHandle);

            switch (message_type)
            {
                case MessageType.Achievements_GetAllDefinitions:
                case MessageType.Achievements_GetDefinitionsByName:
                case MessageType.Achievements_GetNextAchievementDefinitionArrayPage:
                    message = new MessageWithAchievementDefinitions(messageHandle);
                    break;

                case MessageType.Achievements_GetAllProgress:
                case MessageType.Achievements_GetNextAchievementProgressArrayPage:
                case MessageType.Achievements_GetProgressByName:
                    message = new MessageWithAchievementProgressList(messageHandle);
                    break;

                case MessageType.Achievements_AddCount:
                case MessageType.Achievements_AddFields:
                case MessageType.Achievements_Unlock:
                case MessageType.Achievements_VerifyAccessToken:
                    message = new MessageWithAchievementUpdate(messageHandle);
                    break;

            }

            return message;
        }

        public static PXR_Message PopMessage()
        {
            var messageHandle = PXR_AchievementAPI.PopMessage();

            PXR_Message message = ParseMessageHandle(messageHandle);

            return message;
        }

        internal delegate PXR_Message ExtraMessageTypesHandler(AndroidJavaObject messageHandle, MessageType message_type);
        internal static ExtraMessageTypesHandler HandleExtraMessageTypes { set; private get; }
    }


    public class MessageWithAchievementDefinitions : PXR_Message<PXR_AchievementDefinitionList>
    {
        public MessageWithAchievementDefinitions(AndroidJavaObject msg) : base(msg) { }
        public override PXR_AchievementDefinitionList GetAchievementDefinitions() { return Data; }
        protected override PXR_AchievementDefinitionList GetDataFromMessage(AndroidJavaObject msg)
        {
            return new PXR_AchievementDefinitionList(msg);
        }

    }
    public class MessageWithAchievementProgressList : PXR_Message<PXR_AchievementProgressList>
    {
        public MessageWithAchievementProgressList(AndroidJavaObject msg) : base(msg) { }
        public override PXR_AchievementProgressList GetAchievementProgressList() { return Data; }
        protected override PXR_AchievementProgressList GetDataFromMessage(AndroidJavaObject msg)
        {
            return new PXR_AchievementProgressList(msg);
        }

    }
    public class MessageWithAchievementUpdate : PXR_Message<PXR_AchievementUpdate>
    {
        public MessageWithAchievementUpdate(AndroidJavaObject msg) : base(msg) { }
        public override PXR_AchievementUpdate GetAchievementUpdate() { return Data; }
        protected override PXR_AchievementUpdate GetDataFromMessage(AndroidJavaObject msg)
        {
            return new PXR_AchievementUpdate(msg);
        }

    }
    public class MessageWithString : PXR_Message<string>
    {
        public MessageWithString(AndroidJavaObject msg) : base(msg) { }
        public override string GetString() { return Data; }
        protected override string GetDataFromMessage(AndroidJavaObject msg)
        {
            return PXR_AchievementAPI.pvr_Message_GetString(msg);
        }
    }
    public class Error
    {
        public Error(int code, string message, int httpCode)
        {
            Message = message;
            Code = code;
            HttpCode = httpCode;
        }

        public readonly int Code;
        public readonly int HttpCode;
        public readonly string Message;
    }

}
