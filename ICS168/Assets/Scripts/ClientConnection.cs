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
    SetGameInSession = 2,
    CloseDisconnects = 3,
    AccountCreated = 6,
    PreExistingUser = 7,
    InvalidLogin = 8,
    DoesNotExist = 9,
    GoBackToMain = 10
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

	private int UDP_ChannelIDFrag = -1;					//UDP communication channel for large messages
	[SerializeField] private int _connectionID = -1;						//Connection ID

	[SerializeField] private int _socketID = -1;		//Socket ID
	[SerializeField] private int _socketPort = 8888;	//Port number

	[SerializeField] private Image _renderTo;								//Image to render to
    private int _currentFrame = -1;

    private ClientIO _clientIO;

    [SerializeField] private OnlineStatusWindow _statusWindow;
    [SerializeField] private ClientLobbyWindow _clientLobby;

    // Latency Thresholds
    [SerializeField] private int _RTT_Threshold;
    [SerializeField] private float _timeThreshold;
    private float _disconnectTimer = 0.0f;
    private int _currentRTT = -1;


	private void Start() {
        _clientIO = GetComponent<ClientIO>();
        _currentFrame = -1;

        NetworkTransport.Init();
        ConnectionConfig connectionConfig = new ConnectionConfig();
        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);
        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);

        Connect();
	}

	private void Update() {

        byte error;

        //_slowConnectTimer += Time.deltaTime;
        if (_connectionID != -1) {
            _currentRTT = NetworkTransport.GetCurrentRTT(_socketID, _connectionID, out error);
            Debug.Log( _currentRTT );
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
                _connected = true;
                _statusWindow.UpdateOnlineStatus(true);
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
                else if(prefix == (int)ClientCommands.SetGameInSession) {
                    WindowManager.Instance.ToggleWindows(WindowIDs.ClientLobby, WindowIDs.None);
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
                break;

		    case NetworkEventType.DisconnectEvent:
			    Debug.Log("client: remote client event disconnected");
                _connected = false;
                _gameCanvas.gameObject.SetActive(false);
                WindowManager.Instance.ToggleWindows(WindowManager.Instance.currentWindow, WindowIDs.StartWindow);
                WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.OnlineStatus);
                _clientIO.gameInSession = false;
                _connectionID = -1;

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

	public void Connect() {

        byte error = 0;
		_connectionID = NetworkTransport.Connect(_socketID, serverIP, _socketPort, 0, out error);
    }

    private void SendJSONMessage(string JSONobject) {
        byte error = 0;
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        //Debug.Log("Sending message of length " + messageBuffer.Length);
        NetworkTransport.Send(_socketID, _connectionID, UDP_ChannelIDFrag, messageBuffer, messageBuffer.Length, out error);
    }

    //Login
    public void SendMessage(LoginInfo info) {
        string jsonToBeSent = "0";
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessage(jsonToBeSent);
    }

    //Create Account
    public void SendMessageAccount(LoginInfo info) {
        string jsonToBeSent = "1";
        jsonToBeSent += JsonUtility.ToJson(info);
        SendJSONMessage(jsonToBeSent);
    }

    //Player IO
    public void SendMessage(PlayerIO command) {
        string jsonToBeSent = "2";
        jsonToBeSent += JsonUtility.ToJson(command);
        SendJSONMessage(jsonToBeSent);
    }

    public void LeaveLobby() {
        string jsonToBeSent = "4";
        jsonToBeSent += JsonUtility.ToJson("");
        SendJSONMessage(jsonToBeSent);
        _gameCanvas.gameObject.SetActive(false);
    }
}
