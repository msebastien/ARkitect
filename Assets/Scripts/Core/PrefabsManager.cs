using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using ARKitect.Items;
using Sirenix.OdinInspector;

namespace ARKitect.Core
{
    /// <summary>
    /// Manage prefab instances and store item definitions
    /// </summary>
    [AddComponentMenu("ARkitect/Prefabs Manager")]
    public class PrefabsManager : SerializedSingleton<PrefabsManager>
    {
        [DictionaryDrawerSettings(KeyLabel = "Identifier", ValueLabel = "Item properties")]
        [SerializeField]
        private Dictionary<Identifier, Item> itemCatalog = new Dictionary<Identifier, Item>();
        public Dictionary<Identifier, Item> Items => itemCatalog;

        // TODO: Initialize the Item Catalog when starting the app (called via the bootstrapper)


        // TODO: Check if the item can be instantiated. If it is an object, instantiate it.
        public void Spawn(Identifier item, Vector3 position)
        {

        }
    }

}
