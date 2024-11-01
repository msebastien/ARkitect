using System;
using UnityEngine;

using ARKitect.Core;
using ARKitect.Items.Resource;
using ARKitect.Geometry;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.Commands
{
    public class CommandApplyMaterial : ICommand
    {
        private IResourceMaterial _newMaterial;
        private IResourceMaterial _prevMaterial;
        private Guid _instanceId;
        private Vector2 _screenPosition;
        private int _submeshIndex = -1;

        public CommandApplyMaterial(IResourceMaterial itemMaterial, GameObject obj, Vector2 screenPos)
            : this(
                  itemMaterial,
                  obj.GetComponent<BuildingObject>().InstanceId,
                  screenPos
                  )
        { }

        public CommandApplyMaterial(IResourceMaterial itemMaterial, Guid instanceId, Vector2 screenPos)
        {
            _newMaterial = itemMaterial;
            _screenPosition = screenPos;
            _instanceId = instanceId;

            var instanceManager = ARKitectApp.Instance.InstanceManager;
            if (instanceManager.GetInstance(instanceId).TryGetComponent<IGeometryProvider>(out var provider))
            {
                _prevMaterial = new ResourceMaterial(itemMaterial.Item, provider.GetFaceMaterial(screenPos));
            }
        }

        public void Execute()
        {
            var obj = ARKitectApp.Instance.InstanceManager.GetInstance(_instanceId);

            // If submeshindex has already been set
            if (_submeshIndex >= 0)
            {
                _newMaterial.ApplyTo(obj, _submeshIndex);
                return;
            }

            // Else, retrieve the submesh index when executing this command for the first time (never undone and redone)
            int submeshIndex = _newMaterial.ApplyTo(obj, _screenPosition);
            if (submeshIndex >= 0)
                _submeshIndex = submeshIndex;
        }

        public void Undo()
        {
            var obj = ARKitectApp.Instance.InstanceManager.GetInstance(_instanceId);
            _prevMaterial.ApplyTo(obj, _submeshIndex);
        }
    }

}
