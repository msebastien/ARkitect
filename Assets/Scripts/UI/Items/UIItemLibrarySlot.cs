using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ARKitect.Core;
using ARKitect.UI.Modal;
using ARKitect.UI.Colors;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage a item slot in library
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Slots/Item library slot")]
    public class UIItemLibrarySlot : UISlot, IPointerDownHandler
    {
        [SerializeField]
        [Tooltip("Id of the Modal window displaying info about items")]
        private string _modalId;
        public string ModalId
        {
            get => _modalId;
            set => _modalId = value;
        }

        [Header("Favorite")]
        [SerializeField]
        private Image _iconFavorite;
        [SerializeField]
        private UIColor _iconFavoriteColor;

        private void Start()
        {
            _iconFavorite.color = Color.clear;
        }

        public override void RefreshItemVisuals()
        {
            base.RefreshItemVisuals();

            if (_iconFavorite != null)
            {
                if (PrefabsManager.Items[_controller.GetItemId(_index)].MarkedAsFavorite)
                    _iconFavorite.color = _iconFavoriteColor._color;
                else
                    _iconFavorite.color = Color.clear;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
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