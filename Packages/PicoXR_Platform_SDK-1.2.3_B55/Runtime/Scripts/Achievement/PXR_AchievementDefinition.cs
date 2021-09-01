using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_AchievementDefinition
    {
        public readonly AchievementType Type;
        public readonly string Name;
        public readonly int BitfieldLength;
        public readonly long Target;
        public readonly string Title;
        public readonly string Description;
        public readonly string UnlockedDescription;
        public readonly string UnlockedIcon;
        public readonly string LockedIcon;
        public readonly bool IsSecrect;


        public PXR_AchievementDefinition(AndroidJavaObject msg)
        {
            Type = PXR_AchievementAPI.pvr_AchievementDefinition_GetType(msg);
            Name = PXR_AchievementAPI.pvr_AchievementDefinition_GetName(msg);
            BitfieldLength = PXR_AchievementAPI.pvr_AchievementDefinition_GetBitfieldLength(msg);
            Target = PXR_AchievementAPI.pvr_AchievementDefinition_GetTarget(msg);
            Title = PXR_AchievementAPI.pvr_AchievementDefinition_GetTitle(msg);
            Description = PXR_AchievementAPI.pvr_AchievementDefinition_GetDescription(msg);
            UnlockedDescription = PXR_AchievementAPI.pvr_AchievementDefinition_GetUnlockedDescription(msg);
            UnlockedIcon = PXR_AchievementAPI.pvr_AchievementDefinition_GetUnlockedIcon(msg);
            LockedIcon = PXR_AchievementAPI.pvr_AchievementDefinition_GetLockedIcon(msg);
            IsSecrect = PXR_AchievementAPI.pvr_AchievementDefinition_GetIsSecrect(msg);
        }
    }

    public class PXR_AchievementDefinitionList : PXR_DeserializableList<PXR_AchievementDefinition>
    {
        public PXR_AchievementDefinitionList(AndroidJavaObject msg)
        {
            var count = PXR_AchievementAPI.pvr_AchievementDefinitionArray_GetSize(msg);
            data = new List<PXR_AchievementDefinition>(count);
            for (int i = 0; i < count; i++)
            {
                data.Add(new PXR_AchievementDefinition(PXR_AchievementAPI.pvr_AchievementDefinitionArray_GetElement(msg, i)));
            }

            nextUrl = PXR_AchievementAPI.pvr_AchievementDefinitionArray_GetNextUrl(msg);
        }

    }
}
