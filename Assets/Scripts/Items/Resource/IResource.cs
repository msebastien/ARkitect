using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARKitect.Items.Resource
{
    public interface IResource
    {
        Identifier Item { get; }
    }

    public interface IResourceActions : IResource
    {
        int GetRaycastMask();
        void RunCommand(RaycastHit hit, PointerEventData eventData);
    }

    public interface IResourceProperty<T> : IResource where T : UnityEngine.Object
    {
        T Resource { get; }
    }

    public interface IResourceObject : IResourceProperty<GameObject>
    {
        Guid Spawn(Vector3 position, Quaternion rotation);
        void Spawn(Guid guid, Vector3 position, Quaternion rotation);
        bool DestroyObject(Guid instanceID);
    }

    public interface IResourceMaterial : IResourceProperty<Material>
    {
        int ApplyTo(GameObject obj, Vector2 screenPos);
        void ApplyTo(GameObject obj, int submeshIndex);
    }

}
