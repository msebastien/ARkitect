using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitect.SettingsManagement
{
    /// <summary>
    /// An interface that represents a user setting.
    /// </summary>
    public interface IUserSetting
    {
        /// <summary>
        /// Implement this property to get the key for this value.
        /// </summary>
        /// <value>The key used to identify the settings entry. This is used along with the <see cref="type"/> to uniquely identify the value.</value>
        string Key { get; }

        /// <summary>
        /// Implement this property to get the type of the stored value.
        /// </summary>
        /// <value>The type of value. This is used along with the <see cref="key"/> to uniquely identify the value.</value>
        Type Type { get; }

        /// <summary>
        /// Implement this property to get the location in the UI where this setting will appear.
        /// </summary>
        /// <value>
        /// Indicates whether this is a <see cref="SettingsScope.Project"/> setting
        /// or a <see cref="SettingsScope.User"/> preference.
        /// </value>
        SettingsScope Scope { get; }

        /// <summary>
        /// Implement this property to get the name of the <see cref="ISettingsRepository"/> that this setting should be associated with.
        /// If null, the first repository matching the <see cref="scope"/> is used.
        /// </summary>
        /// <value>The bare filename of this repository.</value>
        string SettingsRepositoryName { get; }

        /// <summary>
        /// Implement this property to get the <see cref="Settings"/> instance to save and load this setting from.
        /// </summary>
        /// <value>A reference to <see cref="Settings"/> instance.</value>
        Settings Settings { get; }

        /// <summary>
        /// Implement this method to return the stored settings value.
        /// If you are implementing IUserSetting, you should cache this value.
        /// </summary>
        /// <returns>
        /// The stored value.
        /// </returns>
        object GetValue();

        /// <summary>
        /// Implement this method to return the the default value for this setting.
        /// </summary>
        /// <returns>
        /// The default value for this setting.
        /// </returns>
        object GetDefaultValue();

        /// <summary>
        /// Implement this method to set the value for this setting.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="saveProjectSettingsImmediately">
        /// True to immediately serialize the <see cref="ISettingsRepository"/> that is backing this value; or false to postpone.
        /// If not serializing immediately, be sure to call <see cref="Settings.Save"/>.
        /// </param>
        void SetValue(object value, bool saveProjectSettingsImmediately = false);

        /// <summary>
        /// Implement this method to explicitly update the <see cref="ISettingsRepository"/> that is backing this value.
        /// When the inspected type is a reference value, it is possible to change properties without affecting the
        /// backing setting. ApplyModifiedProperties provides a method to force serialize these changes.
        /// </summary>
        void ApplyModifiedProperties();

        /// <summary>
        /// Implement this method to set the current value back to the default.
        /// </summary>
        /// <param name="saveProjectSettingsImmediately">True to immediately re-serialize project settings. By default, no values are updated. </param>
        void Reset(bool saveProjectSettingsImmediately = false);

        /// <summary>
        /// Implement this method to delete the saved setting. This does not clear the current value.
        /// </summary>
        /// <seealso cref="Reset"/>
        /// <param name="saveProjectSettingsImmediately">True to immediately re-serialize project settings. By default, no values are updated.</param>
        void Delete(bool saveProjectSettingsImmediately = false);
    }

    /// <summary>
    /// A generic implementation of <see cref="IUserSetting"/> to use with a <see cref="Settings"/> instance. This default
    /// implementation assumes that the <see cref="Settings"/> instance contains two <see cref="ISettingsRepository"/> interfaces:
    /// - Project settings (<see cref="SettingsScope.Project"/>)
    /// - User preferences (<see cref="SettingsScope.User"/>)
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public class UserSetting<T> : IUserSetting
    {
        bool _initialized;
        string _key;
        string _repository;
        T _value;
        T m_DefaultValue;
        SettingsScope _scope;
        Settings _settings;

        UserSetting() { }

        /// <summary>
        /// Initializes and returns an instance of the UserSetting&lt;T&gt; type.
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/> instance to save and load this setting from.</param>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The default value for this key.</param>
        /// <param name="scope">The scope for this setting. By default, the scope is the project.</param>
        public UserSetting(Settings settings, string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            _key = key;
            _repository = null;
            _value = value;
            _scope = scope;
            _initialized = false;
            _settings = settings;
        }

        /// <summary>
        /// Initializes and returns an instance of the UserSetting&lt;T&gt; type using the specified repository.
        /// </summary>
        /// <param name="settings">The <see cref="Settings"/> instance to save and load this setting from.</param>
        /// <param name="repository">The <see cref="ISettingsRepository"/> name to save and load this setting from. Specify null to save to the first available instance.</param>
        /// <param name="key">The key for this value.</param>
        /// <param name="value">The default value for this key.</param>
        /// <param name="scope">The scope for this setting. By default, the scope is the project.</param>
        public UserSetting(Settings settings, string repository, string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            _key = key;
            _repository = repository;
            _value = value;
            _scope = scope;
            _initialized = false;
            _settings = settings;
        }

        /// <summary>
        /// Gets the key for this value.
        /// </summary>
        /// <seealso cref="IUserSetting.Key"/>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the name of the repository that this setting is saved in.
        /// </summary>
        /// <seealso cref="IUserSetting.SettingsRepositoryName" />
        public string SettingsRepositoryName
        {
            get { return _repository; }
        }

        /// <summary>
        /// Gets the type that this setting represents (&lt;T&gt;).
        /// </summary>
        /// <seealso cref="IUserSetting.Type" />
        public Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Returns a copy of the default value.
        /// </summary>
        /// <returns>
        /// The default value.
        /// </returns>
        /// <seealso cref="IUserSetting.GetDefaultValue" />
        public object GetDefaultValue()
        {
            return DefaultValue;
        }

        /// <summary>
        /// Returns the currently stored value.
        /// </summary>
        /// <returns>
        /// The value that is currently set.
        /// </returns>
        /// <seealso cref="IUserSetting.GetValue" />
        public object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Gets the scope (<see cref="ISettingsRepository"/>) where the <see cref="Settings"/> instance saves
        /// its data.
        /// </summary>
        /// <seealso cref="IUserSetting.Scope" />
        public SettingsScope Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// Gets the <see cref="Settings"/> instance to read from and save to.
        /// </summary>
        /// <seealso cref="IUserSetting.Settings" />
        public Settings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Sets the value for this setting from the specified object.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="saveProjectSettingsImmediately">
        /// Set this value to true if you want to immediately serialize the <see cref="ISettingsRepository"/>
        /// that is backing this value. By default, this is false.
        ///
        /// **Note**: If not serializing immediately, you need to call <see cref="Settings.Save"/>.
        /// </param>
        /// <seealso cref="IUserSetting.SetValue" />
        public void SetValue(object value, bool saveProjectSettingsImmediately = false)
        {
            // we do want to allow null values
            if (value != null && !(value is T))
                throw new ArgumentException("Value must be of type " + typeof(T) + "\n" + Key + " expecting value of type " + Type + ", received " + value.GetType());
            SetValue((T)value, saveProjectSettingsImmediately);
        }

        /// <inheritdoc cref="SetValue" />
        public void SetValue(T value, bool saveProjectSettingsImmediately = false)
        {
            Init();
            _value = value;
            Settings.Set<T>(Key, _value, _scope);

            if (saveProjectSettingsImmediately)
                Settings.Save();
        }

        /// <summary>
        /// Deletes the saved setting but doesn't clear the current value.
        /// </summary>
        /// <param name="saveProjectSettingsImmediately">
        /// Set this value to true if you want to immediately serialize the <see cref="ISettingsRepository"/>
        /// that is backing this value. By default, this is false.
        ///
        /// **Note**: If not serializing immediately, you need to call <see cref="Settings.Save"/>.
        /// </param>
        /// <seealso cref="Reset" />
        /// <seealso cref="IUserSetting.Delete"/>
        public void Delete(bool saveProjectSettingsImmediately = false)
        {
            Settings.DeleteKey<T>(Key, Scope);
            // Don't Init() because that will set the key again. We just want to reset the m_Value with default and
            // pretend that this field hasn't been initialised yet.
            _value = ValueWrapper<T>.DeepCopy(m_DefaultValue);
            _initialized = false;
        }

        /// <summary>
        /// Forces Unity to serialize the changed properties to the <see cref="ISettingsRepository"/> that is backing this value.
        /// When the inspected type is a reference value, it is possible to change properties without affecting the
        /// backing setting.
        /// </summary>
        /// <seealso cref="IUserSetting.ApplyModifiedProperties"/>
        public void ApplyModifiedProperties()
        {
            Settings.Set<T>(Key, _value, _scope);
            Settings.Save();
        }

        /// <summary>
        /// Sets the current value back to the default.
        /// </summary>
        /// <param name="saveProjectSettingsImmediately">
        /// Set this value to true if you want to immediately serialize the <see cref="ISettingsRepository"/>
        /// that is backing this value. By default, this is false.
        ///
        /// **Note**: If not serializing immediately, you need to call <see cref="Settings.Save"/>.
        /// </param>
        /// <seealso cref="IUserSetting.Reset"/>
        public void Reset(bool saveProjectSettingsImmediately = false)
        {
            SetValue(DefaultValue, saveProjectSettingsImmediately);
        }

        void Init()
        {
            if (!_initialized)
            {
                if (_scope == SettingsScope.Project && Settings == null)
                    throw new Exception("UserSetting \"" + _key + "\" is attempting to access SettingsScope.Project setting with no Settings instance!");

                _initialized = true;

                // DeepCopy uses JsonUtility which is not permitted during construction
                m_DefaultValue = ValueWrapper<T>.DeepCopy(_value);

                if (Settings.ContainsKey<T>(_key, _scope))
                    _value = Settings.Get<T>(_key, _scope);
                else
                    Settings.Set<T>(_key, _value, _scope);
            }
        }

        /// <summary>
        /// Gets the default value for this setting.
        /// </summary>
        public T DefaultValue
        {
            get
            {
                Init();
                return ValueWrapper<T>.DeepCopy(m_DefaultValue);
            }
        }

        /// <summary>
        /// Gets or sets the currently stored value.
        /// </summary>
        public T Value
        {
            get
            {
                Init();
                return _value;
            }

            set { SetValue(value); }
        }

        /// <summary>
        /// Implicit casts this setting to the backing type `T`.
        /// </summary>
        /// <param name="pref">The UserSetting&lt;T&gt; to cast to `T`.</param>
        /// <returns>
        /// The currently stored <see cref="value"/>.
        /// </returns>
        public static implicit operator T(UserSetting<T> pref)
        {
            return pref.Value;
        }

        /// <summary>
        /// Returns a string representation of this setting.
        /// </summary>
        /// <returns>A string summary of this setting of format "[scope] setting. Key: [key]  Value: [value]".</returns>
        public override string ToString()
        {
            return string.Format("{0} setting. Key: {1}  Value: {2}", Scope, Key, Value);
        }
    }

}
