using UnityEngine;

namespace ARKitect.Core
{
    /// <summary>
    /// Make sure only one object instance is created and accessible
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                // If the instance has not initialized
                if (_instance == null)
                {
                    var objs = FindObjectsOfType<T>();

                    // If multiple instances of the same class have been found, keep the first one
                    if (objs.Length > 0)
                    {
                        _instance = objs[0];
                    }

                    // Warn that multiple instances of the same singleton class exist, which SHOULD NOT happen
                    if (objs.Length > 1)
                    {
                        Logger.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }

                    // Initialize a new instance
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = string.Format("_{0}", typeof(T).Name);
                        obj.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }

}