using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

using ARKitect.Core;

namespace ARKitect.Geometry
{
    /// <summary>
    /// Provides static methods for raycasting with ProBuilderMesh objects.
    /// </summary>
    public static class RaycastUtility
    {
        /// <summary>
        /// Convert a screen point (0,0 bottom left, in pixels) to a GUI point (0,0 top left, in points).
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="point"></param>
        /// <param name="pixelsPerPoint"></param>
        /// <returns></returns>
        internal static Vector3 ScreenToGuiPoint(this Camera camera, Vector3 point, float pixelsPerPoint)
        {
            return new Vector3(point.x / pixelsPerPoint, (camera.pixelHeight - point.y) / pixelsPerPoint, point.z);
        }

        /// <summary>
        /// Find a triangle intersected by InRay on InMesh.  InRay is in world space.
        /// Returns the index in mesh.faces of the hit face, or -1.  Optionally can ignore backfaces.
        /// </summary>
        /// <param name="worldRay"></param>
        /// <param name="mesh"></param>
        /// <param name="hit"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        internal static bool FaceRaycast(Ray worldRay, ProBuilderMesh mesh, out GeometryRaycastHit hit, HashSet<Face> ignore = null)
        {
            return FaceRaycast(worldRay, mesh, out hit, Mathf.Infinity, CullingMode.Back, ignore);
        }

        /// <summary>
        /// Find the nearest face intersected by InWorldRay on this pb_Object.
        /// </summary>
        /// <param name="worldRay">A ray in world space.</param>
        /// <param name="mesh">The ProBuilder object to raycast against.</param>
        /// <param name="hit">If the mesh was intersected, hit contains information about the intersect point in local coordinate space.</param>
        /// <param name="distance">The distance from the ray origin to the intersection point.</param>
        /// <param name="cullingMode">Which sides of a face are culled when hit testing. Default is back faces are culled.</param>
        /// <param name="ignore">Optional collection of faces to ignore when raycasting.</param>
        /// <returns>True if the ray intersects with the mesh, false if not.</returns>
        internal static bool FaceRaycast(Ray worldRay, ProBuilderMesh mesh, out GeometryRaycastHit hit, float distance, CullingMode cullingMode, HashSet<Face> ignore = null)
        {
            // Transform ray into model space
            worldRay.origin -= mesh.transform.position; // Why doesn't worldToLocalMatrix apply translation?
            worldRay.origin = mesh.transform.worldToLocalMatrix * worldRay.origin;
            worldRay.direction = mesh.transform.worldToLocalMatrix * worldRay.direction;

            var positions = mesh.positions;
            var faces = mesh.faces;

            float OutHitPoint = Mathf.Infinity;
            int OutHitFace = -1;
            Vector3 OutNrm = Vector3.zero;

            // Iterate faces, testing for nearest hit to ray origin. Optionally ignores backfaces.
            for (int i = 0, fc = faces.Count; i < fc; ++i)
            {
                if (ignore != null && ignore.Contains(faces[i]))
                    continue;

                var indexes = mesh.faces[i].indexes;

                for (int j = 0, ic = indexes.Count; j < ic; j += 3)
                {
                    Vector3 a = positions[indexes[j + 0]];
                    Vector3 b = positions[indexes[j + 1]];
                    Vector3 c = positions[indexes[j + 2]];

                    Vector3 nrm = Vector3.Cross(b - a, c - a);
                    float dot = Vector3.Dot(worldRay.direction, nrm);

                    bool skip = false;

                    switch (cullingMode)
                    {
                        case CullingMode.Front:
                            if (dot < 0f) skip = true;
                            break;

                        case CullingMode.Back:
                            if (dot > 0f) skip = true;
                            break;
                    }

                    var dist = 0f;

                    Vector3 point;
                    if (!skip && Math.RayIntersectsTriangle(worldRay, a, b, c, out dist, out point))
                    {
                        if (dist > OutHitPoint || dist > distance)
                            continue;

                        OutNrm = nrm;
                        OutHitFace = i;
                        OutHitPoint = dist;
                    }
                }
            }

            hit = new GeometryRaycastHit(OutHitPoint,
                    worldRay.GetPoint(OutHitPoint),
                    OutNrm,
                    OutHitFace);

            return OutHitFace > -1;
        }

