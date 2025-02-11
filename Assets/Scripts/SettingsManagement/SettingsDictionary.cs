using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.SettingsManagement
{
    [Serializable]
    sealed class SettingsDictionary : ISerializationCallbackReceiver
    {
        [Serializable]
        struct SettingsKeyValuePair
        {
            public string type;
            public string key;
            public string value;
        }

#pragma warning disable 0649
        [SerializeField]
        List<SettingsKeyValuePair> _dictionaryValues = new List<SettingsKeyValuePair>();
#pragma warning restore 0649

        internal Dictionary<Type, Dictionary<string, string>> dictionary = new Dictionary<Type, Dictionary<string, string>>();

        public bool ContainsKey<T>(string key)
        {
            return dictionary.ContainsKey(typeof(T)) && dictionary[typeof(T)].ContainsKey(key);
        }

        public void Set<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var type = typeof(T).AssemblyQualifiedName;

            SetJson(type, key, ValueWrapper<T>.Serialize(value));
        }

        internal void SetJson(string type, string key, string value)
        {
            var typeValue = Type.GetType(type);

            if (typeValue == null)
                throw new ArgumentException("\"type\" must be an assembly qualified type name.");

            Dictionary<string, string> entries;

            if (!dictionary.TryGetValue(typeValue, out entries))
                dictionary.Add(typeValue, entries = new Dictionary<string, string>());

            if (entries.ContainsKey(key))
                entries[key] = value;
            else
                entries.Add(key, value);
        }

        public T Get<T>(string key, T fallback = default(T))
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            Dictionary<string, string> entries;

            if (dictionary.TryGetValue(typeof(T), out entries) && entries.ContainsKey(key))
            {
                try
                {
                    return ValueWrapper<T>.Deserialize(entries[key]);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public void Remove<T>(string key)
        {
            Dictionary<string, string> entries;

            if (!dictionary.TryGetValue(typeof(T), out entries) || !entries.ContainsKey(key))
                return;

            entries.Remove(key);
        }

        public void OnBeforeSerialize()
        {
            if (_dictionaryValues == null)
                return;

            _dictionaryValues.Clear();

            foreach (var type in dictionary)
            {
                foreach (var entry in type.Value)
                {
                    _dictionaryValues.Add(new SettingsKeyValuePair()
                    {
                        type = type.Key.AssemblyQualifiedName,
                        key = entry.Key,
                        value = entry.Value
                    });
                }
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();

            foreach (var entry in _dictionaryValues)
            {
                Dictionary<string, string> entries;

                var type = Type.GetType(entry.type);

                if (type == null)
                {
                    Logger.LogWarning("Could not instantiate type \"" + entry.type + "\". Skipping key: " + entry.key + ".");
                    continue;
                }

                if (dictionary.TryGetValue(type, out entries))
                    entries.Add(entry.key, entry.value);
                else
                    dictionary.Add(type, new Dictionary<string, string>() { { entry.key, entry.value } });
            }
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            foreach (var type in dictionary)
            {
                sb.AppendLine("Type: " + type.Key);

                foreach (var entry in type.Value)
                {
                    sb.AppendLine(string.Format("   {0,-64}{1}", entry.Key, entry.Value));
                }
            }

            return sb.ToString();
        }
    }

}
