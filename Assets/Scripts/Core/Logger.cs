using TMPro;
using UnityEngine;
using System;


/// <summary>
/// Write formatted debug data in the Unity console and the Application console
/// </summary>
public class Logger : Singleton<Logger>
{
    [SerializeField]
    private TextMeshProUGUI debugAreaText = null;

    [SerializeField]
    private bool enableDebug = false;

    [SerializeField]
    private int maxLines = 15;

    public void Awake()
    {
        if (debugAreaText == null)
        {
            debugAreaText = GetComponent<TextMeshProUGUI>();
        }

        debugAreaText.text = string.Empty;
    }

    public void OnEnable()
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;

        if (enabled)
        {
            debugAreaText.text += $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled.</color>\n";
        }
    }

    public static void LogInfo(string message)
    {
        ClearLines();
        string line = $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        Instance.debugAreaText.text += line;
#if UNITY_EDITOR
        Debug.Log(line);
#endif
    }

    public static void LogWarning(string message)
    {
        ClearLines();
        string line = $"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        Instance.debugAreaText.text += line;
#if UNITY_EDITOR
        Debug.LogWarning(line);
#endif
    }

    public static void LogError(string message)
    {
        ClearLines();
        string line = $"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        Instance.debugAreaText.text += line;
#if UNITY_EDITOR
        Debug.LogError(line);
#endif
    }

    public static void ClearLines()
    {
        if (Instance.debugAreaText.text.Split('\n').Length >= Instance.maxLines)
        {
            Instance.debugAreaText.text = string.Empty;
        }
    }
}