using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public enum ClientCommands {
    StartStream = 0,
    RenderGame = 1,
    GoToGameSelect = 2,
    CloseDisconnects = 3,
    EnableControls = 4,
    MaxInstances = 5,
    AccountCreated = 6,
    PreExistingUser = 7,
    InvalidLogin = 8,
    DoesNotExist = 9,
    GoBackToMain = 10,
    Occupancy = 11,
    NoOccupancy = 12,
    GameBeingCreated = 13,
    ForwardToGame = 14,
    ActiveUser = 15
}

public class PlayerIO {
    public float time;
    public ButtonEnum button;
}

public class LoginInfo {
    public string username;
    public string password;
}

public class ServerInfo
{
    public string servername;
}

public class ClientConnection : Singleton<ClientConnection> {

    [SerializeField] private Canvas _gameCanvas;

    private string _username = "";

    private bool _connected = false;
    public bool Connected {
        get { return _connected; }
    }

    [SerializeField] private string serverIP = "";		//Server IP address
	[SerializeField] private int _bufferSize = 3000;	//Maximum size of receiving buffer
	private int _maxConnections = 1;	                //Maximum umber of connection

    HostTopology hostTopology;
    private int UDP_ChannelIDSeq = -1;
    private int UDP_ChannelIDFrag = -1;
    private int UDP_ChannelID = -1;
    private int TCP_ChannelID = -1;

    [Space]
    [Header("Connection IDs")]
	[SerializeField] private int MS_connectionID = -1;                      //Connection ID
    public int MSConnectionID {
        get { return MS_connectionID; }
    }
    [SerializeField] private int GS_connectionID = -1;

    [Space]
    [Header("Socket IDs")]
    [SerializeField] private int MS_socketID = -1;      //Socket ID
    public int MSSocketID {
        get { return MS_socketID; }
    }
    [SerializeField] private int GS_socketID = -1;

    [Space]
    [Header("Port Numbers")]
    [SerializeField] private int MS_socketPort = 8888;	//Port number
    [SerializeField] private int GS_socketPort = -1;

	[SerializeField] private Image _renderTo;								//Image to render to

    [SerializeField] private OnlineStatusWindow _statusWindow;
    [SerializeField] private ClientLobbyWindow _clientLobby;

    [Space]
    [Header("Timeout Settings")]
    // Latency Thresholds
    [SerializeField] private int _updateRate = 0;
    [SerializeField] private int _RTT_Threshold = 0;
    [SerializeField] private float _timeThreshold = 0;

    private int _currentFrame = -1;
    private ClientIO _clientIO;
    private float _disconnectTimer = 0.0f;
    private int _currentRTT = -1;
    private float _currentRate = 0.0f;

    private bool _connectToMaster = false;
    private bool _connectedToGame = false;


	private void Start() {
        Application.runInBackground = true;

        _clientIO = GetComponent<ClientIO>();
        _currentFrame = -1;

        NetworkTransport.Init();

        ConnectToMaster();
	}

	private void Update() {

        byte error;

        //_slowConnectTimer += Time.deltaTime;
        if (MS_connectionID != -1) {
            _currentRTT = NetworkTransport.GetCurrentRTT(MS_socketID, MS_connectionID, out error);

            if (Time.time - _currentRate > _updateRate) {
                Debug.Log(_currentRTT);
                _currentRate = Time.time;
            }
        }

		int incomingSocketID = 0;
		int incomingConnectionID = 0;
		int incomingChannelID = 0;

        // POSSIBLY MOVE THIS TO BE A PERMANENT VARIABLE TO SAVE ALLOCATION RESOURCES
		byte[] incomingMessageBuffer = new byte[_bufferSize];
		int dataSize = 0;

        NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
			out incomingChannelID, incomingMessageBuffer, _bufferSize, out dataSize, out error);

