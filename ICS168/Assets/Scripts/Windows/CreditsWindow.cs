using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsWindow : GenericWindow {

    public void BackToMain() {
        ToggleWindows(WindowIDs.Credits, WindowIDs.StartWindow);
    }
}
