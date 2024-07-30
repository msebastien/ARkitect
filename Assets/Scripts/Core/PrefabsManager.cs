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
        [Tooltip("Map of Item identifiers and Items")]
        [SerializeField]
        private Dictionary<Identifier, Item> _itemCatalog = new Dictionary<Identifier, Item>();
        public static Dictionary<Identifier, Item> Items => Instance._itemCatalog;

        [Header("Instances")]
        [SerializeField]
        [Tooltip("Parent of the spawned objects")]
        private Transform _instancesParent;
        public Transform InstancesParent => _instancesParent;
        [DictionaryDrawerSettings(KeyLabel = "Instance ID", ValueLabel = "GameObject")]
        [Tooltip("Cached object instances")]
        [SerializeField]
        private Dictionary<int, GameObject> _instancesDict = new Dictionary<int, GameObject>();
        public static Dictionary<int, GameObject> Instances => Instance._instancesDict;

        [Header("Event")]
        [SerializeField]
        private UnityEvent _onItemCatalogLoaded;
        public UnityEvent OnItemCatalogLoaded => _onItemCatalogLoaded;

        private InternalImporter internalImporter;


        private void Awake()
        {
            internalImporter = FindObjectOfType<InternalImporter>();
        }

        private void Start()
        {
            // Load built-in items
            _itemCatalog.AddRange(internalImporter?.Load());
            _onItemCatalogLoaded.Invoke();
        }

        /// <summary>
        /// Instantiate a GameObject and save it
        /// </summary>
        /// <param name="obj">GameObject to instantiate</param>
        /// <param name="position">Coordinates in World Space</param>
        /// <param name="rotation">Rotation</param>
        /// <returns>Instance ID</returns>
        public int Spawn(GameObject obj, Vector3 position, Quaternion rotation)
        {
            var go = Instantiate(obj, position, rotation, _instancesParent);
            _instancesDict.Add(go.GetInstanceID(), go);
            return go.GetInstanceID();
        }
    }

}
