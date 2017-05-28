﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;

public enum MasterServerCommands {
    C_VerifyLogin = 0,
    C_CreateAccount = 1,
    VerifyOccupancy = 6,
    C_SelectGameInstance = 7,
    S_GameInstanceInfo = 8
}

public class GameInstanceStats {

    public GameInstanceStats(string name) {
        serverName = name;
    }

    public string serverName = "";
    public int serverID = 0;
    public int inGamePlayers = 0;
}

public class MasterServerManager : Singleton<MasterServerManager> {

    [Header("Server Settings")]
    [SerializeField] private int _incomingBufferSize = 1024;    // max buffer size
    [SerializeField] private int _socketPort = 8888;
    [SerializeField] private int _maxGameInstances = 0;

    private int UDP_ChannelIDFrag = -1;                         // This channel should be reserved for larger messages
    private int _socketID = -1;
    private int _connectionID = -1;
    private int _maxConnections = 0;

    [Space]
    [Header("Server Stats")]
    [SerializeField] private int _numberOfConnections = 0;
    [SerializeField] private int _numberOfGameInstances = 0;

    /*** PHP VARIABLES ***/
    private string _LoginURL = "http://localhost/teamnewport/LoginManager.php";
    private string _CreateAccountURL = "http://localhost/teamnewport/CreateAccount.php";

    // Keep track of available games.
    private Dictionary<string, GameInstanceStats> _gameInstances = new Dictionary<string, GameInstanceStats>();

    // Maps connectionID with ClientInfo
    private Dictionary<int, ClientInfo> _clientSocketIDs = new Dictionary<int, ClientInfo>();

    // Use this for initialization
    void Start () {
        NetworkTransport.Init();

        ConnectionConfig connectionConfig = new ConnectionConfig();
        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);

        _maxConnections = _maxGameInstances * 4;
        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);

        Application.runInBackground = true;
    }
	
	// Update is called once per frame
	void Update () {

        // Incoming Connection IDs
        int incomingSocketID = -1;
        int incomingConnectionID = -1;
        int incomingChannelID = -1;
        byte[] incomingMessageBuffer = new byte[_incomingBufferSize];
        int dataSize = 0;
        byte error;

        NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
            out incomingChannelID, incomingMessageBuffer, _incomingBufferSize, out dataSize, out error);

        switch (incomingNetworkEvent) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("Master Server: new client connected");
                ++_numberOfConnections;
                break;

            case NetworkEventType.DataEvent:

                //Test Code
                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
                //End Test Code

                int prefix = Convert.ToInt32(message.Substring(0, 1));
                string newMessage = message.Substring(1);

                //process login info
                if (prefix == (int)MasterServerCommands.C_VerifyLogin) {
                    LoginInfo info = JsonUtility.FromJson<LoginInfo>(newMessage);
                    StartCoroutine(verifyLogin(info.username, info.password, incomingSocketID, incomingConnectionID, incomingChannelID));
                }

                //process create account info
                else if (prefix == (int)MasterServerCommands.C_CreateAccount) {
                    LoginInfo info = JsonUtility.FromJson<LoginInfo>(newMessage);
                    StartCoroutine(CreateUser(info.username, info.password, incomingSocketID, incomingConnectionID, incomingChannelID));
                }
                
                // Process game connection requests
                else if (prefix == (int)MasterServerCommands.C_SelectGameInstance) {

                    string serverName = JsonUtility.FromJson<string>(newMessage);

                    if (_gameInstances.ContainsKey(serverName.ToLower())) {
                        // Connect client to the server.
                    }
                    else {
                        // Create an instace of a game and have the client connect.
                        _gameInstances.Add(serverName.ToLower(), new GameInstanceStats(serverName.ToLower()));
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.FileName = "C:/Users/danie/Documents/GitKraken/ICS168/ICS168/Builds/BombermanServer.exe";
                        System.Diagnostics.Process.Start(startInfo);
                    }
                }

                break;

            case NetworkEventType.DisconnectEvent:
                _numberOfConnections = --_numberOfConnections < 0 ? 0 : _numberOfConnections;
                break;
        }
    }

    private IEnumerator verifyLogin(string username, string password, int socketID, int connectionID, int channelID) {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(_LoginURL, form);
        yield return verify;

        if (verify.text == "valid") {

            // ALL THIS SHOULD HAPPEN WHEN THE CLIENT CONNECTS TO THE ACTUAL GAME SERVER
            //WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.Lobby);

            byte error;
            string jsonToBeSent = "2";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

            //_clientSocketIDs[connectionID].username = username;
            //_clientSocketIDs[connectionID].playerNum = connectionID - 1; // decremented so the range starts with 0 and not 1

            //Debug.Log(username + " logged in with connection id: " + connectionID);
            //Debug.Log(username + " playerNum is " + _clientSocketIDs[connectionID].playerNum);

            //// IF the lobby is not loaded, load it.
            //if (WindowManager.Instance.currentWindow == WindowIDs.None) {
            //    WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.Lobby);
            //}
        }
        else if (verify.text == "invalid") {

            byte error;
            string jsonToBeSent = "8";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

        }
        else if (verify.text == "user not found") {

            byte error;
            string jsonToBeSent = "9";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
        }
    }

    private IEnumerator CreateUser(string username, string password, int socketID, int connectionID, int channelID) {
        Debug.Log(username + " created an account with password " + password);

        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(_CreateAccountURL, form);

        yield return verify; ;

        if (verify.text == "username exists") {
            byte error;
            string jsonToBeSent = "7";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

        }
        else if (verify.text == "account created") {
            Debug.Log("account was created");
            byte error;
            string jsonToBeSent = "6";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
        }
    }
}