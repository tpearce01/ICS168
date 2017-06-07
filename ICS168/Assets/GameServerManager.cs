using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.SceneManagement;

public enum GameServerCommands {
    PlayerInput = 2,
    SetUsername = 3,
    LeaveLobby = 4,
    LeaveGame = 5,
    AssignName = 7
}

public class ServerObject {
    public ServerObject(int currentFrame, string tex) {
        frameNum = currentFrame;
        texture = tex;
    }

    public int frameNum;
    public string texture;
}

public class PortID {
    public PortID(int ID) {
        portID = ID;
    }
    public int portID = 0;
}

public class GameServerInfo {

    public GameServerInfo(int numOfPlayers, string name) {
        inGamePlayers = numOfPlayers;
        serverName = name;
    }

    public int inGamePlayers = 0;
    public string serverName = "";
}

public class GameServerManager : Singleton<GameServerManager> {
    /*** RENDER VARIABLES ***/
    [SerializeField] private RenderTexture rt;  //Target render texture
    [SerializeField] private Camera _cam;       //Camera to render from

    /*** Apache Variables ***/
    /// <summary>
    /// The port of the authentication server, defaults to 80
    /// </summary>
    public string apachePort;

    // Maps connectionID with ClientInfo
    private Dictionary<int, ClientInfo> _clients = new Dictionary<int, ClientInfo>();

    /*** SERVER VARIABLES ***/
    [SerializeField] private int _incomingBufferSize = 3000;    // max buffer size
    [SerializeField] private int _maxConnections = 0;           // max number of connections


    public int MaxConnections {
        get { return _maxConnections; }
    }

    // MASTER SERVER CONNECTION INFO
    private int UDP_ChannelIDSeq = -1;
    private int UDP_ChannelIDFrag = -1;
    private int UDP_ChannelID = -1;
    private int TCP_ChannelID = -1;

    private int _socketID = -1;
    //private int GS_connectionID = -1;
    [SerializeField] private int GS_Port = 8889;
    public int PortNumber {
        get { return GS_Port; }
    }

    /*** PLAYER VARIABLES ***/
    [SerializeField] private int _inGamePlayers = 0;            // Players who have successfully logged in
    public int InGamePlayers {
        get { return _inGamePlayers; }
        set { _inGamePlayers = value; }
    }

    [SerializeField] private LobbyWindow _lobby;
    [SerializeField] private NotificationArea _notifArea;
    private void OnEnable() {
        _cam = Camera.main;
    }


    private int MS_ConnectionID = -1;

    private string _serverName = "";

    // Initialization
    void Start() {
        Application.runInBackground = true;

        NetworkTransport.Init();

        // Setup Master Server Connection Channel
        ConnectionConfig config = new ConnectionConfig();

        TCP_ChannelID = config.AddChannel(QosType.Reliable);
        UDP_ChannelIDSeq = config.AddChannel(QosType.UnreliableSequenced);
        UDP_ChannelIDFrag = config.AddChannel(QosType.UnreliableFragmented);
        UDP_ChannelID = config.AddChannel(QosType.Unreliable);

        HostTopology hostTopology = new HostTopology(config, _maxConnections);

        // Setup Client Connection Channel

        // Look for an available socket port for this instance to use. 8888 is used by the master.
        int socketPort = GS_Port;
        int portDelta = 0;
        while (portDelta < 10) {
            try {
                _socketID = NetworkTransport.AddHost(hostTopology, socketPort + portDelta);
                if (_socketID < 0)
                    throw new Exception();
                else
                    break;
            }
            catch (Exception) {
                portDelta++;
            }
        }

        GS_Port = socketPort + portDelta;

        // Give the middle man the port number so it can tell the Master server.
        byte error;
        MS_ConnectionID = NetworkTransport.Connect(_socketID, "127.0.0.1", 8888, 0, out error);
        //ServerMiddleMan.Instance.StartMiddleMan(GS_Port);
    }

