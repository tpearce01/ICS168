using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLobbyWindow : GenericWindow
{
    public void BackToMain()
    {
        ToggleWindows(WindowIDs.ServerLobbyWindow, WindowIDs.StartWindow);
    }
}
