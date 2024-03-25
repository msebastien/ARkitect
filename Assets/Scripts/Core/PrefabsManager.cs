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
        // Maybe a "resource locator" instead of a basic string like "namespace:itemID" ?
        // Use OdinInspector to serialize the dictionary
        [DictionaryDrawerSettings(KeyLabel = "Identifier", ValueLabel = "Item properties")]
        [SerializeField]
        private Dictionary<Identifier, Item> itemCatalog = new Dictionary<Identifier, Item>();
        public Dictionary<Identifier, Item> Items => itemCatalog;
        
    }

}
