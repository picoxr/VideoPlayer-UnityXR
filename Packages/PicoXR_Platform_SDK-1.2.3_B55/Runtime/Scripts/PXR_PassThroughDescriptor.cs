/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_PassThroughDescriptor : SubsystemDescriptor<PXR_PassThroughSystem>
    {
        public struct Cinfo : IEquatable<Cinfo>
        {
            public string id { get; set; }
            public Type ImplementaionType { get; set; }


            public bool Equals(Cinfo other)
            {
                return id == other.id ||ImplementaionType == other.ImplementaionType;
            }
        }
        internal static PXR_PassThroughDescriptor Create(Cinfo info)
        {
            return new PXR_PassThroughDescriptor(info);
        }

        public PXR_PassThroughDescriptor(Cinfo info)
        {
            id = info.id;
            subsystemImplementationType = info.ImplementaionType;
        }
    }
}
