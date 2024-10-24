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
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#elif UNITY_ANDROID
            var ray = Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
#endif
            Logger.LogInfo($"Ray: {ray.ToString()}");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                Logger.LogWarning($"Hit: {hit.collider.gameObject.name}");

                ExecuteItemCommand(hit);

                return true;
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

        private void ExecuteItemCommand(RaycastHit hit)
        {
            Identifier itemId = _controller.GetItemId(_index);
            var item = ARKitectApp.Instance.Items[itemId];

            if (item.Resource is ResourceObject)
                ARKitectApp.Instance.CommandManager.ExecuteCommand(new CommandSpawn((ResourceObject)item.Resource, hit.point, Quaternion.identity));
            else if (item.Resource is ResourceMaterial)
                ARKitectApp.Instance.CommandManager.ExecuteCommand(new CommandApplyMaterial((ResourceMaterial)item.Resource, hit.collider.gameObject));
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