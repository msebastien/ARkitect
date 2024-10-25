using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using ARKitect.Core;


namespace ARKitect.Debug
{
#if UNITY_EDITOR
    public class InstanceDebug : SerializedMonoBehaviour
    {
        [Header("Objects")]
        [DictionaryDrawerSettings(KeyLabel = "Instance ID", ValueLabel = "GameObject")]
        [Tooltip("Cached object instances")]
        [SerializeField]
        private Dictionary<Guid, GameObject> _instances;

        private InstanceManager _instanceManager;
        private int instanceFieldIndex = 0;

        private void Awake()
        {
            _instanceManager = ARKitectApp.Instance.InstanceManager;
        }

        private void Start()
        {
            _instances = GetInstanceField();
        }

        private Dictionary<Guid, GameObject> GetInstanceField()
        {
            FieldInfo[] fields;

            // Get fields of InstanceManager
            Type type = typeof(InstanceManager);
            fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            // Find field index
            int i;
            for (i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name == "_instances")
                {
                    instanceFieldIndex = i;
                    break;
                }
            }

            return (Dictionary<Guid, GameObject>)fields[instanceFieldIndex].GetValue(_instanceManager);
        }
    }
#endif
}
