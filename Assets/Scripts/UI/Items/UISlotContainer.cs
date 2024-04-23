using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ARKitect.Items;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Instantiate slots and assign slots to items
    /// </summary>
    [RequireComponent(typeof(ItemsController))]
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
        /// Initialize all the slots
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Instantiate a slot prefab
        /// </summary>
        protected void InstantiateSlot()
        {
            var slot = Instantiate(slotPrefab, slotsParent);
            slotCache.Add(slot);
        }

        /// <summary>
        /// Instantiate the specified count of slot prefab
        /// </summary>
        /// <param name="count">The number of slots to instantiate</param>
        protected void InstantiateSlots(int count)
        {
            if (count < 0) { Logger.LogError("Count is negative."); return; }
            for (int i = 0; i < count; i++)
            {
                InstantiateSlot();
            }
        }

        /// <summary>
        /// Assign the corresponding index from the ItemsController to the slots
        /// </summary>
        protected virtual void BindSlots()
        {
            int i = 0;
            foreach (var slot in slotCache)
            {
                BindSlot(slot, i);
                i++;
            }
        }

        /// <summary>
        /// Assign the corresponding index from the ItemsController to the specified slot
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="index"></param>
        protected void BindSlot(UISlot slot, int index)
        {
            if (index < 0) { Logger.LogError("Index is negative."); return; }
            if (index > slotCache.Count - 1) { Logger.LogError("Index is too big."); return; }

            slot.Bind(itemsController, index);
        }

        /// <summary>
        /// Refresh the visual of the specified slot
        /// </summary>
        /// <param name="index">Slot index</param>
        public void RefreshSlot(int index)
        {
            if (index < 0) { Logger.LogError("Index is negative."); return; }
            if (index > slotCache.Count - 1) { Logger.LogError("Index is too big."); return; }

            slotCache[index].RefreshItemVisuals();
        }

        /// <summary>
        /// Refresh slots visual (item icon)
        /// </summary>
        public void RefreshSlots()
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
            if (index < 0) { Logger.LogError("Index is negative."); return; }
            if (index > slotCache.Count - 1) { Logger.LogError("Index is too big."); return; }

            slotCache[index].gameObject.SetActive(enable);
        }

        /// <summary>
        /// Check whether this specified item Id exists in at least one slot
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>true, if it exists, else false.</returns>
        public bool Exists(Identifier itemId)
        {
            return itemsController.Exists(itemId);
        }

        /// <summary>
        /// Add slot containing the specified item Id.
        /// If an empty or invalid identifier is specified, the slot will be empty.
        /// If the Items Controller is preallocated, the item will be added
        /// to the first empty slot or the last one.
        /// </summary>
        /// <param name="itemId"></param>
        public void AddSlot(Identifier itemId)
        {
            if (itemId == null) { Logger.LogError("ItemId is null."); return; }

            int index = itemsController.Add(itemId);
            if (!itemsController.Preallocated)
            {
                InstantiateSlot();
                BindSlot(slotCache[index], index);
            }

            RefreshSlot(index);
        }

        /// <summary>
        /// Remove the first slot containing the specified item Id
        /// </summary>
        /// <param name="item">Item Identifier of the first slot containing it to remove</param>
        public void RemoveSlot(Identifier itemId)
        {
            if (itemId == null) { Logger.LogError("ItemId is null."); return; }

            int index = itemsController.FindIndex(itemId);
            RemoveSlot(index);
        }

        /// <summary>
        /// Remove all slots containing the specified item Id
        /// </summary>
        /// <param name="itemId">Item Identifier of the slots containing it to remove</param>
        public void RemoveSlots(Identifier itemId)
        {
            if (itemId == null) { Logger.LogError("ItemId is null."); return; }

            itemsController.FindAllIndex(itemId).ForEach((index) =>
            {
                RemoveSlot(index);
            });
        }

        /// <summary>
        /// Remove a slot and update slot indices accordingly
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveSlot(int index)
        {
            if (index < 0) { Logger.LogError("Index is negative."); return; }
            if (index > slotCache.Count - 1) { Logger.LogError("Index is too big."); return; }

            itemsController.Remove(index);
            if (!itemsController.Preallocated)
            {
                var slotToRemove = slotCache[index];

                slotCache.RemoveAt(index);
                Destroy(slotToRemove.gameObject);
                BindSlots();
                RefreshSlots();
            }
            else
            {
                RefreshSlot(index);
            }

        }
    }

}
