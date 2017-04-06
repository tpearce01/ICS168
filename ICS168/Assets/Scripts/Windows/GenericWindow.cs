using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script is a base template for all windows */

// An enum for all windows in the game
public enum WindowIDs {
    StartWindow = 0,
    Controls = 1,
    Credits = 2,
    None = -1
}

// This class is a base template for all windows, all windows are derived from this class
public class GenericWindow : MonoBehaviour {

    public delegate void GenericWindowEvent(WindowIDs close, WindowIDs open);
    public static event GenericWindowEvent OnToggleWindows;

    private Button[] _buttons;  // used to store the buttons in a window

    // Changes which game object (window) is active in the hierarchy
    protected virtual void Display(bool value) {
        gameObject.SetActive(value);
    }

    // If a window is set to open, this function will display it
    public virtual void Open() {
        Display(true);

        // This block of code does the resizing of the buttons when selected and de-selected
        _buttons = GetComponentsInChildren<Button>();
        float defaultWidth = _buttons[0].GetComponent<LayoutElement>().minWidth;

        for (int i = 0; i < _buttons.Length; ++i) {
            _buttons[i].GetComponent<LayoutElement>().preferredWidth = defaultWidth;
        }
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
