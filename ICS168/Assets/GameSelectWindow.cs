using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectWindow : GenericWindow {
    [SerializeField]
    Button JoinButton;

    public void OnBackToMain() {
        WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.StartWindow);
    }

    //If network drops the message, client is fucked and can never play the game ever again
    public void OnJoinGame() {
        Text[] input = gameObject.GetComponentsInChildren<Text>();
        string serverName = input[3].text;

        Debug.Log("Trying to connect to: " + serverName);
        ClientConnection.Instance.ConnectToGameServer(serverName);
        
        JoinButton.interactable = false;
    }
}
