using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARKitect.Core
{
    // Simple scrolling window that prints to the application screen whatever is printed through
    // calls to the UnityEngine.Debug.Log method.
    public class ScrollingLogger : Singleton<ScrollingLogger>
    {
        /// Font size for log text entries. Spacing between log entries is also set to half this value.
        [SerializeField]
        private int LogEntryFontSize = 32;

        /// The maximum number of log entries to keep history of. Setting to -1 will save all entries
        [SerializeField]
        private int MaxLogCount = -1;

        /// Layout box containing the log entries
        [SerializeField]
        private VerticalLayoutGroup LogHistory = null;

        /// Log entry prefab used to generate new entries when requested
        [SerializeField]
        private TMP_Text LogEntryPrefab = null;

        private readonly List<TMP_Text> _logEntries = new List<TMP_Text>();

        private RectTransform rectTransform;

        private bool moveIn, moveOut;
        private float animationStep = 1500.0f;

        private Vector2 offsetMinLogOpen = new Vector2(0.0f, 0.0f);
        private Vector2 offsetMinLogClosed = new Vector2(0.0f, -500f);
        private Vector2 offsetMaxLogOpen = new Vector2(0.0f, 500f);
        private Vector2 offsetMaxLogClosed = new Vector2(0.0f, 0.0f);

        private bool init = true;

        private void Awake()
        {
            LogEntryPrefab.gameObject.SetActive(false);

            // Using logMessageReceived (instead of logMessageReceivedThreaded) to ensure that
            // HandleDebugLog is only called from one thread (the main thread).
            //Application.logMessageReceived += AddLogEntry;
            rectTransform = GetComponent<RectTransform>();
        }

        protected void Start()
        {
            LogHistory.spacing = LogEntryFontSize / 2f;
        }

        private void OnEnable()
        {
            if (init)
            {
                string line = $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled.</color>\n";
                AddLogEntry(line);
                init = false;
            }
        }

        private void OnDestroy()
        {
            //Application.logMessageReceived -= AddLogEntry;
        }

        public void ShowOrHideLogScreen()
        {
            if (rectTransform.offsetMin.y <= offsetMinLogClosed.y)
            {
                moveIn = true;
            }
            else if (rectTransform.offsetMin.y >= offsetMinLogOpen.y)
            {
                moveOut = true;
            }

        }

        void Update()
        {
            if (moveIn)
            {
                rectTransform.offsetMin = Vector2.MoveTowards(rectTransform.offsetMin, offsetMinLogOpen, animationStep * Time.deltaTime);
                rectTransform.offsetMax = Vector2.MoveTowards(rectTransform.offsetMax, offsetMaxLogOpen, animationStep * Time.deltaTime);
                if (rectTransform.offsetMin.y >= offsetMinLogOpen.y)
                    moveIn = false;
            }
            else if (moveOut)
            {
                rectTransform.offsetMin = Vector2.MoveTowards(rectTransform.offsetMin, offsetMinLogClosed, animationStep * Time.deltaTime);
                rectTransform.offsetMax = Vector2.MoveTowards(rectTransform.offsetMax, offsetMaxLogClosed, animationStep * Time.deltaTime);
                if (rectTransform.offsetMin.y <= offsetMinLogClosed.y)
                    moveOut = false;
            }
        }

        // Creates a new log entry using the provided string.
        private void AddLogEntry(string str)
        {
            var newLogEntry = Instantiate(LogEntryPrefab, Vector3.zero, Quaternion.identity);
            newLogEntry.text = str;
            newLogEntry.fontSize = LogEntryFontSize;
            newLogEntry.gameObject.SetActive(true);

            var transform = newLogEntry.transform;
            transform.SetParent(LogHistory.transform);
            transform.localScale = Vector3.one;

            _logEntries.Add(newLogEntry);

            if (MaxLogCount > 0 && _logEntries.Count > MaxLogCount)
            {
                var textObj = _logEntries.First();
                _logEntries.RemoveAt(0);
                Destroy(textObj.gameObject);
            }
        }

        public static void LogInfo(string message)
        {
            string line = $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
            Instance.AddLogEntry(line);
#if UNITY_EDITOR
            Debug.Log(line);
#endif
        }

        public static void LogWarning(string message)
        {
            string line = $"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
            Instance.AddLogEntry(line);
#if UNITY_EDITOR
            Debug.LogWarning(line);
#endif
        }

        public static void LogError(string message)
        {
            string line = $"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
            Instance.AddLogEntry(line);
#if UNITY_EDITOR
            Debug.LogError(line);
#endif
        }

        public static void Clear()
        {
            foreach (var entry in Instance._logEntries)
                Destroy(entry.gameObject);

            Instance._logEntries.Clear();
        }
    }

}