		switch (incomingNetworkEvent) {
		    case NetworkEventType.Nothing:
			    break;

		    case NetworkEventType.ConnectEvent:

                Debug.Log("Socket: " + incomingSocketID);
                Debug.Log("Connection: " + incomingConnectionID);
                
			    Debug.Log("client incoming connection event received");

                Debug.Log(incomingSocketID);

                // If connected to the master server, register as a client
                if (incomingSocketID == 0) {

                    if (!_connected) {
                        _connected = true;
                        _statusWindow.UpdateOnlineStatus(true);
                    }

                    string jsonToBeSent = "3";
                    jsonToBeSent += JsonUtility.ToJson("");
                    SendJSONMessageToMaster(jsonToBeSent, QosType.Reliable);
                }

                // Send all the user information to the Game server so it can be used in game.
                else if (incomingSocketID == 1) {

                    string jsonToBeSent = "3";
                    LoginInfo info = new LoginInfo();
                    info.username = _username;
                    jsonToBeSent += JsonUtility.ToJson(info) ;
                    SendJSONMessageToGame(jsonToBeSent, QosType.Reliable);
                }

                break;

            //0 for username/password info, 1 for PlayerIO
		    case NetworkEventType.DataEvent:
                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
                int prefix = 0;
                int index;
                string newMessage = "";

                // New parser now allows client commands to be > 1 digit
                for(index = 0; index < message.Length; ++index) {
                    if (message[index] == '{')
                        break;
                }

                prefix = Convert.ToInt32(message.Substring(0, index));
                newMessage = message.Substring(index);

                if (prefix == (int)ClientCommands.StartStream) {
                    _currentFrame = -1;
                    WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.ClientLobby);
                    _gameCanvas.gameObject.SetActive(true);
                }
                else if (prefix == (int)ClientCommands.RenderGame) {
                    Texture2D gameTexture = new Texture2D(0, 0);

                    ServerObject JSONdata = JsonUtility.FromJson<ServerObject>(newMessage);

                    //Debug.Log(JSONdata.frameNum);
                    // Latency Mitigation at its finest.
                    if (JSONdata.frameNum > _currentFrame) {
                        byte[] textureByteArray = Convert.FromBase64String(JSONdata.texture);
                        gameTexture.LoadImage(textureByteArray);
                        _currentFrame = JSONdata.frameNum;
                        _renderTo.GetComponent<CanvasRenderer>().SetTexture(gameTexture);
                    }
                }
                else if(prefix == (int)ClientCommands.GoToGameSelect) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.Login, WindowIDs.GameSelect);
                }
                else if (prefix == (int)ClientCommands.AccountCreated) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.NewAccount, WindowIDs.NewAccountSuccess);
                }
                else if (prefix == (int)ClientCommands.PreExistingUser) {
                    GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "";
                    GameObject.Find("UsernameError").GetComponent<Text>().text = "Username already exists. Choose a different username.";
                }
                else if (prefix == (int)ClientCommands.InvalidLogin) {
                    GameObject.Find("UsernameError").GetComponent<Text>().text = "";
                    GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Invalid username or password.";
                }
                else if (prefix == (int)ClientCommands.DoesNotExist) {
                    GameObject.Find("UsernameError").GetComponent<Text>().text = "";
                    GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Username does not exist in the database.";
                }
                else if (prefix == (int)ClientCommands.ActiveUser) {
                    GameObject.Find("UsernameError").GetComponent<Text>().text = "";
                    GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Username is already logged in.";
                }
                else if (prefix == (int)ClientCommands.CloseDisconnects) {
                    _clientLobby.CannotDisconnect();
                }
                else if (prefix == (int)ClientCommands.GoBackToMain) {
                    _gameCanvas.gameObject.SetActive(false);
                    WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.GameSelect);

                    byte e = 0;
                    NetworkTransport.Disconnect(GS_socketID, GS_connectionID, out e);
                }
                else if (prefix == (int)ClientCommands.Occupancy)
                {
                    //Do nothing... should connect
                }
                else if (prefix == (int)ClientCommands.NoOccupancy)
                {
                    WindowManager.Instance.ToggleWindows(WindowIDs.GameSelect, WindowIDs.FullLobby);
                }
                else if (prefix == (int)ClientCommands.ForwardToGame) {

                    GS_socketPort = JsonUtility.FromJson<PortID>(newMessage).portID;
                    Debug.Log("GS_socketPort: " + GS_socketPort);
                    ConnectToGame();
                }
                else if (prefix == (int)ClientCommands.EnableControls) {
                    _clientIO.gameInSession = true;
                    WindowManager.Instance.ToggleWindows(WindowManager.Instance.currentWindow, WindowIDs.None);
                } 
                else if (prefix == (int)ClientCommands.MaxInstances) {
                    Debug.Log("Printing newMessage: " + newMessage);
                    string serverNames = JsonUtility.FromJson<string>(newMessage);

                    GameObject.Find("MaxNumInstance").GetComponent<Text>().text = "Maximum number of instances created.";
                    GameObject.Find("AvailableInstance").GetComponent<Text>().text = "Available instances: " + serverNames;
                }
                else if (prefix == (int)ClientCommands.GameBeingCreated) {
                    GameObject.Find("PleaseWait").GetComponent<Text>().text = "Game is being loaded. Please wait a moment...";
                }
                break;

		    case NetworkEventType.DisconnectEvent:

                if (incomingSocketID == 0) {
                    Debug.Log("client: disconnected from Master Server");
                    _connected = false;
                    _gameCanvas.gameObject.SetActive(false);
                    WindowManager.Instance.ToggleWindows(WindowManager.Instance.currentWindow, WindowIDs.StartWindow);
                    WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.OnlineStatus);
                    _clientIO.gameInSession = false;
                    MS_connectionID = -1;

                    NetworkTransport.RemoveHost(MS_socketID);
                    MS_socketID = -1;
                    MS_connectionID = -1;

                    _statusWindow.UpdateOnlineStatus(false);
                }
                else if (incomingSocketID == 1) {
                    Debug.Log("client: disconnect from game server");

                    WindowManager.Instance.ToggleWindows(WindowManager.Instance.currentWindow, WindowIDs.GameSelect);
                    NetworkTransport.RemoveHost(GS_socketID);
                    _clientIO.gameInSession = false;
                    GS_socketID = -1;
                    GS_connectionID = -1;
                }

                break;
		    }

        if ((WindowManager.Instance.currentWindow == WindowIDs.None || WindowManager.Instance.currentWindow == WindowIDs.ClientLobby) 
            && _currentRTT >= _RTT_Threshold) {

            _disconnectTimer += Time.deltaTime;

            if (_disconnectTimer >= _timeThreshold) {
                //Network.Disconnect();
                Debug.Log("disconnect from slow conection");
                _gameCanvas.gameObject.SetActive(false);
                _clientIO.gameInSession = false;

                if (WindowManager.Instance.currentWindow == WindowIDs.ClientLobby) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.ClientLobby, WindowIDs.DisconnectWindow);
                }
                else if (WindowManager.Instance.currentWindow == WindowIDs.None) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.DisconnectWindow);
                    WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.OnlineStatus);
                }

                LeaveLobby();
            }
        }
        else {
            _disconnectTimer = 0.0f;
        }
	}

	public void ConnectToMaster() {

        byte error = 0;

        // If the Connection is not established, establish a new one.
        if (MS_socketID == -1) {

            ConnectionConfig config = new ConnectionConfig();

            TCP_ChannelID = config.AddChannel(QosType.Reliable);
            UDP_ChannelIDSeq = config.AddChannel(QosType.UnreliableSequenced);
            UDP_ChannelIDFrag = config.AddChannel(QosType.UnreliableFragmented);
            UDP_ChannelID = config.AddChannel(QosType.Unreliable);

            hostTopology = new HostTopology(config, _maxConnections);
            MS_socketID = NetworkTransport.AddHost(hostTopology, MS_socketPort);
        }

		MS_connectionID = NetworkTransport.Connect(MS_socketID, serverIP, MS_socketPort, 0, out error);
    }

    public void ConnectToGame() {

        GS_socketID = NetworkTransport.AddHost(hostTopology, GS_socketPort);

        byte error = 0;
        GS_connectionID = NetworkTransport.Connect(GS_socketID, serverIP, GS_socketPort, 0, out error);
    }

    private void SendJSONMessageToMaster(string JSONobject, QosType type) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        //Debug.Log("Sending message of length " + messageBuffer.Length);

        if (type == QosType.Reliable) {
            Debug.Log("Sending reliable message.");
            NetworkTransport.Send(MS_socketID, MS_connectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.Unreliable) {
            NetworkTransport.Send(MS_socketID, MS_connectionID, UDP_ChannelID, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.UnreliableFragmented) {
            NetworkTransport.Send(MS_socketID, MS_connectionID, UDP_ChannelIDFrag, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.UnreliableSequenced) {
            NetworkTransport.Send(MS_socketID, MS_connectionID, UDP_ChannelIDSeq, messageBuffer, messageBuffer.Length, out error);
        }
    }

    private void SendJSONMessageToGame(string JSONobject, QosType type) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);

        if (type == QosType.Reliable) {
            NetworkTransport.Send(GS_socketID, GS_connectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.Unreliable) {
            NetworkTransport.Send(GS_socketID, GS_connectionID, UDP_ChannelID, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.UnreliableFragmented) {
            NetworkTransport.Send(GS_socketID, GS_connectionID, UDP_ChannelIDFrag, messageBuffer, messageBuffer.Length, out error);
        }
        else if (type == QosType.UnreliableSequenced) {
            NetworkTransport.Send(GS_socketID, GS_connectionID, UDP_ChannelIDSeq, messageBuffer, messageBuffer.Length, out error);
        }
    }

    //Login
    public void SendMessage(LoginInfo info) {
        string jsonToBeSent = "0";
        _username = info.username;
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessageToMaster(jsonToBeSent, QosType.Reliable);
    }

    //Create Account
    public void SendMessageAccount(LoginInfo info) {
        string jsonToBeSent = "1";
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessageToMaster(jsonToBeSent, QosType.Reliable);
    }

    //Player IO
    public void SendMessage(PlayerIO command) {
        string jsonToBeSent = "2";
        jsonToBeSent += JsonUtility.ToJson(command);
        SendJSONMessageToGame(jsonToBeSent, QosType.Unreliable);
    }

    public void ConnectToGameServer(string serverName) {
        string jsonToBeSent = "7";
        ServerInfo info = new ServerInfo();
        info.servername = serverName;
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessageToMaster(jsonToBeSent, QosType.Reliable);
    }

    public void LeaveLobby() {
        string jsonToBeSent = "4";
        jsonToBeSent += JsonUtility.ToJson("na");

        // Tell the Game server to remove this client from the players list
        //SendJSONMessageToGame(jsonToBeSent, QosType.Reliable);
        _gameCanvas.gameObject.SetActive(false);

        byte error;
        NetworkTransport.Disconnect(GS_socketID, GS_connectionID, out error);
    }

    /// <summary>
    /// Asks the server if there is room for the client to join in.
    /// </summary>
    public void verifyOccupancy()
    {
        string jsonToBeSent = "6";
        jsonToBeSent += JsonUtility.ToJson("");
        //SendJSONMessage(jsonToBeSent);
    }
}
