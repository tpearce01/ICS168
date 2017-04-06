using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWindow : GenericWindow {

    private WindowIDs _toClose = WindowIDs.StartWindow;

    public void Controls() {
        ToggleWindows(_toClose, WindowIDs.Controls);
    }

    public void Credits() {
        ToggleWindows(_toClose, WindowIDs.Credits);
    }

    public void Quit() {
        Application.Quit();
    }
}
