/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using UnityEngine;

namespace Unity.XR.PXR
{
    public class LoadingRatation : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.Rotate(new Vector3(0, 0, -4));
        }
    }
}

