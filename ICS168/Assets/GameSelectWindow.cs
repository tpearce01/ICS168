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
        GameObject.Find("MaxNumInstance").GetComponent<Text>().text = "";
        GameObject.Find("AvailableInstance").GetComponent<Text>().text = "";
        GameObject.Find("PleaseWait").GetComponent<Text>().text = "";
    }

    public void OnBackToMain() {
        byte error;
        NetworkTransport.Disconnect(ClientConnection.Instance.MSSocketID, ClientConnection.Instance.MSConnectionID, out error);
        WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.StartWindow);
    }

    //If network drops the message, client can't connect.
    public void OnJoinGame() {
        // Connect the player to game server and disable the join button
        Text[] input = gameObject.GetComponentsInChildren<Text>();
        string serverName = input[3].text;

        Debug.Log("Trying to connect to: " + serverName);
        ClientConnection.Instance.ConnectToGameServer(serverName);

        _joinButton.interactable = false;
        //if(ClientConnection.Instance.CanConnectToGame == true) {
        //    _joinButton.interactable = false;
        //}else {
        //    _joinButton.interactable = true;
        //}
        
    }
}
