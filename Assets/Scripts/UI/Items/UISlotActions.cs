using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

using ARKitect.Core;
using ARKitect.Items;
using ARKitect.UI.Page;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI.Items
{
    public enum SlotActionType
    {
        RemoveOrClearSlot, ToggleFavorite, AddShortcut, DisplayInfo
    }

    /// <summary>
    /// Define contextual actions that can be executed and applied to a slot and its item
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Slot Actions")]
    public class UISlotActions : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private UISlot _slot;
        public UISlot Slot => _slot;

        [SerializeField]
        [ReadOnly]
        private UISlotContainer _container;

        [SerializeField]
        [ReadOnly]
        private int _slotIndex;

        private Identifier _itemId;
        public Identifier ItemId => _itemId;

        [Header("UI Components used by actions")]
        [SerializeField]
        private UIItembar _itemShortcutBar;
        [SerializeField]
        private UIPage _itemInfoPage;

        public UnityEvent _onNonToggleableActionExecuteEvent;

        private List<UISlotActionHandler> slotActionHandlerCache = new List<UISlotActionHandler>();

        private void Awake()
        {
            var slotActionHandlers = GetComponentsInChildren<UISlotActionHandler>();
            slotActionHandlerCache.AddRange(slotActionHandlers);
        }

        public void SetItemSlot(UISlot slot, Identifier itemId)
        {
            if (slot == null) { Logger.LogError("Slot is null."); return; }
            if (itemId == null) { Logger.LogError("ItemId is null."); return; }

            if (_slot != slot)
            {
                _slot = slot;
                _container = _slot.gameObject.GetComponentInParent<UISlotContainer>();
            }

            if (_slotIndex != slot.Index)
                _slotIndex = slot.Index;

            if (_itemId != itemId)
                _itemId = itemId;

            InitActionHandlers();
        }

        public void InitActionHandlers()
        {
            slotActionHandlerCache.ForEach((slotActionHandler) => slotActionHandler.Init());
        }

        public bool GetActionState(SlotActionType slotAction)
        {
            bool state = false;

            switch (slotAction)
            {
                case SlotActionType.ToggleFavorite:
                    if (_itemId != null && !_itemId.IsUndefined)
                        state = ARKitectApp.Items[_itemId].MarkedAsFavorite;
                    break;

                // Non-toggleable actions, so no state to get
                case SlotActionType.RemoveOrClearSlot:
                case SlotActionType.AddShortcut:
                case SlotActionType.DisplayInfo:
                default:
                    break;
            }

            return state;
        }

        /// <summary>
        /// Called by UISlotActionHandler
        /// </summary>
        /// <param name="slotAction"></param>
        public void Execute(SlotActionType slotAction, bool toggle)
        {
            switch (slotAction)
            {
                case SlotActionType.RemoveOrClearSlot:
                    RemoveOrClearSlot(); break;

                case SlotActionType.ToggleFavorite:
                    ToggleFavorite(toggle); break;

                case SlotActionType.AddShortcut:
                    AddShortcut(); break;

                case SlotActionType.DisplayInfo:
                    DisplayInfo(); break;

                default:
                    break;
            }
        }

        private void RemoveOrClearSlot()
        {
            if (_container != null)
            {
                _container.RemoveSlot(_slotIndex);
                _onNonToggleableActionExecuteEvent.Invoke();
            }    
        }

        private void ToggleFavorite(bool toggle)
        {
            if (_itemId != null && !_itemId.IsUndefined)
                ARKitectApp.Items[_itemId].MarkedAsFavorite = toggle;

            // Refresh library visual
            if (_container is UIItemLibrary)
            {
                var library = (UIItemLibrary)_container;
                library.FilterRefresh();
            }
        }

        private void AddShortcut()
        {
            if (_itemShortcutBar != null)
            {
                _itemShortcutBar.AddSlot(_itemId);
                _onNonToggleableActionExecuteEvent.Invoke();
            }
        }

        private void DisplayInfo()
        {
            if (_itemInfoPage == null) return;

            _onNonToggleableActionExecuteEvent.Invoke();

            UIPageContainer.Instance.Push(_itemInfoPage.Id, true, (page) =>
            {
                var itemInfo = page.gameObject.GetComponent<UIItemInfo>();
                if (itemInfo != null)
                {
                    itemInfo.ItemId = _itemId.ToString();
                }
            });
        }

    }

}