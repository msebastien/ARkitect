using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using ARKitect.UI.Modal;
using Logger = ARKitect.Core.Logger;

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
            DragIcon.Instance.SetIcon(_itemIcon.sprite);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragIcon.Instance.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DropOnItemSlot(eventData) || DropOnGround())
                Logger.LogInfo("Item Dropped successfully");
            else
                Logger.LogInfo("No item dropped");

            // Reset drag icon
            DragIcon.Instance.Clear();

            // Reset slot
            if (_isSlotPressable)
                Reset();
        }

        private bool DropOnGround()
        {
#if UNITY_EDITOR
            var ray = UnityEngine.Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#elif UNITY_ANDROID
            var ray = UnityEngine.Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
#endif
            Logger.LogInfo($"Ray: {ray.ToString()}");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                Logger.LogWarning($"Hit: {hit.collider.gameObject.name}");
                // TODO: Use Command Pattern, trigger default action of the Item (object-> spawnable or texture->appliable to geometry)
                _controller.Spawn(_index, hit.point);
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

        protected override void OpenModalWindow()
        {
            UIModalContainer.Instance.Push(_modalId, true, (modal) =>
            {
                var itemId = _controller.GetItemId(_index);

                var itemInfo = modal.gameObject.GetComponent<UIItemInfo>();
                if (itemInfo != null)
                    itemInfo.ItemId = itemId.ToString();

                var itemSlotActions = modal.gameObject.GetComponent<UISlotActions>();
                if (itemSlotActions != null)
                    itemSlotActions.SetItemSlot(this, itemId);
            });

            Logger.LogInfo($"Item '{_controller.GetItemId(_index)}' clicked");
        }

    }

}