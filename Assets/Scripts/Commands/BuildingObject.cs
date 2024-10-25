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
        [SerializeField]
        private Guid _instanceId = Guid.Empty;
        public Guid InstanceId => _instanceId;

        [SerializeField]
        private Identifier _itemId = new Identifier(); // Item Id matching this object
        public Identifier ItemId => _itemId;

        private bool _isInitialized = false;

        public void Init(Guid instanceId, Identifier itemId)
        {
            if (_isInitialized) return;
            if (instanceId == Guid.Empty) return;
            if (itemId.IsUndefined) return;

            if (_instanceId == Guid.Empty)
                _instanceId = instanceId;

            if (_itemId.IsUndefined)
                _itemId = itemId;

            _isInitialized = true;
        }

    }

}