        internal static bool FaceRaycastBothCullModes(Ray worldRay, ProBuilderMesh mesh, ref SimpleTuple<Face, Vector3> back, ref SimpleTuple<Face, Vector3> front)
        {
            // Transform ray into model space
            worldRay.origin -= mesh.transform.position; // Why doesn't worldToLocalMatrix apply translation?
            worldRay.origin = mesh.transform.worldToLocalMatrix * worldRay.origin;
            worldRay.direction = mesh.transform.worldToLocalMatrix * worldRay.direction;

            var positions = mesh.positions;
            var faces = mesh.faces;

            back.item1 = null;
            front.item1 = null;

            float backDistance = Mathf.Infinity;
            float frontDistance = Mathf.Infinity;

            // Iterate faces, testing for nearest hit to ray origin. Optionally ignores backfaces.
            for (int i = 0, fc = faces.Count; i < fc; ++i)
            {
                var indexes = mesh.faces[i].indexes;

                for (int j = 0, ic = indexes.Count; j < ic; j += 3)
                {
                    Vector3 a = positions[indexes[j + 0]];
                    Vector3 b = positions[indexes[j + 1]];
                    Vector3 c = positions[indexes[j + 2]];

                    float dist;
                    Vector3 point;

                    if (Math.RayIntersectsTriangle(worldRay, a, b, c, out dist, out point))
                    {
                        if (dist < backDistance || dist < frontDistance)
                        {
                            Vector3 nrm = Vector3.Cross(b - a, c - a);
                            float dot = Vector3.Dot(worldRay.direction, nrm);

                            if (dot < 0f)
                            {
                                if (dist < backDistance)
                                {
                                    backDistance = dist;
                                    back.item1 = faces[i];
                                }
                            }
                            else
                            {
                                if (dist < frontDistance)
                                {
                                    frontDistance = dist;
                                    front.item1 = faces[i];
                                }
                            }
                        }
                    }
                }
            }

            if (back.item1 != null)
                back.item2 = worldRay.GetPoint(backDistance);

            if (front.item1 != null)
                front.item2 = worldRay.GetPoint(frontDistance);

            return back.item1 != null || front.item1 != null;
        }

        /// <summary>
        /// Find the all faces intersected by InWorldRay on this pb_Object.
        /// </summary>
        /// <param name="InWorldRay">A ray in world space.</param>
        /// <param name="mesh">The ProBuilder object to raycast against.</param>
        /// <param name="hits">If the mesh was intersected, hits contains all intersection point RaycastHit information.</param>
        /// <param name="cullingMode">What sides of triangles does the ray intersect with.</param>
        /// <param name="ignore">Optional collection of faces to ignore when raycasting.</param>
        /// <returns>True if the ray intersects with the mesh, false if not.</returns>
        internal static bool FaceRaycast(
            Ray InWorldRay,
            ProBuilderMesh mesh,
            out List<GeometryRaycastHit> hits,
            CullingMode cullingMode,
            HashSet<Face> ignore = null)
        {
            // Transform ray into model space
            InWorldRay.origin -= mesh.transform.position;  // Why doesn't worldToLocalMatrix apply translation?

            InWorldRay.origin = mesh.transform.worldToLocalMatrix * InWorldRay.origin;
            InWorldRay.direction = mesh.transform.worldToLocalMatrix * InWorldRay.direction;

            var vertices = mesh.positions;

            hits = new List<GeometryRaycastHit>();

            // Iterate faces, testing for nearest hit to ray origin.  Optionally ignores backfaces.
            for (int CurFace = 0; CurFace < mesh.faces.Count; ++CurFace)
            {
                if (ignore != null && ignore.Contains(mesh.faces[CurFace]))
                    continue;

                var indexes = mesh.faces[CurFace].indexes;

                for (int CurTriangle = 0; CurTriangle < indexes.Count; CurTriangle += 3)
                {
                    Vector3 a = vertices[indexes[CurTriangle + 0]];
                    Vector3 b = vertices[indexes[CurTriangle + 1]];
                    Vector3 c = vertices[indexes[CurTriangle + 2]];

                    var dist = 0f;
                    Vector3 point;

                    if (Math.RayIntersectsTriangle(InWorldRay, a, b, c, out dist, out point))
                    {
                        Vector3 nrm = Vector3.Cross(b - a, c - a);

                        float dot; // vars used in loop
                        switch (cullingMode)
                        {
                            case CullingMode.Front:
                                dot = Vector3.Dot(InWorldRay.direction, nrm);

                                if (dot > 0f)
                                    goto case CullingMode.FrontBack;
                                break;

                            case CullingMode.Back:
                                dot = Vector3.Dot(InWorldRay.direction, nrm);

                                if (dot < 0f)
                                    goto case CullingMode.FrontBack;
                                break;

                            case CullingMode.FrontBack:
                                hits.Add(new GeometryRaycastHit(dist,
                                    InWorldRay.GetPoint(dist),
                                    nrm,
                                    CurFace));
                                break;
                        }

                        continue;
                    }
                }
            }

            return hits.Count > 0;
        }

