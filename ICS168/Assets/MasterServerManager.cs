﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;

public enum MasterServerCommands {
    C_VerifyLogin = 0,
    C_CreateAccount = 1,
    RegisterGameServer = 2,
    RegisterClient = 3,
    VerifyOccupancy = 6,
    C_SelectGameInstance = 7
}

public class GameInstanceStats {

    public GameInstanceStats(string name) {
        serverName = name;
    }

    public string serverName = "";
    public int serverID = 0;
    public int inGamePlayers = 0;
}

/*** CLIENT VARIABLES ***/
// Encapsulated client info
public class ClientInfo {
    public ClientInfo() { }
    public ClientInfo(int socket, int connection, int channel) {
        socketID = socket;
        ConnectionID = connection;
        ChannelID = channel;
    }

    public int gameServerID = -1;
    public int socketID = -1;
    public int ConnectionID = -1;
    public int ChannelID = -1;
    public string username = "";
    public int playerNum = -1;
}

public class MasterServerManager : Singleton<MasterServerManager> {

    [Header("Game Path")]
    [SerializeField] private string _GameInstancePath = "";

    [Header("Server Settings")]
    [SerializeField] private int _incomingBufferSize = 1024;    // max buffer size
    [SerializeField] private int _socketPort = 8888;
    [SerializeField] private int _maxGameInstances = 0;
    [SerializeField] private string _apachePortNumber = "80";

    private int UDP_ChannelIDSeq = -1;
    private int UDP_ChannelIDFrag = -1;
    private int UDP_ChannelID = -1;
    private int TCP_ChannelID = -1;
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
    private Dictionary<int, ClientInfo> _clients = new Dictionary<int, ClientInfo>();

    // Keep track of usernames used.
    private List<string> _activeusernames = new List<string>();

    // Use this for initialization
    void Start () {

        Application.runInBackground = true;
        NetworkTransport.Init();

        if (_apachePortNumber != "80" || _apachePortNumber != "")
        {
            _LoginURL = "http://localhost:" + _apachePortNumber + "/teamnewport/LoginManager.php";
            _CreateAccountURL = "http://localhost:" + _apachePortNumber + "/teamnewport/CreateAccount.php";
        }

        ConnectionConfig config = new ConnectionConfig();
        TCP_ChannelID = config.AddChannel(QosType.Reliable);
        UDP_ChannelIDSeq = config.AddChannel(QosType.UnreliableSequenced);
        UDP_ChannelIDFrag = config.AddChannel(QosType.UnreliableFragmented);
        UDP_ChannelID = config.AddChannel(QosType.Unreliable);

        _maxConnections = _maxGameInstances * 4;
        HostTopology hostTopology = new HostTopology(config, _maxConnections);
        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);

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

                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
                int prefix = 0;
                int index;
                string newMessage = "";

                // New parser now allows client commands to be > 1 digit
                for (index = 0; index < message.Length; ++index) {
                    if (message[index] == '{')
                        break;
                }

                prefix = Convert.ToInt32(message.Substring(0, index));
                newMessage = message.Substring(index);

                // Have game servers register themselves.
                if (prefix == (int)MasterServerCommands.RegisterGameServer) {

                    Debug.Log("register a server");
                    
                    foreach (KeyValuePair<string, GameInstanceStats> instance in _gameInstances) {
                        if (instance.Value.serverID == 0) {
                            instance.Value.serverID = JsonUtility.FromJson<PortID>(newMessage).portID;
                            break;
                        }
                    }
                }
                
                else if (prefix == (int)MasterServerCommands.RegisterClient) {

                    if (!_clients.ContainsKey(incomingConnectionID)) {
                        ClientInfo clientInfo = new ClientInfo(incomingSocketID, incomingConnectionID, incomingChannelID);
                        _clients.Add(incomingConnectionID, clientInfo);
                        Debug.Log(incomingConnectionID + " added to _clients");
                    }
                }

                //process login info
                else if (prefix == (int)MasterServerCommands.C_VerifyLogin) {
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

                    string serverName = JsonUtility.FromJson<LoginInfo>(newMessage).username;

                    if (_gameInstances.ContainsKey(serverName.ToLower())) {
                        // Connect client to an already existing game server.

                        Debug.Log("Forwarding player to already established game: " + serverName);
                        if (_gameInstances[serverName.ToLower()].serverID != 0) {
                            ForwardPlayerToGame(serverName.ToLower(), _clients[incomingConnectionID]);
                        }
                    }
                    else {
                        // Create an instace of a game and have the client connect.
                        _gameInstances.Add(serverName.ToLower(), new GameInstanceStats(serverName.ToLower()));
                        //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        //startInfo.FileName = _GameInstancePath;
                        //System.Diagnostics.Process.Start(startInfo);

                        StartCoroutine(ForwardPlayerToGameWithDelay(serverName.ToLower(), _clients[incomingConnectionID],
                            incomingSocketID, incomingConnectionID));

                    }
                }
                else if(prefix == (int)MasterServerCommands.VerifyOccupancy) {
                    int inGamePlayers = JsonUtility.FromJson<GameServerInfo>(newMessage).inGamePlayers;
                    string serverName = JsonUtility.FromJson<GameServerInfo>(newMessage).serverName;
                    _gameInstances[serverName].inGamePlayers = inGamePlayers;
                }
               
                break;

            case NetworkEventType.DisconnectEvent:
                _numberOfConnections = --_numberOfConnections < 0 ? 0 : _numberOfConnections;
                _activeusernames.Remove(_clients[incomingConnectionID].username);
                break;
        }
    }

    private IEnumerator verifyLogin(string username, string password, int socketID, int connectionID, int channelID) {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(_LoginURL, form);
        yield return verify;

        if (verify.text == "valid" && !_activeusernames.Contains(username)) {
            // Set _clients' username
            _clients[connectionID].username = username;

            // ALL THIS SHOULD HAPPEN WHEN THE CLIENT CONNECTS TO THE ACTUAL GAME SERVER
            //WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.Lobby);

            byte error;
            string jsonToBeSent = "2";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

            _activeusernames.Add(username);
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
        else if (verify.text == "valid" && _activeusernames.Contains(username)) {
            byte error;
            string jsonToBeSent = "15";
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

    private IEnumerator ForwardPlayerToGameWithDelay(string serverName, ClientInfo client, int GS_SocketID, int GS_ConnectionID) {

        while (_gameInstances[serverName].serverID == 0) {
            yield return 0;
        }

        // When the instance has finally been created and setup, forward the player to the new game server.
        byte error = 0;
        string jsonToBeSent = "14";
        jsonToBeSent += JsonUtility.ToJson(new PortID( _gameInstances[serverName].serverID));
        byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
        NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, messageBuffer, messageBuffer.Length, out error);

        // Inform the server of its name.
        jsonToBeSent = "7";
        jsonToBeSent += JsonUtility.ToJson(serverName);
        messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
        NetworkTransport.Send(GS_SocketID, GS_ConnectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
    }

    private void ForwardPlayerToGame(string serverName, ClientInfo client) {

        // When the instance has finally been created and setup, forward the player to the new game server.
        byte error = 0;
        string jsonToBeSent = "14";
        jsonToBeSent += JsonUtility.ToJson(new PortID(_gameInstances[serverName].serverID));
        byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
        NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, messageBuffer, messageBuffer.Length, out error);
    }
}
