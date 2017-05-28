//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.IO;
//using UnityEngine.UI;
//using System;
//using System.Text;
//using UnityEngine.SceneManagement;

//public enum ServerCommands {
//    VerifyLogin = 0,
//    CreateAccount = 1,
//    PlayerInput = 2,
//    SetUsername = 3,
//    LeaveLobby = 4,
//    LeaveGame = 5,
//    VerifyOccupancy = 6,
//    SelectGameInstance = 7
//}

//public class ServerObject {
//    public ServerObject(int currentFrame, string tex) {
//        frameNum = currentFrame;
//        texture = tex;
//    }

//    public int frameNum;
//    public string texture;
//}

///*** CLIENT VARIABLES ***/
//// Encapsulated client info
//public class ClientInfo {
//    public ClientInfo() { }
//    public ClientInfo(int socket, int connection, int channel) {
//        socketID = socket;
//        ConnectionID = connection;
//        ChannelID = channel;
//    }

//    public int socketID = -1;
//    public int ConnectionID = -1;
//    public int ChannelID = -1;
//    public string username = "";
//    public int playerNum = -1;
//}

//public class ServerConnection : Singleton<ServerConnection> {
//    /*** RENDER VARIABLES ***/
//    [SerializeField] private RenderTexture rt;  //Target render texture
//    [SerializeField] private Camera _cam;       //Camera to render from

//    /*** Apache Variables ***/
//    /// <summary>
//    /// The port of the authentication server, defaults to 80
//    /// </summary>
//    public string apachePort;

//    /*** PHP VARIABLES ***/
//    private string _LoginURL = "http://localhost/teamnewport/LoginManager.php";
//    private string _CreateAccountURL = "http://localhost/teamnewport/CreateAccount.php";

//    // Maps connectionID with ClientInfo
//    private Dictionary<int, ClientInfo> _clientSocketIDs = new Dictionary<int, ClientInfo>();

//    /*** SERVER VARIABLES ***/
//    [SerializeField] private int _incomingBufferSize = 3000;    // max buffer size
//    [SerializeField] private int _maxConnections = 0;           // max number of connections


//    public int MaxConnections {
//        get { return _maxConnections; }
//    }

//    private int UDP_ChannelIDFrag = -1;                         // This channel should be reserved for larger messages
//    private int _socketID = -1;
//    private int _connectionID = -1;
//    [SerializeField] private int _socketPort = 8888;
//    [SerializeField] private int _numberOfConnections = 0;
//    public int NumberOfConnections {
//        get { return _numberOfConnections; }
//    }

//    /*** PLAYER VARIABLES ***/
//    [SerializeField] private int _inGamePlayers = 0;            // Players who have successfully logged in
//    public int InGamePlayers {
//        get { return _inGamePlayers; }
//        set { _inGamePlayers = value; }
//    }

//    [SerializeField] private LobbyWindow _lobby;
//    [SerializeField] private NotificationArea _notifArea;
//    private void OnEnable() {
//        _cam = Camera.main;
//    }

//    // Initialization
//    void Start() {
//        NetworkTransport.Init();

//        if (apachePort != "80" || apachePort != "") {
//            _LoginURL = "http://localhost:" + apachePort + "/teamnewport/LoginManager.php";
//            _CreateAccountURL = "http://localhost:" + apachePort + "/teamnewport/CreateAccount.php";
//        }

//        ConnectionConfig connectionConfig = new ConnectionConfig();
//        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);

//        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
//        int portDelta = 0;
//        while (portDelta < 10)
//        {
//            try {_socketID = NetworkTransport.AddHost(hostTopology, _socketPort + portDelta);
//                if (_socketID < 0)
//                    throw new Exception();
//                else
//                    break;
//            }
//            catch (Exception e)
//            {
//                Debug.Log("GOTCHA BITCH. The exception was: " + e);
//                portDelta++;
//            }
//        }

//        Debug.Log("Port: " + (_socketPort + portDelta));

//        Application.runInBackground = true;
//    }

//    void Update() {
//        CaptureFrame();

//        int incomingSocketID = -1;
//        int incomingConnectionID = -1;
//        int incomingChannelID = -1;
//        byte[] incomingMessageBuffer = new byte[_incomingBufferSize];
//        int dataSize = 0;
//        byte error;

//        NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
//            out incomingChannelID, incomingMessageBuffer, _incomingBufferSize, out dataSize, out error);

//        switch (incomingNetworkEvent) {
//            case NetworkEventType.Nothing:
//                break;

//            case NetworkEventType.ConnectEvent:
//                Debug.Log("server: new client connected");
//                ClientInfo clientInfo = new ClientInfo();
//                clientInfo.socketID = incomingSocketID;
//                clientInfo.ConnectionID = incomingConnectionID;
//                clientInfo.ChannelID = incomingChannelID;
//                _clientSocketIDs.Add(incomingConnectionID, clientInfo);

