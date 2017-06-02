using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullLobbyWindow : GenericWindow {

    public void BackToGameSelect()
    {
        ToggleWindows(WindowIDs.FullLobby, WindowIDs.GameSelect);
    }
}