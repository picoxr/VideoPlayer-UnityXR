using UnityEngine;

namespace Unity.XR.PXR
{
    public sealed class PXR_AchievementCore
    {
        private static bool IsPlatformInitialized = true;
        public static bool IsInitialized()
        {
            return IsPlatformInitialized;
        }

        public static void Initialize()
        {
        }

        public static void RegisterNetwork()
        {
            PXR_AchievementAPI.RegisterNetwork();
        }

        public static void UnRegisterNetwork()
        {
            PXR_AchievementAPI.UnRegisterNetwork();
        }

        // If LogMessages is true, then the contents of each request response
        // will be printed using Debug.Log. This allocates a lot of heap memory,
        // and so should not be called outside of testing and debugging.
        public static bool LogMessages = false;

    }

    public static partial class PXR_Achievement
    {
        public static PXR_Request<PXR_AchievementUpdate> Init()
        {

            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementUpdate>(PXR_AchievementAPI.Init());
            }
            return null;
        }
        /*Add 'count' to the achievement with the given name. This must be a COUNT
         achievement. The largest number that is supported by this method is the max
        value of a signed 64-bit integer. If the number is larger than that, it is
        clamped to that max value before being passed to the servers.
        */
        public static PXR_Request<PXR_AchievementUpdate> AddCount(string name, long count)
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementUpdate>(PXR_AchievementAPI.pvr_Achievements_AddCount(name, count));
            }

            return null;
        }

        /// Unlock fields of a BITFIELD achievement.
        /// \param name The name of the achievement to unlock
        /// \param fields A string containing either '0' or '1' characters. Every '1' will unlock the field in the corresponding position.
        ///
        public static PXR_Request<PXR_AchievementUpdate> AddFields(string name, string fields)
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementUpdate>(PXR_AchievementAPI.pvr_Achievements_AddFields(name, fields));
            }

            return null;
        }

        /// Request all achievement definitions for the app.
        ///
        public static PXR_Request<PXR_AchievementDefinitionList> GetAllDefinitions()
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementDefinitionList>(PXR_AchievementAPI.pvr_Achievements_GetAllDefinitions());
            }

            return null;
        }

        /// Request the progress for the user on all achievements in the app.
        ///
        public static PXR_Request<PXR_AchievementProgressList> GetAllProgress()
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementProgressList>(PXR_AchievementAPI.pvr_Achievements_GetAllProgress());
            }

            return null;
        }

        /// Request the achievement definitions that match the specified names.
        ///
        public static PXR_Request<PXR_AchievementDefinitionList> GetDefinitionsByName(string[] names)
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementDefinitionList>(PXR_AchievementAPI.pvr_Achievements_GetDefinitionsByName(names, (names != null ? names.Length : 0)));
            }

            return null;
        }

        /// Request the user's progress on the specified achievements.
        ///
        public static PXR_Request<PXR_AchievementProgressList> GetProgressByName(string[] names)
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementProgressList>(PXR_AchievementAPI.pvr_Achievements_GetProgressByName(names, (names != null ? names.Length : 0)));
            }

            return null;
        }

        /// Unlock the achievement with the given name. This can be of any achievement
        /// type.
        ///
        public static PXR_Request<PXR_AchievementUpdate> Unlock(string name)
        {
            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementUpdate>(PXR_AchievementAPI.pvr_Achievements_Unlock(name));
            }

            return null;
        }

        public static PXR_Request<PXR_AchievementDefinitionList> GetNextAchievementDefinitionListPage(PXR_AchievementDefinitionList list)
        {
            if (!list.HasNextPage)
            {
                Debug.LogWarning("Platform.GetNextAchievementDefinitionListPage: List has no next page");
                return null;
            }

            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementDefinitionList>(
                  PXR_AchievementAPI.pvr_HTTP_GetWithMessageType(
                    list.NextUrl,
                    PXR_Message.MessageType.Achievements_GetNextAchievementDefinitionArrayPage
                  )
                );
            }

            return null;
        }

        public static PXR_Request<PXR_AchievementProgressList> GetNextAchievementProgressListPage(PXR_AchievementProgressList list)
        {
            if (!list.HasNextPage)
            {
                Debug.LogWarning("Platform.GetNextAchievementProgressListPage: List has no next page");
                return null;
            }

            if (PXR_AchievementCore.IsInitialized())
            {
                return new PXR_Request<PXR_AchievementProgressList>(
                  PXR_AchievementAPI.pvr_HTTP_GetWithMessageType(
                    list.NextUrl,
                    PXR_Message.MessageType.Achievements_GetNextAchievementProgressArrayPage
                  )
                );
            }

            return null;
        }

    }
}
