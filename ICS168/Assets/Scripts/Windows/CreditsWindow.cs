using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsWindow : GenericWindow {

    // Closes CreditsWindow, and opens StartWindow
    public void BackToMain() {
        ToggleWindows(WindowIDs.Credits, WindowIDs.StartWindow);
    }
}
