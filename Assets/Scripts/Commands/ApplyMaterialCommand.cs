using ARKitect.Items.Resource;
using UnityEngine;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Commands
{
    public class ApplyMaterialCommand : ICommand
    {
        private ResourceMaterial _itemResource;
        private GameObject _selectedObject;

        public ApplyMaterialCommand(ResourceMaterial itemMaterial, GameObject selectedObject)
        {
            _itemResource = itemMaterial;
            _selectedObject = selectedObject;
        }

        public void Execute()
        {
            _itemResource.ApplyTo(_selectedObject);
        }
    }

}
