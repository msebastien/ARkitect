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

        /// <summary>
        /// Instantiate the object resource
        /// </summary>
        /// <param name="position">Coordinates on world space</param>
        /// <param name="rotation"></param>
        /// <returns>Instance ID</returns>
        public int Spawn(Vector3 position, Quaternion rotation)
        {
            return ARKitectApp.InstanceManager.Spawn(_resource, position, rotation);
        }

        /// <summary>
        /// Destroy the specified object
        /// </summary>
        /// <param name="id">Id of the instantiated object</param>
        /// <returns>'true' if it succeeded, else 'false'</returns>
        public bool DestroyObject(int instanceID)
        {
            return ARKitectApp.InstanceManager.DestroyInstance(instanceID);
        }
    }

}