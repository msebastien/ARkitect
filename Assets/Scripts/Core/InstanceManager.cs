using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using Sirenix.OdinInspector;

namespace ARKitect.Core
{
    /// <summary>
    /// Spawn objects and manage object instances
    /// </summary>
    [AddComponentMenu("ARkitect/Instance Manager")]
    public class InstanceManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        [Tooltip("Parent of the spawned objects")]
        private Transform _instancesParent;
        public Transform InstancesParent => _instancesParent;

        [Header("Objects")]
        [DictionaryDrawerSettings(KeyLabel = "Instance ID", ValueLabel = "GameObject")]
        [Tooltip("Cached object instances")]
        [SerializeField]
        private Dictionary<int, GameObject> _instances = new Dictionary<int, GameObject>();

        /// <summary>
        /// Destroy the specified instance using its Id
        /// </summary>
        /// <param name="id">Id of the instantiated object</param>
        /// <returns>'true' if it succeeded, else 'false'</returns>
        public bool DestroyInstance(int id)
        {
            bool ret = false;

            if (_instances.TryGetValue(id, out GameObject instance))
            {
                Destroy(instance);
                _instances.Remove(id);
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Instantiate a GameObject and save it
        /// </summary>
        /// <param name="obj">GameObject to instantiate</param>
        /// <param name="position">Coordinates in World Space</param>
        /// <param name="rotation">Rotation</param>
        /// <returns>Instance ID</returns>
        public int Spawn(GameObject obj, Vector3 position, Quaternion rotation)
        {
            var go = Instantiate(obj, position, rotation, _instancesParent);
            _instances.Add(go.GetInstanceID(), go);

            ConvertToProbuilderMesh(go);

            return go.GetInstanceID();
        }

        private void ConvertToProbuilderMesh(GameObject go)
        {
            new MeshImporter(go).Import(); // default import settings

            var pbMesh = go.GetComponent<ProBuilderMesh>();

            // Rebuild mesh positions and submeshes
            pbMesh.ToMesh(MeshTopology.Quads);

            // Recalculate UVs, Normals, Tangents, Collisions, then apply to Unity Mesh.
            pbMesh.Refresh();

            // If in Editor, generate UV2 and collapse duplicate vertices.
#if UNITY_EDITOR
            EditorMeshUtility.Optimize(pbMesh, true);
#else
        // At runtime, `EditorMeshUtility` is not available. To collapse duplicate
        // vertices in runtime, modify the MeshFilter.sharedMesh directly.
        // Note that any subsequent changes to `quad` will overwrite the sharedMesh.
        var umesh = pbMesh.GetComponent<MeshFilter>().sharedMesh;
        MeshUtility.CollapseSharedVertices(umesh);    
#endif
        }
    }

}
