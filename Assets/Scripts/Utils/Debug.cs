﻿#if (!UNITY_EDITOR && !ENABLE_LOG)
using UnityEngine;

public class Debug
{
    public static bool developerConsoleVisible
    {
        get => UnityEngine.Debug.developerConsoleVisible;
        set => UnityEngine.Debug.developerConsoleVisible = value;
    }

    public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Break()
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void ClearDeveloperConsole()
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DebugBreak()
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message, Object context)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogError(object message, Object context)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogException(System.Exception exception)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogException(System.Exception exception, Object context)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message)
    {
    }

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void LogWarning(object message, Object context)
    {
    }
}

#endif