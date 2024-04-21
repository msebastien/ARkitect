using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using ARKitect.Core;
using ARKitect.Items;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Instantiate slots and assign slots to items in the building parts library
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Item Library")]
    public class UIItemLibrary : UISlotContainer
    {
        [Header("Buttons")]
        [SerializeField]
        [Tooltip("Parent object containing filter buttons as children")]
        private Transform filterButtonsParent;

        [SerializeField]
        [Tooltip("Default selected filter button")]
        private GameObject defaultSelectedButton;
        private List<Button> filterButtons = new List<Button>();
        private GameObject selectedFilterButton;

        protected override void Awake()
        {
            base.Awake();

            var buttons = filterButtonsParent.GetComponentsInChildren<Button>();
            filterButtons.AddRange(buttons);
        }

        /// <summary>
        /// Initialize all the slots and fill them with items from the catalog.
        /// This method is called when all the items have been loaded in the catalog in the PrefabsManager.
        /// Subscribed to PrefabsManager.OnItemCatalogLoaded() event.
        /// </summary>
        public override void Init()
        {
            InstantiateSlots(PrefabsManager.Instance.Items.Count);
            BindSlots();

            FillSlots();
            RefreshSlots();

            Logger.LogInfo("Item Library loaded!");
        }

        
        private void OnEnable()
        {
            // Set Current Selected button
            if (defaultSelectedButton != null && selectedFilterButton == null)
                EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            else
                EventSystem.current.SetSelectedGameObject(selectedFilterButton);

            // Init filter buttons event listeners
            bool initAllFilter = true;
            bool initFavoriteFilter = true;

            foreach (var filterButton in filterButtons)
            {
                // Register selected button event handler
                filterButton.onClick.AddListener(() =>
                {
                    selectedFilterButton = filterButton.gameObject;
                    Logger.LogInfo($"Set selected Button to {selectedFilterButton.name}");
                });

                // All
                if (initAllFilter && filterButton.name.Contains("All"))
                {
                    filterButton.onClick.AddListener(DisplayAll);
                    initAllFilter = false;
                    continue;
                }

                // Favorites
                if (initFavoriteFilter && filterButton.name.Contains("Favorite"))
                {
                    filterButton.onClick.AddListener(FilterByFavorites);
                    initFavoriteFilter = false;
                    continue;
                }

                // Categories
                var splitName = filterButton.name.Split('_');
                if (splitName.Length > 1 && Enum.TryParse(splitName[1], out ItemCategory category))
                {
                    filterButton.onClick.AddListener(() =>
                    {
                        FilterByCategory(category);
                    });
                } 
                else
                {
                    Logger.LogError($"Failed to parse Item Category with name '{filterButton.name}' !");
                }
            }
        }

        private void OnDisable()
        {
            foreach (var filterButton in filterButtons)
            {
                filterButton.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// Fill slots with items from the item catalog
        /// </summary>
        private void FillSlots()
        {
            foreach (var itemId in PrefabsManager.Instance.Items.Keys)
            {
                itemsController.ItemDefinitionsInSlots.Add(itemId);
            }
        }

        public void DisplayAll()
        {
            for (int i = 0; i < itemsController.ItemDefinitionsInSlots.Count; i++)
            {
                ToggleSlot(i, true);
            }
        }

        /// <summary>
        /// Display only items belonging to a specified category. Other items will be hidden.
        /// </summary>
        /// <param name="category"></param>
        public void FilterByCategory(ItemCategory category)
        {
            int i = 0;
            foreach (var itemId in itemsController.ItemDefinitionsInSlots)
            {
                if (PrefabsManager.Instance.Items[itemId].Category != category)
                    ToggleSlot(i, false);
                else
                    ToggleSlot(i, true);
                
                i++;
            }
        }

        /// <summary>
        /// Display only items marked as favorites. Other items will be hidden.
        /// </summary>
        /// <param name="category"></param>
        public void FilterByFavorites()
        {
            int i = 0;
            foreach (var itemId in itemsController.ItemDefinitionsInSlots)
            {
                if (!PrefabsManager.Instance.Items[itemId].MarkedAsFavorite)
                    ToggleSlot(i, false);
                else
                    ToggleSlot(i, true);

                i++;
            }
        }

    }

}