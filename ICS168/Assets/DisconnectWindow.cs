using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectWindow : GenericWindow {

	public void ReturnToMain() {
        ToggleWindows(WindowIDs.None, WindowIDs.OnlineStatus);
        ToggleWindows(WindowIDs.DisconnectWindow, WindowIDs.StartWindow);
    }
}
