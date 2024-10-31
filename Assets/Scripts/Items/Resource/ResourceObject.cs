using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

using ARKitect.Core;
using ARKitect.Commands;

namespace ARKitect.Items.Resource
{
    public class ResourceObject : IResourceObject
    {
        [SerializeField]
        [ReadOnly]
        private Identifier _itemId;
        public Identifier Item => _itemId;

        [SerializeField]
        private GameObject _resource;
        public GameObject Resource => _resource;

        public ResourceObject(string itemId, GameObject resource)
        {
            _itemId = new Identifier(itemId);
            _resource = resource;
        }

        public ResourceObject(Identifier itemId, GameObject resource)
        {
            _itemId = itemId;
            _resource = resource;
        }

        public int GetRaycastMask()
        {
            string gridLayer = LayerMask.LayerToName((int)Layers.GRID);
            string objectLayer = LayerMask.LayerToName((int)Layers.BUILDING_OBJECT);
            return LayerMask.GetMask(new string[] { gridLayer, objectLayer });
        }

        public void RunCommand(RaycastHit hit, PointerEventData eventData)
        {
            ICommand cmd = new CommandSpawn(this, hit.point, Quaternion.identity);
            ARKitectApp.Instance.CommandManager.ExecuteCommand(cmd);
        }

        /// <summary>
        /// Instantiate the object resource
        /// </summary>
        /// <param name="position">Coordinates on world space</param>
        /// <param name="rotation"></param>
        /// <returns>Unique Instance ID</returns>
        public Guid Spawn(Vector3 position, Quaternion rotation)
        {
            var instanceManager = ARKitectApp.Instance.InstanceManager;

            var guid = instanceManager.Spawn(_resource, position, rotation);
            instanceManager.GetInstance(guid).AddComponent<BuildingObject>().Init(guid, _itemId);

            return guid;
        }

        /// <summary>
        /// Instantiate the object resource with a pre-defined unique Id
        /// </summary>
        /// <param name="guid">Unique Instance Id</param>
        /// <param name="position">Coordinates on world space</param>
        /// <param name="rotation"></param>
        public void Spawn(Guid guid, Vector3 position, Quaternion rotation)
        {
            var instanceManager = ARKitectApp.Instance.InstanceManager;

            instanceManager.Spawn(guid, _resource, position, rotation);
            instanceManager.GetInstance(guid).AddComponent<BuildingObject>().Init(guid, _itemId);
        }

        /// <summary>
        /// Destroy the specified object
        /// </summary>
        /// <param name="id">Id of the instantiated object</param>
        /// <returns>'true' if it succeeded, else 'false'</returns>
        public bool DestroyObject(Guid instanceID)
        {
            return ARKitectApp.Instance.InstanceManager.DestroyInstance(instanceID);
        }
    }

}