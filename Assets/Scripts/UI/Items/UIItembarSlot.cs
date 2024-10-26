using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using ARKitect.UI.Modal;
using Logger = ARKitect.Core.Logger;
using ARKitect.Items;
using ARKitect.Commands;
using ARKitect.Core;
using ARKitect.Items.Resource;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage a slot with draggable item in shortcut bar
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Slots/Item bar slot")]
    public class UIItemBarSlot : UISlot, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isSlotPressable) _isPressed = false; // slot is not pressed but its item is dragged

            // Item icon sprite is null when the Item Id is undefined, which means the slot is empty,
            // so disable drag and drop
            if (_itemIcon.sprite != null)
                DragIcon.Instance.SetIcon(_itemIcon.sprite);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_itemIcon.sprite != null)
                DragIcon.Instance.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_itemIcon.sprite != null)
            {
                // Check if item dropped
                if (DropOnItemSlot(eventData) || DropOnGround())
                    Logger.LogInfo("Item Dropped successfully");
                else
                    Logger.LogInfo("No item dropped");

                // Reset drag icon
                DragIcon.Instance.Clear();
            }

            // Reset slot
            if (_isSlotPressable)
                Reset();
        }

        private bool DropOnGround()
        {
#if UNITY_EDITOR
            var screenPos = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(screenPos);
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
#elif UNITY_ANDROID
            var screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(screenPos);
#endif
            Logger.LogInfo($"Ray: {ray.ToString()}");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                Logger.LogWarning($"Hit: {hit.collider.gameObject.name}");

                return ExecuteItemCommand(screenPos, hit);
            }
            return false;
        }

        private bool DropOnItemSlot(PointerEventData eventData)
        {
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);

            foreach (var hit in hits)
            {
                var droppedSlot = hit.gameObject.GetComponent<UIItemBarSlot>();

                if (droppedSlot)
                {
                    Logger.LogInfo($"Drag End {droppedSlot?.name}");
                    Swap(this, droppedSlot);

                    return true;
                }
            }

            return false;
        }

        private bool ExecuteItemCommand(Vector2 screenPos, RaycastHit hit)
        {
            Identifier itemId = _controller.GetItemId(_index);
            var item = ARKitectApp.Instance.Items[itemId];
            bool ret = false;

            if (item.Resource is ResourceObject)
            {
                ret = true;
                ARKitectApp.Instance.CommandManager.ExecuteCommand(new CommandSpawn((ResourceObject)item.Resource, hit.point, Quaternion.identity));             
            }          
            else if (item.Resource is ResourceMaterial)
            {
                ret = hit.collider.gameObject.layer != 3; // Is the hit object part of layer 3 ? (a layer dedicated to the Editor grid)
                if (ret) 
                    ARKitectApp.Instance.CommandManager.ExecuteCommand(new CommandApplyMaterial((ResourceMaterial)item.Resource, hit.collider.gameObject, screenPos));           
            }

            return ret; // Whether a command has been executed or not
        }

        protected override void OpenModalWindow()
        {
            var itemId = _controller.GetItemId(_index);

            if (itemId.IsUndefined) return; // If the slot is empty, don't open a modal window

            UIModalContainer.Instance.Push(_modalId, true, (modal) =>
            {
                var itemInfo = modal.gameObject.GetComponent<UIItemInfo>();
                if (itemInfo != null)
                    itemInfo.ItemId = itemId.ToString();

                var itemSlotActions = modal.gameObject.GetComponent<UISlotActions>();
                if (itemSlotActions != null)
                    itemSlotActions.SetItemSlot(this, itemId);
            });

            Logger.LogInfo($"Item '{itemId}' clicked");
        }

    }

}