using UnityEngine;

namespace Unity.XR.PXR
{
  public class PXR_AchievementUpdate
  {
    public readonly bool JustUnlocked;
    public readonly string Name;


    public PXR_AchievementUpdate(AndroidJavaObject msg)
    {
      JustUnlocked = PXR_AchievementAPI.pvr_AchievementUpdate_GetJustUnlocked(msg);
      Name = PXR_AchievementAPI.pvr_AchievementUpdate_GetName(msg);
    }
  }

}
