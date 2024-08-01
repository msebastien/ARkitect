using UnityEngine;

using ARKitect.Items.Resource;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Commands
{
    public class CommandSpawn : ICommand
    {
        private ResourceObject _itemResource;
        private Vector3 _position;
        private Quaternion _rotation;
        private int _instanceID = -1;

        public CommandSpawn(ResourceObject itemObject, Vector3 position, Quaternion rotation)
        {
            _itemResource = itemObject;
            _position = position;
            _rotation = rotation;
        }

        public void Execute()
        {
            _instanceID = _itemResource.Spawn(_position, _rotation);
            Logger.LogInfo($"Spawn object of '{_itemResource.Item}'.");
        }

        public void Undo()
        {
            if(_itemResource.DestroyObject(_instanceID))
                Logger.LogInfo($"Destroy instance '{_instanceID}' of the object '{_itemResource.Item}'.");

            _instanceID = -1;
        }
    }

}
