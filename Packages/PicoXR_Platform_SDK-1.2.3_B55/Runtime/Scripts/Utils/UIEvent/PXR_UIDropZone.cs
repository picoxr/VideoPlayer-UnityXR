/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.XR.PXR
{
    public class PXR_UIDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected PXR_UIDraggableItem droppableItem;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag)
            {
                var dragItem = eventData.pointerDrag.GetComponent<PXR_UIDraggableItem>();
                if (dragItem && dragItem.restrictToDropZone)
                {
                    dragItem.validDropZone = gameObject;
                    droppableItem = dragItem;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (droppableItem)
            {
                droppableItem.validDropZone = null;
            }
            droppableItem = null;
        }

    }
}