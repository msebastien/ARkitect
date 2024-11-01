using System;
using UnityEngine;

using ARKitect.Items.Resource;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Commands
{
    public class CommandSpawn : ICommand
    {
        private IResourceObject _itemResource;
        private Vector3 _position;
        private Quaternion _rotation;
        private Guid _instanceID = Guid.Empty;

        public CommandSpawn(IResourceObject itemObject, Vector3 position, Quaternion rotation)
        {
            _itemResource = itemObject;
            _position = position;
            _rotation = rotation;
        }

        public void Execute()
        {
            if(_instanceID == Guid.Empty)
                _instanceID = _itemResource.Spawn(_position, _rotation);
            else
                _itemResource.Spawn(_instanceID, _position, _rotation);

            Logger.LogInfo($"Execute {GetType()}: Spawn object of '{_itemResource.Item}'.");
        }

        public void Undo()
        {
            if(_itemResource.DestroyObject(_instanceID))
                Logger.LogInfo($"Undo {GetType()}: Destroy instance '{_instanceID}' of the object '{_itemResource.Item}'.");
        }
    }

}
