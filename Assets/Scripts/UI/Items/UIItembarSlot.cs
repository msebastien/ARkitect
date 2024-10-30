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
                if (DropOnItemSlot(eventData) || DropOnGround(eventData))
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

        private bool DropOnGround(PointerEventData eventData)
        {
            var screenPos = eventData.position;
            var ray = Camera.main.ScreenPointToRay(screenPos);
            Logger.LogInfo($"Ray: {ray.ToString()}");

#if UNITY_EDITOR
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);
#endif
            // Get Item definition
            Identifier itemId = _controller.GetItemId(_index);
            var item = ARKitectApp.Instance.Items[itemId];

            // Check if the the game object colliding with the ray is a valid one (specified in the layer mask)
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, GetRaycastMask(item), QueryTriggerInteraction.Collide))
            {
                Logger.LogWarning($"Hit: {hit.collider.gameObject.name}");
                ExecuteItemCommand(item, hit, screenPos);
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

        private void ExecuteItemCommand(Item item, RaycastHit hit, Vector2 screenPos)
        {
            if (item.Resource is ResourceObject)
                ARKitectApp.Instance.CommandManager.ExecuteCommand(new CommandSpawn((ResourceObject)item.Resource, hit.point, Quaternion.identity));
            else if (item.Resource is ResourceMaterial)
                ARKitectApp.Instance.CommandManager.ExecuteCommand(new CommandApplyMaterial((ResourceMaterial)item.Resource, hit.collider.gameObject, screenPos));
        }

        private int GetRaycastMask(Item item)
        {
            string gridLayer = LayerMask.LayerToName((int)Layers.GRID);
            string objectLayer = LayerMask.LayerToName((int)Layers.BUILDING_OBJECT);

            int mask = Physics.DefaultRaycastLayers;
            if (item.Resource is ResourceObject)
                mask = LayerMask.GetMask(new string[] { gridLayer, objectLayer });
            else if (item.Resource is ResourceMaterial)
                mask = LayerMask.GetMask(new string[] { objectLayer });

            return mask;
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