using UnityEngine;

namespace ARKitect.Geometry
{
    public interface IGeometryProvider
    {
        void Init();
        int SetFaceMaterial(Vector3 screenPos, Material material);
        void SetFaceMaterial(int submeshIndex, Material material);
        Material GetFaceMaterial(Vector3 screenPos);
        Material GetFaceMaterial(int submeshIndex);
    }

}
