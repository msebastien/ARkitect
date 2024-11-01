using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

using ARKitect.Core;
using ARKitect.Geometry;
using ARKitect.Commands;

namespace ARKitect.Items.Resource
{
    public class ResourceMaterial : IResourceMaterial, IResourceActions
    {
        [SerializeField]
        [ReadOnly]
        private Identifier _itemId;
        public Identifier Item => _itemId;

        [SerializeField]
        private Material _resource;
        public Material Resource => _resource;

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

        public int GetRaycastMask()
        {
            string objectLayer = LayerMask.LayerToName((int)Layers.BUILDING_OBJECT);
            return LayerMask.GetMask(new string[] { objectLayer });
        }

        public void RunCommand(RaycastHit hit, PointerEventData eventData)
        {
            ICommand cmd = new CommandApplyMaterial(this, hit.collider.gameObject, eventData.position);
            ARKitectApp.Instance.CommandManager.ExecuteCommand(cmd);
        }

        public int ApplyTo(GameObject obj, Vector2 screenPos)
        {
            int submeshIndex = -1;
            if (obj.TryGetComponent<IGeometryProvider>(out var provider))
            {
                submeshIndex = provider.SetFaceMaterial(screenPos, _resource);
            }

            return submeshIndex;
        }

        public void ApplyTo(GameObject obj, int submeshIndex)
        {
            if (obj.TryGetComponent<IGeometryProvider>(out var provider))
            {
                provider.SetFaceMaterial(submeshIndex, _resource);
            }
        }
    }

}
