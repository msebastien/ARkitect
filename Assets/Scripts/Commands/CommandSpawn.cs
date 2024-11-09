using System;
using UnityEngine;

using ARKitect.Items.Resource;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.Commands
{
    public class CommandSpawn : ICommand
    {
        private IResourceObject _obj;
        private Vector3 _position;
        private Quaternion _rotation;
        private Guid _instanceID = Guid.Empty;

        public CommandSpawn(IResourceObject itemObject, Vector3 position, Quaternion rotation)
        {
            _obj = itemObject;
            _position = position;
            _rotation = rotation;

            Logger.LogWarning($"{ToString()}");
        }

        public void Execute()
        {
            if (_instanceID == Guid.Empty)
                _instanceID = _obj.Spawn(_position, _rotation);
            else
                _obj.Spawn(_instanceID, _position, _rotation);

            Logger.LogInfo($"Execute {GetType()}: Spawn object of '{_obj.Item}'.");
        }

        public void Undo()
        {
            if (_obj.DestroyObject(_instanceID))
                Logger.LogInfo($"Undo {GetType()}: Destroy instance '{_instanceID}' of the object '{_obj.Item}'.");
        }

        public override string ToString()
        {
            return $"Spawn Object (obj: {_obj.Item}/pos: {_position}/rot: {_rotation}/guid: {_instanceID})";
        }
    }

}
