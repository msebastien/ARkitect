using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            return go.GetInstanceID();
        }
    }

}
