using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ARKitect.Items;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage the number of instanciated slots in item bar, and assign slots to default items, if desired
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Item bar")]
    public class UIItembar : UISlotContainer
    {
        // Total number of slots instantiated
        public const byte MaxSlotCount = 7;

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
        private bool loadDefaultItems = true;

        protected override void Awake()
        {
            base.Awake();

            itemsController.Capacity = MaxSlotCount;
            itemsController.UsedCapacity = numSlots;
            currNumSlots = numSlots;
        }

        /// <summary>
        /// Initialize all the slots
        /// This method is called when all the items have been loaded in the catalog in the PrefabsManager.
        /// Subscribed to PrefabsManager.OnItemCatalogLoaded() event.
        /// </summary>
        public override void Init()
        {
            InstantiateSlots(MaxSlotCount);
            BindSlots();

            if (loadDefaultItems) itemsController.LoadDefaultItems();
            RefreshSlots();
            Logger.LogInfo("Item bar loaded!");
        }

        // Update is called once per frame
        private void Update()
        {
            // Check if the number of active slots has changed
            if (numSlots != currNumSlots)
            {
                UpdateSlots();
                itemsController.UsedCapacity = numSlots;
                currNumSlots = numSlots;
            }
        }

        /// <summary>
        /// Assign the corresponding index for the ItemsController to the slot
        /// </summary>
        protected override void BindSlots()
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

    }

}