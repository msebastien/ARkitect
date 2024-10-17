using System.Collections.Generic;
using System;
using UnityEngine;

using ARKitect.Core;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Items
{
    /// <summary>
    /// Assign item identifiers to slot indices
    /// </summary>
    public class ItemsController : MonoBehaviour
    {
        private List<Identifier> itemDefinitions;

        /// <summary>
        /// Number of item slots
        /// </summary>
        public int Count => itemDefinitions.Count;

        [Header("Config")]
        [Tooltip("Whether to preallocate capacity for the item definitions list")]
        [SerializeField]
        private bool preallocated = false;
        public bool Preallocated => preallocated;

        /// <summary>
        /// Total Capacity to preallocate for the item definitions list
        /// </summary>
        public int Capacity { get; set; } = 7;

        /// <summary>
        /// Actually used capacity for displayed slots
        /// </summary>
        public int UsedCapacity { get; set; } = 5;

        [SerializeField]
        private string[] defaultItems = new string[]
        {
            "arkitect:foundation_wall", "arkitect:wall", "arkitect:wall_corner",
            "arkitect:wall_frontdoor", "arkitect:wall_interior"
        };

        private void Awake()
        {
            // Initialize the Item definitions list with a predefined/pre-allocated capacity.
            if (preallocated)
            {
                Preallocate();
            }
            else // Initialize an empty Item definition list
            {
                itemDefinitions = new List<Identifier>();
            }
        }

        private void Preallocate()
        {
            itemDefinitions = new List<Identifier>(Capacity);
            for (int i = 0; i < Capacity; i++)
            {
                itemDefinitions.Add(new Identifier());
            }
        }

        public void LoadDefaultItems()
        {
            for (int i = 0; i < defaultItems.Length; i++)
            {
                var item = new Identifier(defaultItems[i]);
                if (preallocated)
                {
                    if (i > Count - 1) break;
                    itemDefinitions[i] = item;
                }
                else
                {
                    itemDefinitions.Add(item);
                }
            }
        }

        public Identifier GetItemId(int slot)
        {
            try
            {
                return itemDefinitions[slot];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.LogException(e);
                return new Identifier();
            }
        }

        /// <summary>
        /// Find the index of the first slot containing the specified Item Id.
        /// </summary>
        /// <param name="itemId">Identifier of the item to find</param>
        /// <returns>Index of the first slot containing the specified Item Id, else -1.</returns>
        public int FindIndex(Identifier itemId)
        {
            if (itemId == null) { Logger.LogError("ItemId is null."); return -1; }

            return itemDefinitions.FindIndex((id) => id == itemId);
        }

        /// <summary>
        /// Find indices of all the item occurrences
        /// </summary>
        /// <param name="itemId">Identifier of the item to find</param>
        /// <returns>Indices of matching Item Ids. If Item Id is null, an empty list.</returns>
        public List<int> FindAllIndex(Identifier itemId)
        {
            List<int> result = new List<int>();
            if (itemId == null) { Logger.LogError("ItemId is null."); return result; }

            int i = 0;
            ForEach((id) =>
            {
                if (id == itemId) result.Add(i);
                i++;
            });
            return result;
        }

        public bool Exists(Identifier itemId)
        {
            if (itemId == null) { Logger.LogError("ItemId is null."); return false; }

            return itemDefinitions.Exists((id) => id == itemId);
        }

        /// <summary>
        /// Add an item in an empty slot or in a new slot.
        /// If slots are preallocated, the item will be added to an existing empty slot or the last one.
        /// Else, it will be added to a new slot.
        /// </summary>
        /// <param name="itemId">Identifier of the Item to add</param>
        /// <returns>Index of the slot containing the added item. If Item Id is null, -1.</returns>
        public int Add(Identifier itemId)
        {
            if (itemId == null) { Logger.LogError("ItemId is null."); return -1; }

            int index = Count - 1;
            if (preallocated)
            {
                index = UsedCapacity - 1;

                int firstEmptySlot = FindIndex(new Identifier());
                if (firstEmptySlot != -1 && firstEmptySlot <= index)
                    index = firstEmptySlot;

                itemDefinitions[index] = itemId;
            }
            else
            {
                itemDefinitions.Add(itemId);
            }

            return index;
        }

        /// <summary>
        /// Remove the slot matching the specified index.
        /// If slots are preallocated, the slot is only emptied, matching with no valid item.
        /// </summary>
        /// <param name="slot">Index of the slot.</param>
        public void Remove(int slot)
        {
            if (slot < 0) { Logger.LogError("Slot index is negative."); return; }
            if (slot > Count - 1) { Logger.LogError("Slot index is too big."); return; }

            if (preallocated)
            {
                itemDefinitions[slot] = new Identifier();
            }
            else
            {
                itemDefinitions.RemoveAt(slot);
            }
        }

        /// <summary>
        /// Remove all slots containing the specified Item Id.
        /// If slots are preallocated, the slots are emptied, matching with no valid item.
        /// Else, the slot containing this Item Id is completely removed.
        /// </summary>
        /// <param name="itemId">Identifier of the Item to remove</param>
        /// <returns>Number of slots emptied or removed. If Item Id is null, 0.</returns>
        public int RemoveAll(Identifier itemId)
        {
            int removed = 0;
            if (itemId == null) { Logger.LogError("ItemId is null."); return removed; }

            if (preallocated)
            {
                FindAllIndex(itemId).ForEach((index) =>
                {
                    itemDefinitions[index] = new Identifier();
                    removed++;
                });
            }
            else
            {
                removed = itemDefinitions.RemoveAll((id) => id == itemId);
            }
            return removed;
        }

        public void ForEach(Action<Identifier> action)
        {
            itemDefinitions.ForEach((identifier) => action(identifier));
        }

        /// <summary>
        /// Swaps the elements at <paramref name="slot1"/> and <paramref name="slot2"/> with minimal copying.
        /// This implementation uses tuples as it is the recommended way to do it since C#7.
        /// See: https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0180
        /// </summary>
        /// <param name="slot1">The index of the first item to swap.</param>
        /// <param name="slot2">The index of the second item to swap.</param>
        public void Swap(int slot1, int slot2)
        {
            (itemDefinitions[slot1], itemDefinitions[slot2]) = (itemDefinitions[slot2], itemDefinitions[slot1]);
        }

        public Dictionary<Identifier, float> Search(string query)
        {
            Dictionary<Identifier, float> resultsByItem = new Dictionary<Identifier, float>();
            if (String.IsNullOrWhiteSpace(query)) return resultsByItem;

            string trimmedQuery = query.Trim();
            string[] searchTerms = trimmedQuery.Split(' ');

            foreach (var itemId in itemDefinitions)
            {
                if (itemId.IsUndefined) continue;
                var item = ARKitectApp.Instance.Items[itemId];

                float weight = 0;
                float totalWeight = 0;
                foreach (var term in searchTerms)
                {
                    weight += item.Name.CountOccurrences(term) * 1.8F;
                    weight += item.Author.CountOccurrences(term) * 1.4F;
                    weight += item.Category.ToString().CountOccurrences(term) * 1.4F;
                    weight += item.Type.ToString().CountOccurrences(term) * 1.4F;
                    weight += item.Description.CountOccurrences(term);
                    weight += string.Join(",", item.Tags).CountOccurrences(term) * 1.4F;

                    if (totalWeight > 0 && weight > 0)
                    {
                        weight *= 3.0F;
                    }

                    if (weight > 0)
                    {
                        totalWeight += weight;
                        weight = 0;
                    }
                }

                resultsByItem.Add(itemId, totalWeight);
            }

            return resultsByItem;
        }

        public void Sort(Comparison<Identifier> comparison)
        {
            itemDefinitions.Sort(comparison);
        }

        public void SortInAlphabeticalOrder()
        {
            itemDefinitions.Sort();
        }
    }

}