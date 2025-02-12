using System;
using System.IO;
using PathUtils = System.IO.Path;
using UnityEngine;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.SettingsManagement
{
    /// <summary>
    /// Represents a settings repository that stores data serialized to a JSON file.
    /// </summary>
    [Serializable]
    public class FileSettingsRepository : ISettingsRepository
    {
        protected const string DirectoryName = "ProjectSettings";

        /// <summary>
        /// Location of where the project-specific settings are saved under the `ProjectSettings` directory.
        /// </summary>
        /// <returns>Per-project user settings directory. </returns>
        protected static readonly string ProjectSettingsPath =
            Application.persistentDataPath + PathUtils.DirectorySeparatorChar + DirectoryName;

        const bool PrettyPrintJson = true;

        bool _initialized;
        string _path;
        [SerializeField]
        SettingsDictionary _dictionary = new SettingsDictionary();
        Hash128 _jsonHash;

        /// <summary>
        /// Initializes and returns an instance of the FileSettingsRepository
        /// with the serialized data location set to the specified path.
        /// </summary>
        /// <param name="path">The project-relative path to save settings to.</param>
        public FileSettingsRepository(string path)
        {
            _path = path;
            _initialized = false;
            Application.quitting += Save;
        }

        void Init()
        {
            if (_initialized)
                return;

            _initialized = true;

            if (TryLoadSavedJson(out string json))
            {
                _dictionary = null;
                _jsonHash = Hash128.Compute(json);
                JsonUtility.FromJsonOverwrite(json, this);
            }

            if (_dictionary == null)
                _dictionary = new SettingsDictionary();
        }

        /// <summary>
        /// Sets the <see cref="SettingsScope"/> this repository applies to.
        /// </summary>
        /// <remarks>
        /// By default, this repository implementation is relevant to the Project scope, but any implementations
        /// that override this method can choose to store this serialized data at a user scope instead.
        /// </remarks>
        /// <value>
        /// <see cref="SettingsScope.Project"/>, meaning that this setting applies to project settings (the default);
        /// or <see cref="SettingsScope.User"/>, meaning that this setting applies to user preferences.
        /// </value>
        /// <seealso cref="ISettingsRepository.Scope"/>
        public virtual SettingsScope Scope => SettingsScope.Project;

        /// <summary>
        /// Gets the full path to the file containing the serialized settings data.
        /// </summary>
        /// <value>The location stored for this repository.</value>
        /// <seealso cref="ISettingsRepository.Path"/>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Sets the name of file containing the serialized settings data.
        /// </summary>
        /// <value>The bare filename of the settings file.</value>
        public string Name => PathUtils.GetFileNameWithoutExtension(Path);

        /// <summary>
        /// Loads the JSON file that stores the values for this settings object.
        /// </summary>
        /// <param name="json">JSON string read from the file</param>
        /// <returns>True if the file exists; false if it doesn't.</returns>
        public bool TryLoadSavedJson(out string json)
        {
            json = string.Empty;
            if (!File.Exists(Path))
                return false;
            json = File.ReadAllText(Path);
            return true;
        }

        /// <summary>
        /// Saves all settings to their serialized state.
        /// </summary>
        /// <seealso cref="ISettingsRepository.Save"/>
        public void Save()
        {
            Init();

            if (!File.Exists(Path))
            {
                var directory = PathUtils.GetDirectoryName(Path);

                if (string.IsNullOrEmpty(directory))
                {
                    Logger.LogError(
                        $"Settings file {Name} is saved to an invalid path: {Path}. Settings will not be saved.");
                    return;
                }

                Directory.CreateDirectory(directory);

                string json = JsonUtility.ToJson(this, PrettyPrintJson);

                // While unlikely, a hash collision is possible. Always test the actual saved contents before early exit.
                if (_jsonHash == Hash128.Compute(json)
                    && TryLoadSavedJson(out string existing)
                    && existing.Equals(json))
                    return;

                try
                {
                    _jsonHash = Hash128.Compute(json);
                    File.WriteAllText(Path, json);
                }
                catch (UnauthorizedAccessException)
                {
                    Logger.LogWarning($"Could not save project settings to {Path}");
                }
            }
        }

        /// <summary>
        /// Sets a value for a settings entry with a matching key and type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="value">The value to set. This value must be serializable.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <seealso cref="ISettingsRepository.Set{T}"/>
        public void Set<T>(string key, T value)
        {
            Init();
            _dictionary.Set<T>(key, value);
        }

        /// <summary>
        /// Returns a value with key of type `T`, or the fallback value if no matching key is found.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <param name="fallback">Specify the value of type `T` to return if the entry can't be found.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>The settings value if a match is found; otherwise, it returns the default (fallback) value.</returns>
        /// <seealso cref="ISettingsRepository.Get{T}"/>
        public T Get<T>(string key, T fallback = default(T))
        {
            Init();
            return _dictionary.Get<T>(key, fallback);
        }

        /// <summary>
        /// Determines whether this repository contains a settings entry that matches the specified key and is of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <returns>True if a match is found for both key and type; false if no entry is found.</returns>
        /// <seealso cref="ISettingsRepository.ContainsKey{T}"/>
        public bool ContainsKey<T>(string key)
        {
            Init();
            return _dictionary.ContainsKey<T>(key);
        }

        /// <summary>
        /// Removes a key-value pair from the settings repository. This method identifies the settings entry to remove
        /// by matching the specified key for a value of type `T`.
        /// </summary>
        /// <param name="key">The key used to identify the settings entry.</param>
        /// <typeparam name="T">The type of value that this key points to.</typeparam>
        /// <seealso cref="ISettingsRepository.Remove{T}"/>
        public void Remove<T>(string key)
        {
            Init();
            _dictionary.Remove<T>(key);
        }

    }

}
