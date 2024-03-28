using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


using ARKitect.Items;
using ARKitect.Core;
using Logger = ARKitect.Core.Logger;



namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage a item slot in shortcut bar
    /// </summary>
    public class UIItemBarSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Image icon;

        private ItemsController controller;

        public byte index;

        /// <summary>
        /// Assign an item from an item controller to this slot
        /// </summary>
        /// <param name="controller">Items controller managing the assignment of items to slots</param>
        /// <param name="index">Slot index</param>
        public void Bind(ItemsController controller, byte index)
        {
            this.index = index;
            this.controller = controller;
            Identifier itemDefinitionId = controller.ItemDefinitionsInSlots.Count > index ? controller.ItemDefinitionsInSlots[index] : new Identifier();
            ShowItemDefinition(itemDefinitionId);
        }

        /// <summary>
        /// Refresh the slot visual (the icon of the assigned item)
        /// </summary>
        public void RefreshItemVisuals()
        {
            Identifier itemDefinitionId = controller.ItemDefinitionsInSlots.Count > index ? controller.ItemDefinitionsInSlots[index] : new Identifier();
            ShowItemDefinition(itemDefinitionId);
        }

        /// <summary>
        /// Display item in slot
        /// </summary>
        /// <param name="itemDefinitionId">Item identifier</param>
        public void ShowItemDefinition(Identifier itemDefinitionId)
        {
            // If no Image component is referenced
            if (icon == null) { Logger.LogError("Icon is null"); return; }

            // If the identifier is undefined, it means the slot is empty : there is no valid item assigned to it
            if (itemDefinitionId.IsUndefined)
            {
                icon.sprite = null;
                Logger.LogWarning($"Itembar Slot (idx:{index}) has an undefined item definition. ({itemDefinitionId})");
                return;
            }

            // Get the item matching the identifier from the item database/catalog
            var itemDefinition = PrefabsManager.Instance.Items[itemDefinitionId];

            if (itemDefinition == null) { Logger.LogError($"Item {itemDefinitionId} not found in catalog."); }

            icon.sprite = itemDefinition.Icon;
        }

        /// <summary>
        /// Remove the item definition by replacing it with an undefined identifier
        /// </summary>
        public void RemoveItemDefinition()
        {
            controller.Remove(index);
            RefreshItemVisuals();
        }

        public void Swap(UIItemBarSlot slot1, UIItemBarSlot slot2)
        {
            controller.Swap(slot1.index, slot2.index);
            RefreshItemVisuals();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            DragIcon.Instance.SetSprite(icon.sprite);
            DragIcon.Instance.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragIcon.Instance.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DropOnItemSlot(eventData) ||
                DropOnGround())
            {
                DragIcon.Instance.SetSprite(null);
                DragIcon.Instance.gameObject.SetActive(false);
            }
        }

        private bool DropOnGround()
        {
            var ray = UnityEngine.Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
            if (Physics.Raycast(ray, out var hit))
            {
                // TODO: Use Command Pattern, trigger default action of the Item (object-> spawnable or texture->appliable to geometry)
                // TODO: Spawn Item's prefab
                //controller.Spawn(hit.point);
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





    }

}