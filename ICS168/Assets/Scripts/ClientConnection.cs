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
    AccountCreated = 6,
    PreExistingUser = 7,
    InvalidLogin = 8,
    DoesNotExist = 9,
    GoBackToMain = 10,
    Occupancy = 11,
    NoOccupancy = 12,
    ForwardToGame = 14
}

public class PlayerIO {
    public float time;
    public ButtonEnum button;
}

public class LoginInfo {
    public string username;
    public string password;
}

public class ClientConnection : Singleton<ClientConnection> {

    [SerializeField] private Canvas _gameCanvas;

    private bool _connected = false;
    public bool Connected {
        get { return _connected; }
    }

    [SerializeField] private string serverIP = "";		//Server IP address
	[SerializeField] private int _bufferSize = 3000;	//Maximum size of receiving buffer
	private int _maxConnections = 1;	                //Maximum umber of connection

    // MASTER SERVER CONNECTION INFO
	//private int TCP_Frag_ChannelID = -1;					//UDP communication channel for large messages
    private int MS_TCP_ChannelID = -1;
	[SerializeField] private int MS_connectionID = -1;						//Connection ID
	[SerializeField] private int MS_socketID = -1;		//Socket ID
	[SerializeField] private int MS_socketPort = 8888;	//Port number

    // GAME SERVER CONNECTION INFO
    private int GS_TCP_ChannelID = -1;
    private int GS_connectionID = -1;
    private int GS_socketID = -1;
    private int GS_socketPort = -1;

	[SerializeField] private Image _renderTo;								//Image to render to
    private int _currentFrame = -1;

    private ClientIO _clientIO;

    [SerializeField] private OnlineStatusWindow _statusWindow;
    [SerializeField] private ClientLobbyWindow _clientLobby;

    [Space]
    [Header("Timeout Settings")]
    // Latency Thresholds
    [SerializeField] private int _updateRate = 0;
    [SerializeField] private int _RTT_Threshold = 0;
    [SerializeField] private float _timeThreshold = 0;
    private float _disconnectTimer = 0.0f;
    private int _currentRTT = -1;
    private float _currentRate = 0.0f;


	private void Start() {
        _clientIO = GetComponent<ClientIO>();
        _currentFrame = -1;

        NetworkTransport.Init();
        ConnectionConfig MSconnectionConfig = new ConnectionConfig();
        //TCP_Frag_ChannelID = connectionConfig.AddChannel(QosType.ReliableFragmented);
        MS_TCP_ChannelID = MSconnectionConfig.AddChannel(QosType.Reliable);
        HostTopology hostTopology = new HostTopology(MSconnectionConfig, _maxConnections);
        MS_socketID = NetworkTransport.AddHost(hostTopology, MS_socketPort);

        Application.runInBackground = true;
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
                
			    Debug.Log("client incoming connection event received");

                if (!_connected) {
                    _connected = true;
                    _statusWindow.UpdateOnlineStatus(true);
                }

                // If connected to the master server, register as a client
                if (incomingSocketID == 1) {
                    string jsonToBeSent = "3";
                    byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
                    NetworkTransport.Send(MS_socketID, MS_connectionID, MS_TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
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
                    WindowManager.Instance.ToggleWindows(WindowIDs.Login, WindowIDs.ClientLobby);
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

                        //_slowConnectTimer = 0.0f;
                    }
                }
                else if(prefix == (int)ClientCommands.GoToGameSelect) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.Login, WindowIDs.GameSelect);
                    _clientIO.gameInSession = true;
                }
                else if (prefix == (int)ClientCommands.AccountCreated) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.NewAccount, WindowIDs.NewAccountSuccess);
                }
                else if (prefix == (int)ClientCommands.PreExistingUser) {
                    GameObject.Find("UsernameError").GetComponent<Text>().text = "Username already exists. Choose a different username.";
                }
                else if (prefix == (int)ClientCommands.InvalidLogin) {
                    GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Invalid username or password.";
                }
                else if (prefix == (int)ClientCommands.DoesNotExist) {
                    GameObject.Find("LoginUsernameError").GetComponent<Text>().text = "Username does not exist in the database.";
                }
                else if (prefix == (int)ClientCommands.CloseDisconnects) {
                    _clientLobby.CannotDisconnect();
                }
                else if (prefix == (int)ClientCommands.GoBackToMain) {
                    Debug.Log("Go back to main damn it!");
                    _gameCanvas.gameObject.SetActive(false);
                    WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.StartWindow);
                }
                else if (prefix == (int)ClientCommands.Occupancy)
                {

                }
                else if (prefix == (int)ClientCommands.NoOccupancy)
                {

                }
                else if (prefix == (int)ClientCommands.ForwardToGame) {

                    ConnectionConfig GSconnectionConfig = new ConnectionConfig();
                    GS_TCP_ChannelID = GSconnectionConfig.AddChannel(QosType.Reliable);
                    HostTopology hostTopology = new HostTopology(GSconnectionConfig, _maxConnections);
                    GS_socketPort = JsonUtility.FromJson<PortID>(newMessage).portID;
                    GS_socketID = NetworkTransport.AddHost(hostTopology, GS_socketPort);

                    Application.runInBackground = true;
                    ConnectToGame();
                }
                break;

		    case NetworkEventType.DisconnectEvent:
			    Debug.Log("client: remote client event disconnected");
                _connected = false;
                _gameCanvas.gameObject.SetActive(false);
                WindowManager.Instance.ToggleWindows(WindowManager.Instance.currentWindow, WindowIDs.StartWindow);
                WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.OnlineStatus);
                _clientIO.gameInSession = false;
                //_connectionID = -1;

                _statusWindow.UpdateOnlineStatus(false);
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
		MS_connectionID = NetworkTransport.Connect(MS_socketID, serverIP, MS_socketPort, 0, out error);
    }

    public void ConnectToGame() {
        byte error = 0;
        GS_connectionID = NetworkTransport.Connect(GS_socketID, serverIP, GS_socketPort, 0, out error);
    }

    private void SendJSONMessageToMaster(string JSONobject) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        //Debug.Log("Sending message of length " + messageBuffer.Length);
        NetworkTransport.Send(MS_socketID, MS_connectionID, MS_TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
    }

    private void SendJSONMessageToGame(string JSONobject) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        //Debug.Log("Sending message of length " + messageBuffer.Length);
        NetworkTransport.Send(GS_socketID, GS_connectionID, GS_TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
    }

    //Login
    public void SendMessage(LoginInfo info) {
        string jsonToBeSent = "0";
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessageToMaster(jsonToBeSent);
    }

    //Create Account
    public void SendMessageAccount(LoginInfo info) {
        string jsonToBeSent = "1";
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessageToMaster(jsonToBeSent);
    }

    //Player IO
    //public void SendMessage(PlayerIO command) {
    //    string jsonToBeSent = "2";
    //    jsonToBeSent += JsonUtility.ToJson(command);
    //    SendJSONMessageToGame(jsonToBeSent);
    //}

    public void ConnectToGameServer(string serverName) {
        string jsonToBeSent = "7";
        jsonToBeSent += JsonUtility.ToJson(serverName);
        SendJSONMessageToGame(jsonToBeSent);
    }

    public void LeaveLobby() {
        string jsonToBeSent = ((int)GameServerCommands.LeaveLobby).ToString();
        jsonToBeSent += JsonUtility.ToJson("");
        SendJSONMessageToGame(jsonToBeSent);
        _gameCanvas.gameObject.SetActive(false);
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
