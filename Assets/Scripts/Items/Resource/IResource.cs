using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;

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
        Face ApplyTo(GameObject obj, Vector2 screenPos);
        void ApplyTo(GameObject obj, Face face);
    }

}
