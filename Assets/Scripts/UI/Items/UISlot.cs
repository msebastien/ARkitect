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
        protected Image _icon;

        protected ItemsController _controller;

        [SerializeField]
        protected int _index;
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        /// <summary>
        /// Assign an item from an item controller to this slot
        /// </summary>
        /// <param name="controller">Items controller managing the assignment of items to slots</param>
        /// <param name="index">Slot index</param>
        public void Bind(ItemsController controller, int index)
        {
            _index = index;
            _controller = controller;
            Identifier itemDefinitionId = controller.Count > _index ? controller.GetItemId(_index) : new Identifier();
            ShowItemDefinition(itemDefinitionId);
        }

        /// <summary>
        /// Refresh the slot visual (the icon of the assigned item)
        /// </summary>
        public virtual void RefreshItemVisuals()
        {
            Identifier itemDefinitionId = _controller.Count > _index ? _controller.GetItemId(_index) : new Identifier();
            ShowItemDefinition(itemDefinitionId);
        }

        /// <summary>
        /// Display item in slot
        /// </summary>
        /// <param name="itemDefinitionId">Item identifier</param>
        public void ShowItemDefinition(Identifier itemDefinitionId)
        {
            // If no Image component is referenced
            if (_icon == null) { Logger.LogError("Icon is null"); return; }

            // If the identifier is undefined, it means the slot is empty : there is no valid item assigned to it
            if (itemDefinitionId.IsUndefined)
            {
                _icon.sprite = null;
                _icon.color = Color.clear;
                return;
            }

            // Get the item matching the identifier from the item database/catalog
            var itemDefinition = PrefabsManager.Items[itemDefinitionId];

            if (itemDefinition == null)
            {
                _icon.sprite = null;
                _icon.color = Color.clear;
                Logger.LogError($"Item {itemDefinitionId} not found in catalog.");
                return;
            }

            _icon.sprite = itemDefinition.Icon;
            _icon.color = Color.white;
        }

        /// <summary>
        /// Swap the items in slot1 and slot2
        /// </summary>
        /// <param name="slot1"></param>
        /// <param name="slot2"></param>
        protected void Swap(UISlot slot1, UISlot slot2)
        {
            _controller.Swap(slot1._index, slot2._index);
            slot1.RefreshItemVisuals();
            slot2.RefreshItemVisuals();
        }

    }

}