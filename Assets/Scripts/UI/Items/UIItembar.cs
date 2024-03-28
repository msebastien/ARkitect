using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ARKitect.Items;
using ARKitect.UI.Events;



namespace ARKitect.UI.Events
{
    [System.Serializable]
    public class OnItembarSlotCountChanged : UnityEvent<byte> { }
}

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage the number of instanciated slots in item bar, and assign slots to default items, if desired
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Item bar")]
    [RequireComponent(typeof(ItemsController))]
    public class UIItembar : MonoBehaviour
    {
        [Header("Controller")]
        [SerializeField]
        private ItemsController itemsController;

        // Total number of slots instantiated
        public const byte MaxSlotCount = 8;

        [Header("Config")]
        [Range(1, MaxSlotCount)]
        [Tooltip("Number of active/used slots")]
        [SerializeField]
        private byte numSlots = 5;
        public byte SlotCount
        {
            get { return numSlots; }
            set { numSlots = (byte)Mathf.Clamp(value, 1, MaxSlotCount); }
        }
        private byte currNumSlots;

        [SerializeField]
        private Transform slotsParent;
        [SerializeField]
        private UIItemBarSlot slotPrefab;

        [Header("Event")]
        public OnItembarSlotCountChanged slotCountChangedEvent;

        // Keep references to slots internally to avoid looking for them
        // with FindObjectOfType<T>(), which is terribly inefficient, or GetComponentsInChildren<T>()
        private List<UIItemBarSlot> slotCache = new List<UIItemBarSlot>();

        private void Awake()
        {
            if (itemsController == null) itemsController = GetComponent<ItemsController>();
            itemsController.capacity = MaxSlotCount;
            currNumSlots = numSlots;
        }

        // Start is called before the first frame update
        void Start()
        {
            InstantiateSlots();
            BindSlots();
        }

        // Update is called once per frame
        void Update()
        {
            // Check if the number of active slots has changed
            if (numSlots != currNumSlots)
            {
                slotCountChangedEvent.Invoke(numSlots);
                UpdateSlots();
                currNumSlots = numSlots;
            }
        }

        /// <summary>
        /// Instantiate specified slot prefab
        /// </summary>
        private void InstantiateSlots()
        {
            for (byte i = 0; i < MaxSlotCount; i++)
            {
                var slot = Instantiate(slotPrefab, slotsParent);
                slotCache.Add(slot);
            }
        }

        /// <summary>
        /// Assign the corresponding index for the ItemsController to the slot
        /// </summary>
        private void BindSlots()
        {
            byte i = 0;
            foreach (var slot in slotCache)
            {
                slot.Bind(itemsController, i);
                if (i >= numSlots) ToggleSlot(i, false);
                i++;
            }
        }

        /// <summary>
        /// Toggle slots based on the number of active slots
        /// </summary>
        private void UpdateSlots()
        {
            for (byte i = 0; i < MaxSlotCount; i++)
            {
                if (i < numSlots)
                {
                    ToggleSlot(i, true);
                }
                else
                {
                    ToggleSlot(i, false);
                }
            }
        }

        /// <summary>
        /// Enable or disable the slot at the specified index
        /// </summary>
        /// <param name="index">Slot index</param>
        /// <param name="enable">True to enable, false to disable the slot</param>
        private void ToggleSlot(byte index, bool enable)
        {
            slotCache[index].gameObject.SetActive(enable);
        }
    }

}