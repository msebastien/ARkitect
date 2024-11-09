#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using ARKitect.Core;

namespace ARKitect.Debug
{
    public class InstanceDebug : SerializedMonoBehaviour
    {
        [Header("Objects")]
        [DictionaryDrawerSettings(KeyLabel = "Instance ID", ValueLabel = "GameObject")]
        [Tooltip("Cached object instances")]
        [SerializeField]
        private Dictionary<Guid, GameObject> _instances;

        private InstanceManager _instanceManager;

        private void Awake()
        {
            _instanceManager = ARKitectApp.Instance.InstanceManager;
        }

        private void Start()
        {
            _instances = ReflectionUtils.GetPrivateFieldValue<InstanceManager, Dictionary<Guid, GameObject>>("_instances", _instanceManager);
        }

    }

}
#endif
