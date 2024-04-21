using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ARKitect.Items;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Instantiate slots and assign slots to items
    /// </summary>
    public abstract class UISlotContainer : MonoBehaviour
    {
        [Header("Controller")]
        [SerializeField]
        protected ItemsController itemsController;

        [Header("Slots")]
        [SerializeField]
        protected Transform slotsParent;
        [SerializeField]
        protected UISlot slotPrefab;

        // Keep references to slots internally to avoid looking for them
        // with FindObjectOfType<T>(), which is terribly inefficient, or GetComponentsInChildren<T>()
        protected List<UISlot> slotCache = new List<UISlot>();

        protected virtual void Awake()
        {
            if (itemsController == null)
                itemsController = GetComponent<ItemsController>();
        }

        /// <summary>
        /// Initialize all the slots and fill them with items from the catalog.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Instantiate specified slot prefab
        /// </summary>
        protected abstract void InstantiateSlots();

        /// <summary>
        /// Assign the corresponding index for the ItemsController to the slot
        /// </summary>
        protected virtual void BindSlots()
        {
            int i = 0;
            foreach (var slot in slotCache)
            {
                slot.Bind(itemsController, i);
                i++;
            }
        }

        /// <summary>
        /// Refresh slots visual (item icon)
        /// </summary>
        protected virtual void RefreshSlots()
        {
            foreach (var slot in slotCache)
            {
                slot.RefreshItemVisuals();
            }
        }

        /// <summary>
        /// Enable or disable the slot at the specified index
        /// </summary>
        /// <param name="index">Slot index</param>
        /// <param name="enable">True to enable, false to disable the slot</param>
        protected void ToggleSlot(int index, bool enable)
        {
            slotCache[index].gameObject.SetActive(enable);
        }
    }

}