//                _numberOfConnections = _clientSocketIDs.Count;
//                break;

//            case NetworkEventType.DataEvent:
//                //Debug.Log("server: Message received. Message size: " + incomingMessageBuffer.Length);

//                //Test Code
//                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
//                //End Test Code

//                int prefix = Convert.ToInt32(message.Substring(0, 1));
//                string newMessage = message.Substring(1);
                
//                //process login info
//                if (prefix == (int)ServerCommands.VerifyLogin) {
//                    LoginInfo info = JsonUtility.FromJson<LoginInfo>(newMessage);
//                    StartCoroutine(verifyLogin(info.username, info.password, incomingSocketID, incomingConnectionID, incomingChannelID));
//                }

//                //process create account info
//                else if (prefix == (int)ServerCommands.CreateAccount) {
//                    LoginInfo info = JsonUtility.FromJson<LoginInfo>(newMessage);
//                    StartCoroutine(CreateUser(info.username, info.password, incomingSocketID, incomingConnectionID, incomingChannelID));
//                }

//                //process user game input
//                else if (prefix == (int)ServerCommands.PlayerInput) {
//                    PlayerIO input = JsonUtility.FromJson<PlayerIO>(newMessage);
//                    Debug.Log(incomingConnectionID + " is moving " + input.button);
//                    GameManager.Instance.PlayerActions(incomingConnectionID, input);
//                }

//                else if (prefix == (int)ServerCommands.SetUsername) {
//                    _clientSocketIDs[incomingConnectionID].username = message;
//                    _notifArea.playerEntered(message);
//                }
//                else if (prefix == (int)ServerCommands.LeaveLobby) {

//                    _inGamePlayers--;
//                    if (_inGamePlayers < 0) { _inGamePlayers = 0; }
//                    if (_lobby.gameObject.activeInHierarchy == true) {
//                        _lobby.RemovePlayerFromLobby(_clientSocketIDs[incomingConnectionID].playerNum);
//                    }

//                    if (_inGamePlayers < 1) {
//                        GameManager.Instance.ResetGameManager();
//                        SceneManager.LoadScene("Server Game Version");
//                    }
//                }
//                else if (prefix == (int)ServerCommands.LeaveGame) {

//                    _inGamePlayers--;
//                    if (_inGamePlayers < 0) { _inGamePlayers = 0; }
                    
//                    if (_inGamePlayers < 1) {
//                        GameManager.Instance.ResetGameManager();
//                        SceneManager.LoadScene("Server Game Version");
//                    }
//                }
//                else if (prefix == (int)ServerCommands.VerifyOccupancy)
//                {
//                    sendOccupancy(incomingSocketID, incomingConnectionID, incomingChannelID);
//                }
//                break;

//            case NetworkEventType.DisconnectEvent:
//                Debug.Log("server: remote client event disconnected");
//                //_notifArea.playerLeft(_clientSocketIDs[_connectionID].username);
//                GameManager.Instance.LeaveGame(incomingConnectionID);
//                _clientSocketIDs.Remove(incomingConnectionID);

//                // Decrement the number of players and remove the player from the hashmap.
//                _inGamePlayers--;
//                if (_inGamePlayers < 0) { _inGamePlayers = 0; }
//                _numberOfConnections--;
//                if (_numberOfConnections < 0) { _numberOfConnections = 0; }

//                // If the lobby is currently showing, make sure to update the information.
//                if (_lobby.gameObject.activeInHierarchy == true) {
//                    _lobby.RemovePlayerFromLobby(_clientSocketIDs[incomingConnectionID].playerNum);
//                }

//                ClientInfo clientToDelete = new ClientInfo(-1, -1, -1);

//                foreach (KeyValuePair<int, ClientInfo> client in _clientSocketIDs) {
//                    if (client.Value.ConnectionID == incomingConnectionID) {
//                        clientToDelete = client.Value;
//                    }
//                }

//                if (clientToDelete.socketID != -1) {
//                    _clientSocketIDs.Remove(clientToDelete.ConnectionID);
//                }

//                if (_inGamePlayers < 1) {
//                    GameManager.Instance.ResetGameManager();
//                    SceneManager.LoadScene("Server Game Version");
//                }
//                break;
//        }
//    }

//    public void SendJSONMessage(string JSONobject) {

//        byte error = 0;
//        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
//        //Debug.Log("Sending message of length " + messageBuffer.Length);
//        foreach (KeyValuePair<int, ClientInfo> client in _clientSocketIDs) {
//            NetworkTransport.Send(client.Value.socketID, client.Value.ConnectionID, client.Value.ChannelID, messageBuffer, messageBuffer.Length, out error);
//        }
//    }