    void Update() {
        CaptureFrame();

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
                Debug.Log("GameServer: ConnectEvent");
                // Master Server Connection
                if (incomingConnectionID == 1) {

                    // Tell the Master Server to Inform the Client of the connection info
                    string jsonToBeSent = "2";

                    jsonToBeSent += JsonUtility.ToJson(new PortID(GS_Port));
                    byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
                    NetworkTransport.Send(_socketID, MS_ConnectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
                }
                // Client Connections
                else if (incomingConnectionID > 1) {

                    ClientInfo clientInfo = new ClientInfo();
                    clientInfo.socketID = incomingSocketID;
                    clientInfo.ConnectionID = incomingConnectionID;
                    clientInfo.ChannelID = incomingChannelID;
                    _clients.Add(incomingConnectionID, clientInfo);

                    _inGamePlayers = _clients.Count;

                    // Need to inform master server of current connections every time a new client connects
                    string jsonToBeSend = "6";
                    jsonToBeSend += JsonUtility.ToJson(new GameServerInfo(_inGamePlayers, _serverName));
                    SendJSONMessageToMaster(jsonToBeSend);
                }

                break;

            case NetworkEventType.DataEvent:
                Debug.Log("Game Server: Data Event");

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

                Debug.Log("Prefix: " + prefix);

                //process user game input
                if (prefix == (int)GameServerCommands.PlayerInput) {
                    PlayerIO input = JsonUtility.FromJson<PlayerIO>(newMessage);
                    //Debug.Log(incomingConnectionID + " is moving " + input.button);
                    Debug.Log("ConnectionID: " + incomingConnectionID);
                    GameManager.Instance.PlayerActions(incomingConnectionID, input);
                }

                else if (prefix == (int)GameServerCommands.SetUsername) {
                    _clients[incomingConnectionID].username = JsonUtility.FromJson<LoginInfo>(newMessage).username;
                    _clients[incomingConnectionID].playerNum = incomingConnectionID - 2; // decremented so the range starts with 0 and not 1

                    _notifArea.playerEntered(JsonUtility.FromJson<LoginInfo>(newMessage).username);

                    string jsonToBeSent = "0";
                    SendJSONMessage(jsonToBeSent, _clients[incomingConnectionID], QosType.Reliable);

                    // IF the lobby is not loaded, load it.
                    if (WindowManager.Instance.currentWindow == WindowIDs.None) {
                        WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.Lobby);
                    }

                    if (_lobby.gameObject.activeInHierarchy == true) {
                        _lobby.AddPlayerToLobby(_clients[incomingConnectionID].username, _clients[incomingConnectionID].playerNum);
                    }
                }
                else if (prefix == (int)GameServerCommands.LeaveLobby) {

                    Debug.Log("Leave Lobby Command");

                    _inGamePlayers = --_inGamePlayers < 0 ? 0 : _inGamePlayers;

                    if (_lobby.gameObject.activeInHierarchy == true) {
                        _lobby.RemovePlayerFromLobby(_clients[incomingConnectionID].playerNum);
                    }
                }
                else if (prefix == (int)GameServerCommands.LeaveGame) {

                    _inGamePlayers--;
                    if (_inGamePlayers < 0) { _inGamePlayers = 0; }

                    if (_inGamePlayers < 1) {
                        GameManager.Instance.ResetGameManager();
                        SceneManager.LoadScene("Server Game Version");
                    }
                }
                else if (prefix == (int)GameServerCommands.AssignName) {
                    _serverName = JsonUtility.FromJson<string>(newMessage);
                }

                break;

            case NetworkEventType.DisconnectEvent:

                //if (incomingConnectionID > 1) {
                    _inGamePlayers = --_inGamePlayers < 0 ? 0 : _inGamePlayers;

                    if (_lobby.gameObject.activeInHierarchy == true) {
                        _lobby.RemovePlayerFromLobby(_clients[incomingConnectionID].playerNum);
                    }

                    Debug.Log("Removed " + _clients[incomingConnectionID].username + " from the game.");
                    GameManager.Instance.LeaveGame(incomingConnectionID);
                    _clients.Remove(incomingConnectionID);

                    // Need to inform master server of current connections
                    string jsonToBeSent1 = "6";
                    jsonToBeSent1 += JsonUtility.ToJson(new GameServerInfo(_inGamePlayers, _serverName));
                    SendJSONMessageToMaster(jsonToBeSent1);

                    if (_inGamePlayers < 1) {
                        GameManager.Instance.ResetGameManager();
                        WindowManager.Instance.ToggleWindows(WindowIDs.PlayerInfo, WindowIDs.None);
                        SceneManager.LoadScene("Server Game Version");
                    }
               // }
                break;
        }
    }

    public void SendJSONMessageToMaster(string JSONObject) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONObject);
        NetworkTransport.Send(_socketID, MS_ConnectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
    }

    public void SendJSONMessage(string JSONobject, ClientInfo client, QosType type) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);

        if (type == QosType.Reliable) {
            NetworkTransport.Send(client.socketID, client.ConnectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.Unreliable) {
            NetworkTransport.Send(client.socketID, client.ConnectionID, UDP_ChannelID, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.UnreliableFragmented) {
            NetworkTransport.Send(client.socketID, client.ConnectionID, UDP_ChannelIDFrag, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.UnreliableSequenced) {
            NetworkTransport.Send(client.socketID, client.ConnectionID, UDP_ChannelIDSeq, messageBuffer, messageBuffer.Length, out error);
        }
    }

    public void SendJSONMessageToAll(string JSONobject, QosType type) {

        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        //Debug.Log("Sending message of length " + messageBuffer.Length);
        foreach (KeyValuePair<int, ClientInfo> client in _clients) {

            if (type == QosType.Reliable) {
                NetworkTransport.Send(client.Value.socketID, client.Value.ConnectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
            }
            else if (type == QosType.Unreliable) {
                NetworkTransport.Send(client.Value.socketID, client.Value.ConnectionID, UDP_ChannelID, messageBuffer, messageBuffer.Length, out error);
            }
            else if (type == QosType.UnreliableFragmented) {
                NetworkTransport.Send(client.Value.socketID, client.Value.ConnectionID, UDP_ChannelIDFrag, messageBuffer, messageBuffer.Length, out error);
            }
            else if (type == QosType.UnreliableSequenced) {
                NetworkTransport.Send(client.Value.socketID, client.Value.ConnectionID, UDP_ChannelIDSeq, messageBuffer, messageBuffer.Length, out error);
            }
        }
    }

    /// <summary>
    /// Sends the JSON message to one specific client.
    /// </summary>
    /// <param name="JSONobject"></param>
    /// <param name="socketID"></param>
    /// <param name="connectionID"></param>
    /// <param name="channelID"></param>
    public void SendOneJSONMessage(string JSONobject, int socketID, int connectionID, int channelID) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
    }

    void CaptureFrame() {
        RenderTexture.active = rt;
        Camera.main.Render();

        Texture2D tex = new Texture2D(_cam.targetTexture.width, _cam.targetTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, _cam.targetTexture.width, _cam.targetTexture.height), 0, 0);
        tex.Apply();
        byte[] image = tex.EncodeToPNG();

        // Create a new Server object and populate its attributes
        ServerObject toBeSent = new ServerObject(Time.frameCount, Convert.ToBase64String(image));

        // Convert to JSON
        string jsonToBeSent = "1";
        jsonToBeSent += JsonUtility.ToJson(toBeSent);

        // Once we have at least 1 successfully logged in player, we should begin to transmit the lobby/game.
        if (_inGamePlayers > 0) {
            SendJSONMessageToAll(jsonToBeSent, QosType.UnreliableFragmented);
        }
    }

    public void EnableClientControls() {
        string toBeSent = "4";
        SendJSONMessageToAll(toBeSent, QosType.Reliable);
    }

    public void PreventDisconnects() {
        string toBeSent = "3";
        SendJSONMessageToAll(toBeSent, QosType.Reliable);
    }
}

