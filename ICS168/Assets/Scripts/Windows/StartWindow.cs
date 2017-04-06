using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWindow : GenericWindow {

    private WindowIDs _toClose = WindowIDs.StartWindow;

    // ***Still need to add a Play function that opens another scene for the game

    // Closes StartWindow, and opens ControlsWindow
    public void Controls() {
        ToggleWindows(_toClose, WindowIDs.Controls);
    }

    // Closes StartWindow, and opens CreditsWindow
    public void Credits() {
        ToggleWindows(_toClose, WindowIDs.Credits);
    }

    // Exits the game application
    public void Quit() {
        Application.Quit();
    }
}
