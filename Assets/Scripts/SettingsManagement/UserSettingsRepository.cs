using UnityEngine;

namespace ARKitect.SettingsManagement
{
    /// <summary>
    /// Represents a settings repository for user preferences.
    /// </summary>
    /// <seealso cref="UnityEngine.PlayerPrefs"/>
    public class UserSettingsRepository : ISettingsRepository
    {
        static string GetPlayerPrefKey<T>(string key)
        {
            return GetPlayerPrefKey(typeof(T).FullName, key);
        }

        static string GetPlayerPrefKey(string fullName, string key)
        {
            return fullName + "::" + key;
        }

        static void SetPlayerPref<T>(string key, T value)
        {
            var k = GetPlayerPrefKey<T>(key);

            if (typeof(T) == typeof(string))
                PlayerPrefs.SetString(key, (string)(object)value);
            else if (typeof(T) == typeof(bool))
                PlayerPrefs.SetInt(key, (bool)(object)value ? 1 : 0);
            else if (typeof(T) == typeof(float))
                PlayerPrefs.SetFloat(key, (float)(object)value);
            else if (typeof(T) == typeof(int))
                PlayerPrefs.SetInt(key, (int)(object)value);
            else
                PlayerPrefs.SetString(key, ValueWrapper<T>.Serialize(value));
        }

        static T GetPlayerPref<T>(string key, T fallback = default(T))
        {
            var k = GetPlayerPrefKey<T>(key);

            if (!PlayerPrefs.HasKey(k))
                return fallback;

            var o = (object)fallback;

            if (typeof(T) == typeof(string))
                o = PlayerPrefs.GetString(k, (string)o);
            else if (typeof(T) == typeof(bool))
                o = PlayerPrefs.GetInt(k, (int)o) > 0 ? true : false;
            else if (typeof(T) == typeof(float))
                o = PlayerPrefs.GetFloat(k, (float)o);
            else if (typeof(T) == typeof(int))
                o = PlayerPrefs.GetInt(k, (int)o);
            else
                o = ValueWrapper<T>.Deserialize(PlayerPrefs.GetString(k));

            return (T)o;
        }

        /// <summary>
        /// Gets the <see cref="SettingsScope">scope</see> this repository applies to.
        /// </summary>
        /// <value>Indicates that this is a <see cref="SettingsScope.User"/> preference.</value>
        /// <seealso cref="ISettingsRepository.Scope"/>
        public SettingsScope Scope
        {
            get { return SettingsScope.User; }
        }

        /// <summary>
        /// Gets the identifying name for this repository.
        /// </summary>
        /// <value>User settings are named "PlayerPrefs".</value>
        public string Name
        {
            get { return "PlayerPrefs"; }
        }

        /// <summary>
        /// Gets the full path to the file containing the serialized settings data.
        /// </summary>
        /// <remarks>This property returns an empty string.</remarks>
        /// <value>The location stored for this repository.</value>
        /// <seealso cref="ISettingsRepository.Path"/>
        public string Path
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Saves all settings to their serialized state.
        /// </summary>
        /// <seealso cref="ISettingsRepository.Save"/>
        public void Save()
        {
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This must be serializable.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void Set<T>(string key, T value)
        {
            SetPlayerPref<T>(key, value);
        }

        /// <summary>
        /// Returns a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The value matching both `key` and type `T`. If there was no match, this returns the `fallback` value.</returns>
        public T Get<T>(string key, T fallback = default(T))
        {
            return GetPlayerPref<T>(key, fallback);
        }

        /// <summary>
        /// Determines whether this repository contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a settings entry matches both `key` and type `T`; false if no entry is found.</returns>
        public bool ContainsKey<T>(string key)
        {
            return PlayerPrefs.HasKey(GetPlayerPrefKey<T>(key));
        }

        /// <summary>
        /// Removes a key-value pair from this settings repository. This method identifies the settings entry to remove
        /// by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        public void Remove<T>(string key)
        {
            PlayerPrefs.DeleteKey(GetPlayerPrefKey<T>(key));
        }
    }

}
