using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsWindow : GenericWindow {

    public static event GenericWindowEvent OnBackToMain;

    public void BackToMain() {
        ToggleWindows(WindowIDs.Controls, WindowIDs.StartWindow);
    }
}
