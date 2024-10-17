using UnityEngine;

namespace ARKitect.Items.Resource
{
    public interface IResource
    {
        Identifier Item { get; }
    }

    public interface IResourceProp<T> where T : Object
    {
        T Resource { get; }
    }

    public interface IResourceObject : IResource, IResourceProp<GameObject>
    {
        int Spawn(Vector3 position, Quaternion rotation);
        bool DestroyObject(int instanceID);
    }
    public interface IResourceMaterial : IResource, IResourceProp<Material>
    {
        void ApplyTo(GameObject obj);
    }

}
