using System;
using UnityEngine;
using UnityEngine.ProBuilder;

using ARKitect.Core;
using ARKitect.Items;
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
        private Face _face;
        private Ray _ray;

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
            _instanceId = instanceId;
            _ray = Camera.main.ScreenPointToRay(screenPos);

            var instanceManager = ARKitectApp.Instance.InstanceManager;
            if (instanceManager.GetInstance(instanceId).TryGetComponent<GeometrySystem>(out var geometry))
            {
                _face = geometry.GetFace(_ray);
                var mat = geometry.GetFaceMaterial(_face);
                _prevMaterial = new ResourceMaterial(new Identifier(mat.name), mat);
            }

            Logger.LogWarning($"{ToString()}");
        }

        public void Execute()
        {
            var obj = ARKitectApp.Instance.InstanceManager.GetInstance(_instanceId);

            if (_face == null && obj.TryGetComponent<GeometrySystem>(out var geometry))
                _face = geometry.GetFace(_ray);

            _newMaterial.ApplyTo(obj, _face);
        }

        public void Undo()
        {
            var obj = ARKitectApp.Instance.InstanceManager.GetInstance(_instanceId);
            _prevMaterial.ApplyTo(obj, _face);
            _face = null;
        }

        public override string ToString()
        {
            return $"Apply Material (mtl: {_newMaterial.Item}/prev_mtl: {_prevMaterial.Item}/obj: {_instanceId})";
        }
    }

}
