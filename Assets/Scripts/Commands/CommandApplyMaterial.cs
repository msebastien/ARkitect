using UnityEngine;

using ARKitect.Items.Resource;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Commands
{
    public class CommandApplyMaterial : ICommand
    {
        private ResourceMaterial _itemResource;
        private GameObject _selectedObject;

        public CommandApplyMaterial(ResourceMaterial itemMaterial, GameObject selectedObject)
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
