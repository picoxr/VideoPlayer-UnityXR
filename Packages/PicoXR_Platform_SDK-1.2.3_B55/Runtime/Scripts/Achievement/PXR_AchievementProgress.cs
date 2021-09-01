
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
  public class PXR_AchievementProgress
  {
    public readonly string Bitfield;
    public readonly long Count;
    public readonly bool IsUnlocked;
    public readonly string Name;
    public readonly DateTime UnlockTime;


    public PXR_AchievementProgress(AndroidJavaObject msg)
    {
      Bitfield = PXR_AchievementAPI.pvr_AchievementProgress_GetBitfield(msg);
      Count = PXR_AchievementAPI.pvr_AchievementProgress_GetCount(msg);
      IsUnlocked = PXR_AchievementAPI.pvr_AchievementProgress_GetIsUnlocked(msg);
      Name = PXR_AchievementAPI.pvr_AchievementProgress_GetName(msg);
      UnlockTime = PXR_AchievementAPI.pvr_AchievementProgress_GetUnlockTime(msg);
    }
  }

  public class PXR_AchievementProgressList : PXR_DeserializableList<PXR_AchievementProgress> {
    public PXR_AchievementProgressList(AndroidJavaObject msg) {
      var count = PXR_AchievementAPI.pvr_AchievementProgressArray_GetSize(msg);
      data = new List<PXR_AchievementProgress>(count);
      for (int i = 0; i < count; i++) {
        data.Add(new PXR_AchievementProgress(PXR_AchievementAPI.pvr_AchievementProgressArray_GetElement(msg, i)));
      }

      nextUrl = PXR_AchievementAPI.pvr_AchievementProgressArray_GetNextUrl(msg);
    }

  }
}
