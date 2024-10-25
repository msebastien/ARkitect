using System;
using UnityEngine;

using ARKitect.Items;

namespace ARKitect.Core
{
    /// <summary>
    /// Store metadata about an instanced building object in the scene
    /// </summary>
    public class BuildingObject : MonoBehaviour
    {
        private Guid _instanceId = Guid.Empty;
        public Guid InstanceId => _instanceId;

        private Identifier _itemId = new Identifier(); // Item Id matching this object
        public Identifier ItemId => _itemId;

        private bool _isInitialized = false;

#if UNITY_EDITOR
        [SerializeField]
        private string _instanceGuidString;
        [SerializeField]
        private string _itemIdString;
#endif

        public void Init(Guid instanceId, Identifier itemId)
        {
            if (_isInitialized) return;
            if (instanceId == Guid.Empty) return;
            if (itemId.IsUndefined) return;

            if (_instanceId == Guid.Empty)
                _instanceId = instanceId;

            if (_itemId.IsUndefined)
                _itemId = itemId;

#if UNITY_EDITOR
            _instanceGuidString = _instanceId.ToString();
            _itemIdString = _itemId.ToString();
#endif

            _isInitialized = true;
        }

    }

}