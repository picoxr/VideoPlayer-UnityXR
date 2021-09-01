
using UnityEngine;

namespace Unity.XR.PXR
{
  public class PXR_CallbackRunner : MonoBehaviour
  {

    public bool IsPersistantBetweenSceneLoads = true;

    void Awake()
    {
      var existingCallbackRunner = FindObjectOfType<PXR_CallbackRunner>();
      if (existingCallbackRunner != this)
      {
        Debug.LogWarning("You only need one instance of CallbackRunner");
      }
      if (IsPersistantBetweenSceneLoads)
      {
        DontDestroyOnLoad(gameObject);
      }
    }

    void Update()
    {
      Request.RunCallbacks();
    }

    void OnApplicationQuit()
    {
      PXR_Callback.OnApplicationQuit();
    }
  }
}
