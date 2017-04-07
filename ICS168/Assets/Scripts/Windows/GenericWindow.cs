using System.Collections;
using UnityEngine;

/* This script is a base template for all windows */

// An enum for all windows in the game
public enum WindowIDs {
    StartWindow = 0,
    Controls = 1,
    Credits = 2,
    Login = 3,
    NewAccount = 4,
    Victory = 5,
    None = -1
}

// This class is a base template for all windows, all windows are derived from this class
public class GenericWindow : MonoBehaviour {

    public delegate void GenericWindowEvent(WindowIDs close, WindowIDs open);
    public static event GenericWindowEvent OnToggleWindows;

    // Changes which game object (window) is active in the hierarchy
    protected virtual void Display(bool value) {
        gameObject.SetActive(value);
    }

    // If a window is set to open, this function will display it
    public virtual void Open() {
        Display(true);
    }

    // If a window is set to close, this function will un-display(?) it
    public virtual void Close() {
        Display(false);
    }

    // A method that allows all windows derived from GenericWindow to have access to the ToggleWindows method in the WindowManager
    protected virtual void ToggleWindows(WindowIDs close, WindowIDs open) {
        if (OnToggleWindows != null) { OnToggleWindows(close, open); }
    }
}
