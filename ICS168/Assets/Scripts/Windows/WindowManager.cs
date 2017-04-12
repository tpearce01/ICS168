using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script handles toggling on and off all our available windows */

public class WindowManager : Singleton<WindowManager> {

    [SerializeField]
    private WindowIDs _defaultWindow;   // The initial default window

    public WindowIDs currentWindow;     // The window that is currently displayed

    [SerializeField]
    private GenericWindow[] _windows;   // An array of all windows derived from GenericWindow
    
    private void OnEnable() {
        GenericWindow.OnToggleWindows += ToggleWindows;

		GameManager.OnEndGame += ToggleWindows;
    }


    private void OnDisable() {
        GenericWindow.OnToggleWindows -= ToggleWindows;
		GameManager.OnEndGame -= ToggleWindows;
    }

    // This function handles displaying and un-displaying the windows 
    public void ToggleWindows(WindowIDs close, WindowIDs open) {

        // If the "close" window being passed in isn't the empty window (-1 in enum), un-display it
        if (close != WindowIDs.None) { _windows[(int)close].GetComponent<GenericWindow>().Close(); }

        // Set the current window to the "open" window that was passed in
        currentWindow = open;

        // If the "open" window that was passed in isn't the empty window, display it
        if (currentWindow != WindowIDs.None) { _windows[(int)currentWindow].GetComponent<GenericWindow>().Open(); }
    }
}
