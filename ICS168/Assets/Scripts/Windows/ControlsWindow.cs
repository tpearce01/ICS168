using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsWindow : GenericWindow {

    // Closes ControlsWindow, and opens StartWindow
    public void BackToMain() {
        ToggleWindows(WindowIDs.Controls, WindowIDs.StartWindow);
    }
}
