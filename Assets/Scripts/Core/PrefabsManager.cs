using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

using ARKitect.Items;
using ARKitect.Items.Import;

namespace ARKitect.Core
{
    /// <summary>
    /// Manage prefab instances and store item definitions
    /// </summary>
    [AddComponentMenu("ARkitect/Prefabs Manager")]
    public class PrefabsManager : SerializedSingleton<PrefabsManager>
    {
        [Header("Item Catalog")]
        [DictionaryDrawerSettings(KeyLabel = "Identifier", ValueLabel = "Item properties")]
        [SerializeField]
        private Dictionary<Identifier, IItem> itemCatalog = new Dictionary<Identifier, IItem>();
        public Dictionary<Identifier, IItem> Items => itemCatalog;

        // Cached object instances
        [Header("Instances")]
        [DictionaryDrawerSettings(KeyLabel = "Instance ID", ValueLabel = "GameObject")]
        [SerializeField]
        private Dictionary<int, GameObject> instances = new Dictionary<int, GameObject>();

        [Header("Event")]
        [SerializeField]
        private UnityEvent _onItemCatalogLoaded;
        public static UnityEvent OnItemCatalogLoaded => Instance._onItemCatalogLoaded;

        
        private InternalImporter internalImporter;

        private void Awake()
        {
            internalImporter = FindObjectOfType<InternalImporter>();
        }

        private void Start()
        {
            // Load built-in items
            itemCatalog.AddRange(internalImporter?.Load());
            _onItemCatalogLoaded.Invoke();
        }

        /// <summary>
        /// Instantiate an item and save it
        /// </summary>
        /// <param name="item">Item definition identifier</param>
        /// <param name="position">Coordinates in World Space</param>
        /// <param name="parent">Parent transform of this newly instanced object</param>
        public void Spawn(Identifier item, Vector3 position, Transform parent = null)
        {       
            if (itemCatalog[item].Type == ItemType.Object)
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
