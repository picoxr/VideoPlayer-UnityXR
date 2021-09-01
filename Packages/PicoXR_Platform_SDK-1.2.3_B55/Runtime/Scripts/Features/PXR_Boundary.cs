/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_Boundary
    {
        /// <summary>
        /// Set boundary system visibility to be the specified value.
        /// Note:
        ///     The actual visibility of boundary can be overridden by the system(e.g. proximity trigger) or user setting(e.g. disable boundary system).
        /// </summary>
        /// <param name="value"></param>
        public static void SetVisible(bool value)
        {
            PXR_Plugin.Boundary.UPxr_SetBoundaryVisible(value);
        }

        /// <summary>
        /// Get boundary system visibility to be the specified value.
        /// Note:
        ///     The actual visibility of boundary can be overridden by the system(e.g. proximity trigger) or user setting(e.g. disable boundary system).
        /// </summary>
        /// <param name="value"></param>
        public static bool GetVisible()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryVisible();
        }

        /// <summary>
        /// Returns true if the boundary system is currently configured with valid boundary data.
        /// </summary>
        /// <returns></returns>
        public static bool GetConfigured()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryConfigured();
        }

        /// Return result of whether Safety Boundary is enabled.
        /// </summary>
        /// <returns></returns>
        public static bool GetEnabled()
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryEnabled();
        }

        /// <summary>
        ///  Returns the result of testing a tracked node against the specified boundary type
        /// </summary>
        /// <param name="node"></param>
        /// <param name="boundaryType"></param>
        /// <returns></returns>
        public static BoundaryTestResult TestNode(BoundaryTrackingNode node, BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_TestBoundaryNode(node, boundaryType);
        }

        /// <summary>
        /// Returns the result of testing a 3d point against the specified boundary type.
        /// </summary>
        /// <param name="point">the coordinate of the point</param>
        /// <param name="boundaryType">OuterBoundary or PlayArea</param>
        /// <returns></returns>
        public static BoundaryTestResult TestPoint(Vector3 point, BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_TestBoundaryPoint(point, boundaryType);
        }

        /// <summary>
        /// Return the boundary geometry points
        /// </summary>
        /// <param name="boundaryType">OuterBoundary or PlayArea</param>
        /// <returns>Boundary geometry point-collection num</returns>
        public static Vector3[] GetGeometry(BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryGeometry(boundaryType);
        }

        /// <summary>
        /// Get the size of self-defined safety boundary PlayArea
        /// </summary>
        /// <param name="boundaryType"></param>
        /// <returns></returns>
        public static Vector3 GetDimensions(BoundaryType boundaryType)
        {
            return PXR_Plugin.Boundary.UPxr_GetBoundaryDimensions(boundaryType);
        }

        /// <summary>
        /// Get the camera image of Neo 2 and set it as the environmental background
        /// </summary>
        /// <param name="value">whether SeeThrough is enabled or not, true enabled, false disenabled</param>
        public static void EnableSeeThroughManual(bool value)
        {
            PXR_Plugin.Boundary.UPxr_EnableSeeThroughManual(value);
        }

        /// <summary>
        /// Get Boundary Dialog State
        /// </summary>
        /// <returns>NothingDialog = -1,GobackDialog = 0,ToofarDialog = 1,LostDialog = 2,LostNoReason = 3,LostCamera = 4,LostHighLight = 5,LostLowLight = 6,LostLowFeatureCount = 7,LostReLocation = 8</returns>
        public static int GetDialogState()
        {
            return PXR_Plugin.Boundary.UPxr_GetDialogState();
        }
    }
}


