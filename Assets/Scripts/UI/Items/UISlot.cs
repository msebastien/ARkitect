using UnityEngine;
using UnityEngine.UI;

using ARKitect.Core;
using ARKitect.Items;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Base class for creating item slots for various purposes
    /// </summary>
    public abstract class UISlot : MonoBehaviour
    {
        [SerializeField]
        protected Image icon;

        protected ItemsController controller;

        public int index;

        /// <summary>
        /// Assign an item from an item controller to this slot
        /// </summary>
        /// <param name="controller">Items controller managing the assignment of items to slots</param>
        /// <param name="index">Slot index</param>
        public void Bind(ItemsController controller, int index)
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

        public void RemoveItemDefinition()
        {
            controller.Remove(index);
            RefreshItemVisuals();
        }
    }

}