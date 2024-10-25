using System;
using UnityEngine;

namespace ARKitect.Items.Resource
{
    public interface IResource
    {
        Identifier Item { get; }
    }

    public interface IResourceProp<T> where T : UnityEngine.Object
    {
        T Resource { get; }
    }

    public interface IResourceObject : IResource, IResourceProp<GameObject>
    {
        Guid Spawn(Vector3 position, Quaternion rotation);
        void Spawn(Guid guid, Vector3 position, Quaternion rotation);
        bool DestroyObject(Guid instanceID);
    }
    public interface IResourceMaterial : IResource, IResourceProp<Material>
    {
        int ApplyTo(GameObject obj, Vector2 screenPos);
        void ApplyTo(GameObject obj, int submeshIndex);
    }

}
