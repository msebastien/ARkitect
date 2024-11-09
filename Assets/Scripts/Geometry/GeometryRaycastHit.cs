using UnityEngine;

namespace ARKitect.Geometry
{
    /// <summary>
    /// The results of a raycast hit.
    /// </summary>
    sealed class GeometryRaycastHit
    {
        public float distance;
        public Vector3 point;
        public Vector3 normal;
        public int face;

        public GeometryRaycastHit(
            float distance,
            Vector3 point,
            Vector3 normal,
            int face)
        {
            this.distance = distance;
            this.point = point;
            this.normal = normal;
            this.face = face;
        }
    }

}
