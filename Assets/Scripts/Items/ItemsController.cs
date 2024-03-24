using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

namespace ARKitect.Items
{
    /// <summary>
    /// Assign items to slots
    /// </summary>
    public class ItemsController : MonoBehaviour
    {
        public List<Identifier> ItemDefinitionsInSlots = new List<Identifier>();

        public void Swap(int slot1, int slot2)
        {
            ItemDefinitionsInSlots.SwapAtIndices(slot1, slot2);
        }

        public void Spawn(Vector3 position)
        {

        }
    }

}