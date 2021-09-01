/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_FoveationRendering
    {
        private static PXR_FoveationRendering instance = null;
        public static PXR_FoveationRendering Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PXR_FoveationRendering();
                }

                return instance;
            }
        }

        /// <summary>
        /// Set foveated rendering level
        /// </summary>
        /// <param name="level"></param>
        public static void SetFoveationLevel(FoveationLevel level)
        {
            PXR_Plugin.Render.UPxr_SetFoveationLevel(level);
        }

        /// <summary>
        /// Get current foveated rendering level
        /// </summary>
        /// <returns></returns>
        public static FoveationLevel GetFoveationLevel()
        {
            return PXR_Plugin.Render.UPxr_GetFoveationLevel();
        }

        /// <summary>
        /// Set current foveated rendering parameters
        /// </summary>
        /// <param name="foveationGainX"></param>
        /// <param name="foveationGainY"></param>
        /// <param name="foveationArea"></param>
        /// <param name="foveationMinimum"></param>
        public static void SetFoveationParameters(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
        {
            PXR_Plugin.Render.UPxr_SetFoveationParameters(foveationGainX, foveationGainY, foveationArea, foveationMinimum);
        }
    }
}