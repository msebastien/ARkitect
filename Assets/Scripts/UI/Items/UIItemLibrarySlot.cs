using UnityEngine;
using UnityEngine.EventSystems;

using ARKitect.UI.Modal;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            UIModalContainer.Instance.Push(_modalId, true, (modal) =>
            {
                var itemInfo = modal.gameObject.GetComponent<UIItemInfo>();
                if (itemInfo != null)
                {
                    itemInfo.ItemId = controller.GetItemId(index).ToString();
                }
            });

            Logger.LogInfo($"Item '{controller.GetItemId(index)}' clicked");
        }
    }

}