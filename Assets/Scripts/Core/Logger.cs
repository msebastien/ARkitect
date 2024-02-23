#if DEBUG || UNITY_EDITOR
#define ENABLE_LOGS
#endif

using System;
using System.Diagnostics;
using UnityEngine;

namespace ARKitect.Core
{
    /// <summary>
    /// Logs a message to the console. 
    /// This custom class allows to strip the UnityEngine.Debug logging APIs from non-development builds
    /// and avoid writing to log files.
    /// See Unity doc: https://docs.unity3d.com/Manual/UnderstandingPerformanceGeneralOptimizations.html
    /// </summary>
    public static class Logger
    {
        [Conditional("ENABLE_LOGS")]
        public static void LogInfo(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }

}