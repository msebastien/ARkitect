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
        private ResourceMaterial _newMaterial;
        private ResourceMaterial _prevMaterial;
        private Guid _instanceId;
        private Vector2 _screenPosition;
        private int _submeshIndex;

        public CommandApplyMaterial(ResourceMaterial itemMaterial, GameObject obj, Vector2 screenPos)
            : this(
                  itemMaterial,
                  obj.GetComponent<BuildingObject>().InstanceId,
                  screenPos
                  )
        { }

        public CommandApplyMaterial(ResourceMaterial itemMaterial, Guid instanceId, Vector2 screenPos)
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

            int submeshIndex;
            if ((submeshIndex = _newMaterial.ApplyTo(obj, _screenPosition)) != -1)
                _submeshIndex = submeshIndex;
        }

        public void Undo()
        {
            var obj = ARKitectApp.Instance.InstanceManager.GetInstance(_instanceId);
            _prevMaterial.ApplyTo(obj, _submeshIndex);
        }
    }

}