        /// <summary>
        /// Transform a ray from world space to a transform local space.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="InWorldRay"></param>
        /// <returns></returns>
        internal static Ray InverseTransformRay(this Transform transform, Ray InWorldRay)
        {
            Vector3 o = InWorldRay.origin;
            o -= transform.position;
            o = transform.worldToLocalMatrix * o;
            Vector3 d = transform.worldToLocalMatrix.MultiplyVector(InWorldRay.direction);
            return new Ray(o, d);
        }

        /// <summary>
        /// Find the nearest triangle intersected by InWorldRay on this mesh.
        /// </summary>
        /// <param name="InWorldRay"></param>
        /// <param name="hit"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        internal static bool MeshRaycast(Ray InWorldRay, GameObject gameObject, out GeometryRaycastHit hit, float distance = Mathf.Infinity)
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            var mesh = meshFilter != null ? meshFilter.sharedMesh : null;

            if (!mesh)
            {
                hit = default(GeometryRaycastHit);
                return false;
            }

            var transform = gameObject.transform;
            var ray = transform.InverseTransformRay(InWorldRay);
            return MeshRaycast(ray, mesh.vertices, mesh.triangles, out hit, distance);
        }

        private delegate bool RayIntersectsTriangle2
        (
            Vector3 origin,
            Vector3 dir,
            Vector3 vert0,
            Vector3 vert1,
            Vector3 vert2,
            ref float distance,
            ref Vector3 normal
        );

        /// <summary>
        /// Cast a ray (in model space) against a mesh.
        /// </summary>
        /// <param name="InRay"></param>
        /// <param name="mesh"></param>
        /// <param name="triangles"></param>
        /// <param name="hit"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        internal static bool MeshRaycast(Ray InRay, Vector3[] mesh, int[] triangles, out GeometryRaycastHit hit, float distance = Mathf.Infinity)
        {
            // float dot;               // vars used in loop
            float hitDistance = Mathf.Infinity;
            Vector3 hitNormal = Vector3.zero;    // vars used in loop
            Vector3 a, b, c, n = Vector3.zero;
            int hitFace = -1;
            Vector3 o = InRay.origin, d = InRay.direction;

            // Iterate faces, testing for nearest hit to ray origin.
            for (int CurTri = 0; CurTri < triangles.Length; CurTri += 3)
            {
                a = mesh[triangles[CurTri + 0]];
                b = mesh[triangles[CurTri + 1]];
                c = mesh[triangles[CurTri + 2]];

                var rayIntersectsTriangle2 = ReflectionUtils.CreateDelegate<RayIntersectsTriangle2>("RayIntersectsTriangle2", typeof(Math));
                if (rayIntersectsTriangle2(o, d, a, b, c, ref distance, ref n))
                {
                    if (distance < hitDistance)
                    {
                        hitFace = CurTri / 3;
                        hitDistance = distance;
                        hitNormal = n;
                    }
                }
            }

            hit = new GeometryRaycastHit(hitDistance,
                    InRay.GetPoint(hitDistance),
                    hitNormal,
                    hitFace);

            return hitFace > -1;
        }

        /// <summary>
        /// Returns true if this point in world space is occluded by a triangle on this object.
        /// </summary>
        /// <remarks>This is very slow, do not use.</remarks>
        /// <param name="cam"></param>
        /// <param name="pb"></param>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        internal static bool PointIsOccluded(Camera cam, ProBuilderMesh pb, Vector3 worldPoint)
        {
            Vector3 dir = (cam.transform.position - worldPoint).normalized;

            // move the point slightly towards the camera to avoid colliding with its own triangle
            Ray ray = new Ray(worldPoint + dir * .0001f, dir);

            GeometryRaycastHit hit;

            return FaceRaycast(ray, pb, out hit, Vector3.Distance(cam.transform.position, worldPoint), CullingMode.Front);
        }

    }

}
