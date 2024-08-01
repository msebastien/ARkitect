using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ARKitect.Core;
using ARKitect.Coroutine;
using ARKitect.Items;
using ARKitect.UI.Colors;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI.Items
{
    /// <summary>
    /// Base class for creating item slots for various purposes
    /// </summary>
    public abstract class UISlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Slot")]
        [SerializeField]
        protected Image _itemIcon;

        protected ItemsController _controller;

        [SerializeField]
        protected int _index;
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        [Header("Config")]
        [SerializeField]
        protected bool _isSlotPressable = true;

        [Header("Pressable Slot: States")]
        [SerializeField]
        protected Image _slotImage;
        [SerializeField]
        protected UIColor _defaultColor;
        [SerializeField]
        protected UIColor _pressedColor;
        [SerializeField]
        protected UIColor _clickedColor;
        protected bool _isPressed = false;

        [Header("Modal window")]
        [SerializeField]
        [Tooltip("Id of the Modal window to open when clicking this slot")]
        protected string _modalId = "";
        public string ModalId
        {
            get => _modalId;
            set => _modalId = value;
        }

        protected virtual void Start()
        {
            // Slot states
            if (_defaultColor == null)
            {
                _defaultColor = (UIColor)ScriptableObject.CreateInstance(typeof(UIColor));
                _defaultColor.color = UIColors.White2;
            }

            if (_pressedColor == null)
            {
                _pressedColor = (UIColor)ScriptableObject.CreateInstance(typeof(UIColor));
                _pressedColor.color = UIColors.Blue;
            }

            if (_clickedColor == null)
            {
                _clickedColor = (UIColor)ScriptableObject.CreateInstance(typeof(UIColor));
                _clickedColor.color = UIColors.Green;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isSlotPressable && !_isPressed)
            {
                _isPressed = true;
                _slotImage.color = _pressedColor.color;
                ARKitectApp.CoroutineManager.Run(LongPressToClickRoutine());
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isSlotPressable && _isPressed)
                Reset();
        }

        public IEnumerator LongPressToClickRoutine()
        {
            if (!_isPressed) yield break;

            yield return new WaitForSecondsRealtime(0.5F);
            if (!_isPressed || _itemIcon.sprite == null) yield break; // no "clicked" state when the slot is empty
            _slotImage.color = _clickedColor.color;

            yield return new WaitForSecondsRealtime(0.15F);
            if (!_isPressed) yield break;
            OpenModalWindow();

            yield return new WaitForSecondsRealtime(0.15F);
            Reset();
        }

        /// <summary>
        /// Set the slot in its default state (not pressed) and color
        /// </summary>
        public void Reset()
        {
            _isPressed = false;
            _slotImage.color = _defaultColor.color;
        }

        protected abstract void OpenModalWindow();

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
            if (_itemIcon == null) { Logger.LogError("Icon is null"); return; }

            // If the identifier is undefined, it means the slot is empty : there is no valid item assigned to it
            if (itemDefinitionId.IsUndefined)
            {
                _itemIcon.sprite = null;
                _itemIcon.color = Color.clear;
                return;
            }

            // Get the item matching the identifier from the item database/catalog
            var itemDefinition = Core.ARKitectApp.Items[itemDefinitionId];

            if (itemDefinition == null)
            {
                _itemIcon.sprite = null;
                _itemIcon.color = Color.clear;
                Logger.LogError($"Item {itemDefinitionId} not found in catalog.");
                return;
            }

            _itemIcon.sprite = itemDefinition.Icon;
            _itemIcon.color = Color.white;
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