using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using ARKitect.Core;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Items
{
    /// <summary>
    /// Assign items to slots
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

        [Tooltip("Capacity to preallocate for the item definitions list")]
        public int capacity = 7;

        [SerializeField]
        [Tooltip("Parent of the spawned objects")]
        private Transform instancesParent;

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
            itemDefinitions = new List<Identifier>(capacity);
            for (int i = 0; i < capacity; i++)
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

        public int FindIndex(Identifier itemId)
        {
            return itemDefinitions.FindIndex((id) => id == itemId);
        }

        /// <summary>
        /// Find all the item occurences
        /// </summary>
        /// <param name="itemId">Item Identifier</param>
        /// <returns>Indices of matching Item Ids</returns>
        public List<int> FindAllIndex(Identifier itemId)
        {
            List<int> result = new List<int>();

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
            return itemDefinitions.Exists((id) => id == itemId);
        }

        public void Add(Identifier itemId)
        {
            itemDefinitions.Add(itemId);
        }

        /// <summary>
        /// Remove an element matching a slot index.
        /// If slots are preallocated, the slot is only emptied, matching with no valid item.
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        public void Remove(int slot)
        {
            if (preallocated)
            {
                itemDefinitions[slot] = new Identifier();
            }
            else
            {
                itemDefinitions.RemoveAt(slot);
            }
        }

        public int RemoveAll(Identifier itemId)
        {
            int removed = 0;
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

        /// <summary>
        /// Spawn the item in the specified slot, if it can be instantiated
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="position">Position of the object to instantiate in World Space</param>
        public void Spawn(int slot, Vector3 position)
        {
            var itemDefinitionId = itemDefinitions[slot];
            PrefabsManager.Instance.Spawn(itemDefinitionId, position, instancesParent);
        }
    }

}