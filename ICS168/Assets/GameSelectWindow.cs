using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameSelectWindow : GenericWindow {
    [SerializeField]
    private Button _joinButton;

    public void OnEnable() {
        _joinButton.interactable = true;
    }

    public void OnBackToMain() {
        byte error;
        NetworkTransport.Disconnect(ClientConnection.Instance.MSSocketID, ClientConnection.Instance.MSConnectionID, out error);
        WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.StartWindow);
    }

    //If network drops the message, client can't connect.
    public void OnJoinGame() {
        Text[] input = gameObject.GetComponentsInChildren<Text>();
        string serverName = input[3].text;

        Debug.Log("Trying to connect to: " + serverName);
        ClientConnection.Instance.ConnectToGameServer(serverName);

        _joinButton.interactable = false;
    }
}
