using UnityEngine;
using Sirenix.OdinInspector;

namespace ARKitect.Items.Resource
{
    public class ResourceMaterial : IResourceMaterial
    {
        [SerializeField]
        [ReadOnly]
        private Identifier _itemId;
        public Identifier Item => _itemId;

        [SerializeField]
        private Material _resource;
        public Material Resource => _resource;

        public System.Type Type => GetType();

        public ResourceMaterial(string itemId, Material resource)
        {
            _itemId = new Identifier(itemId);
            _resource = resource;
        }

        public ResourceMaterial(Identifier itemId, Material resource)
        {
            _itemId = itemId;
            _resource = resource;
        }

        // TODO: Implement ApplyTo method to add the material to an object
        public void ApplyTo(GameObject obj)
        {

        }
    }

}
