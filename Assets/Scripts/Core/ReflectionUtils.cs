using System;
using System.Reflection;
using UnityEngine;

namespace ARKitect.Core
{
    /// <summary>
    /// Reflection allows to obtain information about loaded assemblies and the types defined within them, 
    /// such as classes, interfaces, and value types (that is, structures and enumerations).
    /// It can also be used to create type instances at run time, and to invoke and access them.
    /// It enables to access and set private fields' value but also invoke private methods.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Get the value of a private field in an object by using reflection
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="U">Field type</typeparam>
        /// <param name="fieldName">Field name</param>
        /// <param name="instance">Object instance from which to retrieve the field value</param>
        /// <returns>Field value</returns>
        public static U GetPrivateFieldValue<T, U>(string fieldName, T instance)
            where T : class
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (U)field.GetValue(instance);
        }

        /// <summary>
        /// Set the value of a private field in an object by using reflection
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="fieldName">Field name</param>
        /// <param name="instance">Object instance on which to set the new value of the specified field</param>
        /// <param name="value">Value to set</param>
        public static void SetPrivateFieldValue<T>(string fieldName, T instance, object value)
            where T : class
        {
            Type type = typeof(T);
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(instance, value);
        }

        /// <summary>
        /// Invoke a private method, which returns nothing, from an object by using reflection
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="methodName">Method name</param>
        /// <param name="instance">Object instance from which to invoke the method</param>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Value returned by the method</returns>
        public static void InvokePrivateMethod<T>(string methodName, T instance, params object[] parameters)
            where T : class
        {
            Type type = typeof(T);
            MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Invoke a private method, which returns a value, from an object by using reflection
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <typeparam name="U">Return value type</typeparam>
        /// <param name="methodName">Method name</param>
        /// <param name="instance">Object instance from which to invoke the method</param>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Value returned by the method</returns>
        public static U InvokePrivateMethod<T, U>(string methodName, T instance, params object[] parameters)
            where T : class
        {
            Type type = typeof(T);
            MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return (U)method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Invoke a static private method, which returns nothing, from a static class by using reflection
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="classType">Type of the class</param>
        /// <param name="parameters">Parameters' value</param>
        public static void InvokeStaticClassPrivateMethod(string methodName, Type classType, params object[] parameters)
        {
            MethodInfo method = classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, parameters);
        }

        /// <summary>
        /// Invoke a static private method, which returns nothing, from a static class by using reflection
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="classType">Type of the class</param>
        /// <param name="paramTypes">Types for each method parameter</param>
        /// <param name="parameters">Parameters' value</param>
        public static void InvokeStaticClassPrivateMethod(string methodName, Type classType, Type[] paramTypes, params object[] parameters)
        {
            MethodInfo method = classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, paramTypes, null);
            method.Invoke(null, parameters);
        }

        /// <summary>
        /// Invoke a static private method, which returns a value, from a static class by using reflection
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="methodName">Method name</param>
        /// <param name="classType">Type of the class</param>
        /// <param name="parameters">Parameters' value</param>
        /// <returns>Value returned by the method</returns>
        public static T InvokeStaticClassPrivateMethod<T>(string methodName, Type classType, params object[] parameters)
        {
            MethodInfo method = classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            return (T)method.Invoke(null, parameters);
        }

        /// <summary>
        /// Invoke a static private method, which returns a value, from a static class by using reflection
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="methodName">Method name</param>
        /// <param name="classType">Type of the class</param>
        /// <param name="paramTypes">Types for each method parameter</param>
        /// <param name="parameters">Parameters' value</param>
        /// <returns>Value returned by the method</returns>
        public static T InvokeStaticClassPrivateMethod<T>(string methodName, Type classType, Type[] paramTypes, params object[] parameters)
        {
            MethodInfo method = classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, paramTypes, null);
            return (T)method.Invoke(null, parameters);
        }

        public static T CreateDelegate<T>(string methodName, Type classType, object instance = null)
            where T : Delegate
        {
            return (T)Delegate.CreateDelegate(typeof(T), instance, classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
        }

        public static T CreateDelegate<T>(string methodName, Type classType, Type[] paramTypes, object instance = null)
            where T : Delegate
        {
            return (T)Delegate.CreateDelegate(typeof(T), instance, classType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, paramTypes, null));
        }

    }

}