//    /// <summary>
//    /// Sends the JSON message to one specific client.
//    /// </summary>
//    /// <param name="JSONobject"></param>
//    /// <param name="socketID"></param>
//    /// <param name="connectionID"></param>
//    /// <param name="channelID"></param>
//    public void SendOneJSONMessage(string JSONobject, int socketID, int connectionID, int channelID)
//    {
//        byte error = 0;
//        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
//        NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
//    }
//    /// <summary>
//    /// Sends to the client whether or not there is room for them to join in.
//    /// </summary>
//    /// <param name="socketID"></param>
//    /// <param name="connectionID"></param>
//    /// <param name="channelID"></param>
//    public void sendOccupancy(int socketID, int connectionID, int channelID)
//    {
//        string jsonToBeSent;
//        if (_numberOfConnections == _maxConnections)
//            jsonToBeSent = "12";
//        else
//            jsonToBeSent = "11";
//        SendOneJSONMessage(jsonToBeSent, socketID, connectionID, channelID);
//    }

//    void CaptureFrame() {
//        RenderTexture.active = rt;
//        Camera.main.Render();

//        Texture2D tex = new Texture2D(_cam.targetTexture.width, _cam.targetTexture.height, TextureFormat.RGB24, false);
//        tex.ReadPixels(new Rect(0, 0, _cam.targetTexture.width, _cam.targetTexture.height), 0, 0);
//        tex.Apply();
//        byte[] image = tex.EncodeToPNG();

//        // Create a new Server object and populate its attributes
//        ServerObject toBeSent = new ServerObject(Time.frameCount, Convert.ToBase64String(image));

//        // Convert to JSON
//        string jsonToBeSent = "1";
//        jsonToBeSent += JsonUtility.ToJson(toBeSent);

//        // Once we have at least 1 successfully logged in player, we should begin to transmit the lobby/game.
//        if (_inGamePlayers > 0) {
//            SendJSONMessage(jsonToBeSent);
//        }
//    }

//    private IEnumerator verifyLogin(string username, string password, int socketID, int connectionID, int channelID) {
//        WWWForm form = new WWWForm();
//        form.AddField("usernamePost", username);
//        form.AddField("passwordPost", password);

//        WWW verify = new WWW(_LoginURL, form);
//        yield return verify;

//        if (verify.text == "valid") {
//            WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.Lobby);

//            byte error;
//            string jsonToBeSent = "0";
//            jsonToBeSent += JsonUtility.ToJson(verify.text);
//            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
//            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
            
//            _clientSocketIDs[connectionID].username = username;
//            _clientSocketIDs[connectionID].playerNum = connectionID-1; // decremented so the range starts with 0 and not 1

//            Debug.Log(username + " logged in with connection id: " + connectionID);
//            Debug.Log(username + " playerNum is " + _clientSocketIDs[connectionID].playerNum);
            
//            // IF the lobby is not loaded, load it.
//            if (WindowManager.Instance.currentWindow == WindowIDs.None) {
//                WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.Lobby);
//            }

//            // Tell the lobby to add this player so it shows in the lobby.
//            _lobby.AddPlayerToLobby(_clientSocketIDs[connectionID].username, _clientSocketIDs[connectionID].playerNum);
//            _inGamePlayers++;
//        }
//        else if (verify.text == "invalid") {

//            byte error;
//            string jsonToBeSent = "8";
//            jsonToBeSent += JsonUtility.ToJson(verify.text);
//            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
//            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

//        }
//        else if (verify.text == "user not found") {

//            byte error;
//            string jsonToBeSent = "9";
//            jsonToBeSent += JsonUtility.ToJson(verify.text);
//            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
//            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
//        }
//    }

//    private IEnumerator CreateUser(string username, string password, int socketID, int connectionID, int channelID) {
//        Debug.Log(username + " created an account with password " + password);

//        WWWForm form = new WWWForm();
//        form.AddField("usernamePost", username);
//        form.AddField("passwordPost", password);

//        WWW verify = new WWW(_CreateAccountURL, form);

//        yield return verify; ;

//        if (verify.text == "username exists") {
//            byte error;
//            string jsonToBeSent = "7";
//            jsonToBeSent += JsonUtility.ToJson(verify.text);
//            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
//            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

//        }
//        else if (verify.text == "account created") {
//            Debug.Log("account was created");
//            byte error;
//            string jsonToBeSent = "6";
//            jsonToBeSent += JsonUtility.ToJson(verify.text);
//            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
//            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
//        }
//    }

//    public void EnableClientControls() {
//        string toBeSent = "2";
//        SendJSONMessage(toBeSent);
//    }

//    public void PreventDisconnects() {
//        string toBeSent = "3";
//        SendJSONMessage(toBeSent);
//    }
//}

