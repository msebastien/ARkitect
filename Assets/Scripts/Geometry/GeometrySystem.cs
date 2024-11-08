using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

#if UNITY_EDITOR
using UnityEditor.ProBuilder;
#endif

using Logger = ARKitect.Core.Logger;

namespace ARKitect.Geometry
{
    [RequireComponent(typeof(MeshRenderer))]
    public class GeometrySystem : MonoBehaviour
    {
        private ProBuilderMesh _pbMesh;
        private MeshRenderer _renderer;

        private void Awake() => _renderer = GetComponent<MeshRenderer>();

        public void Init()
        {
            new MeshImporter(gameObject).Import(); // default import settings

            _pbMesh = gameObject.GetComponent<ProBuilderMesh>();

            ApplyChanges();
        }

        /// <summary>
        /// Set the material of the first face on a ProBuilder mesh that collides with the ray cast by ProBuilder.
        /// </summary>
        /// <param name="screenPos"></param>
        /// <param name="material"></param>
        public Face SetFaceMaterial(Vector3 screenPos, Material material)
        {
            var face = GetFace(screenPos);
            _pbMesh.SetMaterial(new Face[] { face }, material);
            ApplyChanges();
            return face;
        }

        public void SetFaceMaterial(Face face, Material material)
        {
            _pbMesh.SetMaterial(new Face[] { face }, material);
            ApplyChanges();
        }

        public Material GetFaceMaterial(Face face)
        {
            return _renderer.materials[face.submeshIndex];
        }

        public Face GetFace(Vector3 screenPos)
        {
            return SelectionPicker.PickFace(Camera.main, screenPos, _pbMesh);
        }

        public Face GetFace(Ray ray)
        {
            throw new NotImplementedException();
        }

        public void ApplyChanges()
        {
            // Rebuild mesh positions and submeshes
            _pbMesh.ToMesh(MeshTopology.Quads);

            // Recalculate UVs, Normals, Tangents, Collisions, then apply to Unity Mesh.
            _pbMesh.Refresh();

            // If in Editor, generate UV2 and collapse duplicate vertices.
#if UNITY_EDITOR
            EditorMeshUtility.Optimize(_pbMesh, true);
#else
            // At runtime, `EditorMeshUtility` is not available. To collapse duplicate
            // vertices in runtime, modify the MeshFilter.sharedMesh directly.
            // Note that any subsequent changes to `quad` will overwrite the sharedMesh.
            var umesh = _pbMesh.GetComponent<MeshFilter>().sharedMesh;
            MeshUtility.CollapseSharedVertices(umesh);    
#endif
        }



    }

}
