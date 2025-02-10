using System;
using UnityEngine;

namespace ARKitect.SettingsManagement
{
    [Serializable]
    public sealed class ValueWrapper<T>
    {
#if PRETTY_PRINT_JSON
        const bool PrettyPrintJson = true;
#else
        const bool PrettyPrintJson = false;
#endif
        [SerializeField]
        T _value;

        public static string Serialize(T value)
        {
            var obj = new ValueWrapper<T>() { _value = value };
            return JsonUtility.ToJson(obj, PrettyPrintJson);
        }

        public static T Deserialize(string json)
        {
            var value = (object)Activator.CreateInstance<ValueWrapper<T>>();
            JsonUtility.FromJsonOverwrite(json, value);
            return ((ValueWrapper<T>)value)._value;
        }

        public static T DeepCopy(T value)
        {
            if (typeof(ValueType).IsAssignableFrom(typeof(T)))
                return value;
            var str = Serialize(value);
            return Deserialize(str);
        }
    }

}
