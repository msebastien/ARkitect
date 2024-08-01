using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

using ARKitect.Items;
using ARKitect.Items.Import;
using ARKitect.Coroutine;

namespace ARKitect.Core
{
    /// <summary>
    /// Main class for ARkitect application
    /// </summary>
    [AddComponentMenu("ARkitect/App")]
    public class ARKitectApp : SerializedSingleton<ARKitectApp>
    {
        [Header("Managers")]
        [SerializeField]
        private InstanceManager _instanceManager;
        public static InstanceManager InstanceManager => Instance._instanceManager;
        [SerializeField]
        private CoroutineManager _coroutineManager;
        public static CoroutineManager CoroutineManager => Instance._coroutineManager;

        [Header("Item Catalog")]
        [DictionaryDrawerSettings(KeyLabel = "Identifier", ValueLabel = "Item properties")]
        [Tooltip("Map of Item identifiers and Items")]
        [SerializeField]
        private Dictionary<Identifier, Item> _itemCatalog = new Dictionary<Identifier, Item>();
        public static Dictionary<Identifier, Item> Items => Instance._itemCatalog;

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
    }

}
