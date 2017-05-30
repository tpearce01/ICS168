using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectWindow : GenericWindow {

    public void OnBackToMain() {
        WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.StartWindow);
    }

    public void OnJoinGame() {
        Text[] input = gameObject.GetComponentsInChildren<Text>();
        string serverName = input[3].text;

        Debug.Log("Trying to connect to: " + serverName);
        ClientConnection.Instance.ConnectToGameServer(serverName);
    }
}
