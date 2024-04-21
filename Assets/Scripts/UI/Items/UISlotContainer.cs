using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ARKitect.Items;

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
        /// Initialize all the slots and fill them with items from the catalog.
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
            slot.Bind(itemsController, index);
        }

        /// <summary>
        /// Refresh the visual of the specified slot
        /// </summary>
        /// <param name="index">Slot index</param>
        public void RefreshSlot(int index)
        {
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
            slotCache[index].gameObject.SetActive(enable);
        }

        /// <summary>
        /// Check whether this specified item Id exists in at least one slot
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>true, if it exists, else false.</returns>
        public bool IsItemExistInSlots(Identifier itemId)
        {
            return itemsController.ItemDefinitionsInSlots.Find((id) => id == itemId) != null;
        }

        /// <summary>
        /// Add slot containing the specified item Id.
        /// If an empty or invalid identifier is specified, the slot will be empty.
        /// </summary>
        /// <param name="itemId"></param>
        public void AddSlot(Identifier itemId)
        {
            itemsController.ItemDefinitionsInSlots.Add(itemId);
            InstantiateSlot();
            
            BindSlot(slotCache[slotCache.Count - 1], slotCache.Count - 1);
            RefreshSlot(slotCache.Count - 1);
        }

        /// <summary>
        /// Remove the first slot containing the specified item Id
        /// </summary>
        /// <param name="item">Item Identifier of the first slot containing it to remove</param>
        public void RemoveSlot(Identifier itemId)
        {
            int index = itemsController.ItemDefinitionsInSlots.FindIndex((id) => id == itemId);
            RemoveSlot(index);
        }

        /// <summary>
        /// Remove all slots containing the specified item Id
        /// </summary>
        /// <param name="itemId">Item Identifier of the slots containing it to remove</param>
        public void RemoveSlots(Identifier itemId)
        {
            for (int i = 0; i < itemsController.ItemDefinitionsInSlots.Count; i++)
            {
                if (itemsController.ItemDefinitionsInSlots[i] == itemId)
                {
                    RemoveSlot(i);
                }
            }
        }

        /// <summary>
        /// Remove a slot and update slot indices accordingly
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveSlot(int index)
        {
            var slotToRemove = slotCache[index];
            
            itemsController.Remove(index);
            if(!itemsController.Preallocated)
            {
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
