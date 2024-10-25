using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

        //[Header("Objects")]
        //[DictionaryDrawerSettings(KeyLabel = "Instance ID", ValueLabel = "GameObject")]
        //[Tooltip("Cached object instances")]
        //[SerializeField]
        private Dictionary<Guid, GameObject> _instances = new Dictionary<Guid, GameObject>();

        public GameObject GetInstance(Guid guid)
        {
            GameObject go = null;

            if (_instances.TryGetValue(guid, out GameObject instance))
                go = instance;

            return go;
        }

        /// <summary>
        /// Destroy the specified instance using its Id
        /// </summary>
        /// <param name="guid">Unique Id of the instantiated object</param>
        /// <returns>'true' if it succeeded, else 'false'</returns>
        public bool DestroyInstance(Guid guid)
        {
            bool ret = false;

            if (_instances.TryGetValue(guid, out GameObject instance))
            {
                Destroy(instance);
                _instances.Remove(guid);
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Create a new instance of a GameObject with a random Guid and save it
        /// </summary>
        /// <param name="obj">GameObject to instantiate</param>
        /// <param name="position">Coordinates in World Space</param>
        /// <param name="rotation">Rotation</param>
        /// <returns>Instance ID</returns>
        public Guid Spawn(GameObject obj, Vector3 position, Quaternion rotation)
        {
            var guid = Guid.NewGuid();
            Spawn(guid, obj, position, rotation);

            return guid;
        }

        /// <summary>
        /// Create a new instance of a GameObject with a specified Guid and save it
        /// </summary>
        /// <param name="guid">Unique Instance ID</param>
        /// <param name="obj">GameObject to instantiate</param>
        /// <param name="position">Coordinates in World Space</param>
        /// <param name="rotation">Rotation</param>
        public void Spawn(Guid guid, GameObject obj, Vector3 position, Quaternion rotation)
        {
            var go = Instantiate(obj, position, rotation, _instancesParent);
            _instances.Add(guid, go);
            ARKitectApp.Instance.GeometryProviderManager.InitProvider(go);
        }
    }

}
