using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

using ARKitect.Items;
using ARKitect.Items.Import;
using ARKitect.Coroutine;
using ARKitect.Commands;
using ARKitect.Geometry;

namespace ARKitect.Core
{
    /// <summary>
    /// Main class for ARkitect application
    /// </summary>
    [AddComponentMenu("ARkitect/App")]
    public class ARKitectApp : Singleton<ARKitectApp>
    {
        [Header("Managers")]
        [SerializeField]
        private InstanceManager _instanceManager;
        public InstanceManager InstanceManager => _instanceManager;
        [SerializeField]
        private CoroutineManager _coroutineManager;
        public CoroutineManager CoroutineManager => _coroutineManager;
        [SerializeField]
        private CommandManager _commandManager;
        public CommandManager CommandManager => _commandManager;
        [SerializeField]
        private GeometryProviderManager _geometryProviderManager;
        public GeometryProviderManager GeometryProviderManager => _geometryProviderManager;


        private Dictionary<Identifier, Item> _itemCatalog = new Dictionary<Identifier, Item>();
        public Dictionary<Identifier, Item> Items => _itemCatalog;

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
