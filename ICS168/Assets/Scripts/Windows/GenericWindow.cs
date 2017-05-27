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
    Game = 5,
    Victory = 6,
    NewAccountSuccess = 7,
    Lobby = 8,
    OnlineStatus = 9,
    ClientLobby = 10,
    DisconnectWindow = 11,
    NotificationArea = 12,
    PlayerInfo = 13,
    None = -1
}

// This class is a base template for all windows, all windows are derived from this class
public abstract class GenericWindow : MonoBehaviour {

    public delegate void GenericWindowEvent(WindowIDs close, WindowIDs open);
    public static event GenericWindowEvent OnToggleWindows;

    /// <summary>
    /// Changes which game object (window) is active in the hierarchy
    /// </summary>
    /// <param name="value"></param>
    protected virtual void Display(bool value) {
        gameObject.SetActive(value);
    }

    /// <summary>
    /// If a window is set to open, this function will display it
    /// </summary>
    public virtual void Open() {
        Display(true);
    }

    /// <summary>
    /// If a window is set to close, this function will un-display(?) it
    /// </summary>
    public virtual void Close() {
        Display(false);
    }

    /// <summary>
    /// A method that allows all windows derived from GenericWindow to have access to the ToggleWindows method in the WindowManager
    /// </summary>
    /// <param name="close"></param>
    /// <param name="open"></param>
    protected virtual void ToggleWindows(WindowIDs close, WindowIDs open) {
        if (OnToggleWindows != null) { OnToggleWindows(close, open); }
    }
}
