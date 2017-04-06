using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager> {

    [SerializeField]
    private WindowIDs _defaultWindow;

    public WindowIDs currentWindow;

    [SerializeField]
    private GenericWindow[] _windows;

    private void OnEnable() {
        GenericWindow.OnToggleWindows += ToggleWindows;
    }

    private void OnDisable() {
        GenericWindow.OnToggleWindows -= ToggleWindows;
    }

    public void ToggleWindows(WindowIDs close, WindowIDs open) {

        if (close != WindowIDs.None) { _windows[(int)close].GetComponent<GenericWindow>().Close(); }
        currentWindow = open;

        if (currentWindow != WindowIDs.None) { _windows[(int)currentWindow].GetComponent<GenericWindow>().Open(); }
    }
}
