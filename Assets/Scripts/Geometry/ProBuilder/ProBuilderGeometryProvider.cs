using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

#if UNITY_EDITOR
using UnityEditor.ProBuilder;
#endif

using Logger = ARKitect.Core.Logger;

namespace ARKitect.Geometry.Providers.ProBuilder
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ProBuilderGeometryProvider : MonoBehaviour, IGeometryProvider
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
        public int SetFaceMaterial(Vector3 screenPos, Material material)
        {
            var face = SelectionPicker.PickFace(Camera.main, screenPos, _pbMesh);
            _pbMesh.SetMaterial(new Face[] { face }, material);
            ApplyChanges();
            return face.submeshIndex;
        }

        public void SetFaceMaterial(int submeshIndex, Material material)
        {
            var materials = _renderer.sharedMaterials;
            var submeshCount = materials.Length;
            var index = -1;

            for (int i = 0; i < submeshCount && index < 0; i++)
            {
                if (materials[i] == material)
                    index = i;
            }

            if (index < 0) // Material doesn't exist in MeshRenderer.sharedMaterials, so use the specified subMeshIndex
            {
                if (submeshIndex > 0 && submeshIndex < submeshCount)
                {
                    materials[submeshIndex] = material;
                    _renderer.sharedMaterials = materials;
                }
                else
                {
                    Logger.LogError("SubmeshIndex is negative");
                }
            }
            else // Material already exists, so use his submeshIndex instead to avoid duplicates
            {
                materials[index] = material;
                _renderer.sharedMaterials = materials;
            }

            ApplyChanges();
        }

        public Material GetFaceMaterial(Vector3 screenPos)
        {
            var face = SelectionPicker.PickFace(Camera.main, screenPos, _pbMesh);
            return GetFaceMaterial(face.submeshIndex);
        }

        public Material GetFaceMaterial(int submeshIndex)
        {
            Material mat = BuiltinMaterials.defaultMaterial;

            if (submeshIndex > 0 && submeshIndex < _renderer.materials.Length)
                mat = _renderer.materials[submeshIndex];

            return mat;
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
