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
        private Dictionary<Identifier, IItem> itemCatalog = new Dictionary<Identifier, IItem>();
        public Dictionary<Identifier, IItem> Items => itemCatalog;

        // Cached object instances
        // Maybe use a dictionary with the Unity instance ID ?
        private Dictionary<int, GameObject> instances = new Dictionary<int, GameObject>();

        // TODO: Initialize the Item Catalog when starting the app (called via the bootstrapper)


        /// <summary>
        /// Instantiate an item and save it
        /// </summary>
        /// <param name="item">Item definition identifier</param>
        /// <param name="position">Coordinates in World Space</param>
        /// <param name="parent">Parent transform of this newly instanced object</param>
        public void Spawn(Identifier item, Vector3 position, Transform parent = null)
        {
            var selectedItem = itemCatalog[item];
            if (itemCatalog[item] is Item<GameObject>)
            {
                var itemObject = itemCatalog[item] as Item<GameObject>;
                var go = Instantiate(itemObject.Resource, position, Quaternion.identity, parent);
                instances.Add(go.GetInstanceID(), go);
                Logger.LogInfo($"Object of the item {item} has been instantiated.");
            }
            else
            {
                Logger.LogInfo($"The resource of the item {item} is not a GameObject and cannot be instantiated");
            }
        }
    }

}
