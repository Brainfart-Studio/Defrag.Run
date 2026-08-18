using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class OrientationDetector : MonoBehaviour
{
    private static OrientationDetector instance;
    public static OrientationDetector Instance => instance;

    public static event Action<bool> OnOrientationChanged;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int IsPortrait();

    [DllImport("__Internal")]
    private static extern void RegisterOrientationListener(string gameObjectName);
#endif

    private void Awake()
    {
        instance = this;

#if UNITY_WEBGL && !UNITY_EDITOR
        RegisterOrientationListener(gameObject.name);
#endif
    }

    public bool IsCurrentlyPortrait()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return IsPortrait() == 1;
#else
        return Screen.height > Screen.width;
#endif
    }

    // Invoked by the browser via SendMessage when the JS media query fires
    private void HandleOrientationChanged(string isPortrait)
    {
        OnOrientationChanged?.Invoke(isPortrait == "1");
    }
}