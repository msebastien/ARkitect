using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

using ARKitect.Core;

namespace ARKitect.Items
{
    /// <summary>
    /// Assign items to slots
    /// </summary>
    public class ItemsController : MonoBehaviour
    {
        private List<Identifier> itemDefinitions;
        public List<Identifier> ItemDefinitionsInSlots => itemDefinitions;

        [Tooltip("Whether to preallocate capacity for the item definitions list")]
        [SerializeField]
        private bool preallocated = false;
        public bool Preallocated => preallocated;

        [Tooltip("Capacity to preallocate for the item definitions list")]
        public int capacity = 7;

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

        public void Remove(int slot)
        {
            if (preallocated)
            {
                ItemDefinitionsInSlots[slot] = new Identifier();
            }
            else
            {
                // TODO: If not preallocated, remove completely the item definition from the list and the slot itself.
                // Then, because a slot is removed, the indices must be updated.
                //ItemDefinitionsInSlots.RemoveAt(slot);
                //GetComponent<UIItembar>().
            }
        }

        public void Swap(int slot1, int slot2)
        {
            ItemDefinitionsInSlots.SwapAtIndices(slot1, slot2);
        }

        /// <summary>
        /// Spawn the item in the specified slot, if it can be instantiated
        /// </summary>
        /// <param name="slot">Index of the slot</param>
        /// <param name="position">Position of the object to instantiate in World Space</param>
        public void Spawn(int slot, Vector3 position)
        {
            var itemDefinitionId = itemDefinitions[slot];
            PrefabsManager.Instance.Spawn(itemDefinitionId, position);
        }
    }

}