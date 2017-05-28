using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectWindow : GenericWindow {

    public void OnBackToMain() {
        WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.StartWindow);
    }

    public void OnJoinGame() {
        Text input = gameObject.GetComponentInChildren<Text>();
        string serverName = input.text;

        ClientConnection.Instance.ConnectToGameServer(serverName);
    }
}
