using UnityEngine;
using Sirenix.OdinInspector;

using ARKitect.Core;

namespace ARKitect.Items.Resource
{
    public class ResourceObject : IResourceObject
    {
        [SerializeField]
        [ReadOnly]
        private Identifier _itemId;
        public Identifier Item => _itemId;

        [SerializeField]
        private GameObject _resource;
        public GameObject Resource => _resource;

        public System.Type Type => GetType();

        public ResourceObject(string itemId, GameObject resource)
        {
            _itemId = new Identifier(itemId);
            _resource = resource;
        }

        public ResourceObject(Identifier itemId, GameObject resource)
        {
            _itemId = itemId;
            _resource = resource;
        }

        public int Spawn(Vector3 position, Quaternion rotation)
        {
            return PrefabsManager.Instance.Spawn(_resource, position, rotation);
        }
    }

}