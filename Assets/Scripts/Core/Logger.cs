using System.Diagnostics;

namespace ARKitect.Core
{
    /// <summary>
    /// Logs a message to the console. 
    /// This custom class wraps Unity's internal logger and allows to strip the UnityEngine.Debug logging APIs from non-development builds
    /// and avoid writing to log files.
    /// See Unity doc: https://docs.unity3d.com/Manual/UnderstandingPerformanceGeneralOptimizations.html
    /// </summary>
    public static class Logger
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogInfo(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogInfo(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogFormat(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogFormat(UnityEngine.Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(context, message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarningFormat(string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarningFormat(UnityEngine.Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(context, message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogErrorFormat(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogErrorFormat(UnityEngine.Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(context, message, args);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(System.Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(System.Exception exception, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(exception, context);
        }
    }

}