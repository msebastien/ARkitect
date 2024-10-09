using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using ARKitect.Core;
using ARKitect.Items;

namespace ARKitect.Debug
{
#if UNITY_EDITOR
    public class ItemCatalogDebug : SerializedMonoBehaviour
    {
        [Header("Item Catalog")]
        [DictionaryDrawerSettings(KeyLabel = "Identifier", ValueLabel = "Item properties")]
        [Tooltip("Map of Item identifiers and Items")]
        [SerializeField]
        private Dictionary<Identifier, Item> _items;

        private void Awake()
        {
            _items = ARKitectApp.Instance.Items;
        }

    }
#endif
}
