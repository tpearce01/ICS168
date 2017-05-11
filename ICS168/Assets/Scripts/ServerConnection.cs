using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;

public class ServerObject {
    //public float time;
    public string texture;
}

public class ServerConnection : Singleton<ServerConnection>
{
    [SerializeField] private RenderTexture rt;    //Target render texture
    [SerializeField] private Camera _cam;          //Camera to render from

    private string _LoginURL = "http://localhost/teamnewport/LoginManager.php";
    private string _CreateAccountURL = "http://localhost/teamnewport/CreateAccount.php";

    private class ClientInfo {
        public int socketID = -1;
        public int ConnectionID = -1;
        public int ChannelID = -1;
    }

    [SerializeField]
    private int _bufferSize = 3000;
    [SerializeField] private int _maxConnections = 0;

    private int UDP_ChannelIDFrag = -1;             // This channel should be reserved for larger messages
    private int _socketID = -1;
    [SerializeField] private int _socketPort = 8888;
    private int _connectionID = -1;

    private HashSet<ClientInfo> _clientSocketIDs = new HashSet<ClientInfo>();
    private int _numberOfConnections = -1;
    public int NumberOfConnections {
        get { return _numberOfConnections; }
    }

    private void OnEnable() {
        _cam = Camera.main;
    }

    void Start () {
        NetworkTransport.Init();
        ConnectionConfig connectionConfig = new ConnectionConfig();
        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);
        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);
    }
	
	void Update () {
        CaptureFrame();

        int incomingSocketID = -1;
        int incomingConnectionID = -1;
        int incomingChannelID = -1;
        byte[] incomingMessageBuffer = new byte[_bufferSize];
        int dataSize = 0;
        byte error;

        NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
            out incomingChannelID, incomingMessageBuffer, _bufferSize, out dataSize, out error);

        switch (incomingNetworkEvent) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("server: new client connected");
                ClientInfo clientInfo = new ClientInfo();
                clientInfo.socketID = incomingSocketID;
                clientInfo.ConnectionID = incomingConnectionID;
                clientInfo.ChannelID = incomingChannelID;
                _clientSocketIDs.Add(clientInfo);
                _numberOfConnections = _clientSocketIDs.Count;
                //GameManager.Instance
                break;

            case NetworkEventType.DataEvent:
                Debug.Log("server: Message received. Message size: " + incomingMessageBuffer.Length);
                //Stream stream = new MemoryStream(incomingMessageBuffer);
                //BinaryFormatter formatter = new BinaryFormatter();
                //string message = formatter.Deserialize(stream) as string;
                //Test Code
                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
                //End Test Code

                string prefix = message.Substring(0,1);
                string newMessage = message.Substring(1);

                if (prefix == "0") {
                    //process login info
                    LoginInfo info = JsonUtility.FromJson<LoginInfo>(newMessage);
                    StartCoroutine(verifyLogin(info.username, info.password, incomingSocketID, incomingConnectionID, incomingChannelID));
                } else if (prefix == "1") {
                    //process create account info
                    LoginInfo info = JsonUtility.FromJson<LoginInfo>(newMessage);
                    StartCoroutine(CreateUser(info.username, info.password, incomingSocketID, incomingConnectionID, incomingChannelID));
                } else if (prefix == "2") {
                    //process user game input
                    PlayerIO input = JsonUtility.FromJson<PlayerIO>(newMessage);
                    Debug.Log(incomingConnectionID);
                    GameManager.Instance.PlayerActions(incomingConnectionID, input);
                }
                
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("server: remote client event disconnected");
                GameManager.Instance.SignedInPlayers -= 1;
                break;
        }
    }

    public void SendJSONMessage(string JSONobject) {
        byte error = 0;
        //byte[] messageBuffer = new byte[_bufferSize];
        //Stream stream = new MemoryStream(messageBuffer);
        //BinaryFormatter formatter = new BinaryFormatter();
        //formatter.Serialize(stream, JSONobject);
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        Debug.Log("Sending message of length " + messageBuffer.Length);
        foreach (ClientInfo client in _clientSocketIDs) {
            NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, messageBuffer, _bufferSize, out error);
            //Debug.Log("Message Sent");
        }
    }

    void CaptureFrame() {
        RenderTexture.active = rt;
        Camera.main.Render();
        Texture2D tex = new Texture2D(_cam.targetTexture.width, _cam.targetTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0,0, _cam.targetTexture.width, _cam.targetTexture.height), 0,0);
        tex.Apply();
        byte[] image = tex.EncodeToPNG();

        // Create a new Server object and populate its attributes
        ServerObject toBeSent = new ServerObject();
        //toBeSent.time = Time.time;
        toBeSent.texture = Convert.ToBase64String(image);

        // Convert to JSON
        string jsonToBeSent = "1";
        jsonToBeSent += JsonUtility.ToJson(toBeSent);

        if (_numberOfConnections > 0) {
            SendJSONMessage(jsonToBeSent);
        }

        //byte error;
        //foreach (ClientInfo client in _clientSocketIDs)
        //{
        //    NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, image, image.Length, out error);
        //}
    }

    private IEnumerator verifyLogin(string username, string password, int socketID, int connectionID, int channelID) {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(_LoginURL, form);
        yield return verify;

        if (verify.text == "valid") {
            WindowManager.Instance.ToggleWindows(WindowIDs.Login, WindowIDs.Lobby);

            byte error;
            string jsonToBeSent = "0";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

            GameManager.Instance.SignedInPlayers += 1;

        } else if (verify.text == "invalid") {
            byte error;
            string jsonToBeSent = "8";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
        } else if (verify.text == "user not found") {
            byte error;
            string jsonToBeSent = "9";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
        }
    }

    private IEnumerator CreateUser(string username, string password, int socketID, int connectionID, int channelID) {
        Debug.Log(username + " " + password);
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        WWW verify = new WWW(_CreateAccountURL, form);
        yield return verify; ;

        if (verify.text == "username exists") {
            //GameObject.Find("UsernameError").GetComponent<Text>().text = "Username already exists. Choose a different username.";
            byte error;
            string jsonToBeSent = "7";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);

        } else if (verify.text == "account created") {
            Debug.Log("account was created");
            //WindowManager.Instance.ToggleWindows(WindowIDs.NewAccount, WindowIDs.NewAccountSuccess);
            byte error;
            string jsonToBeSent = "6";
            jsonToBeSent += JsonUtility.ToJson(verify.text);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
            NetworkTransport.Send(socketID, connectionID, channelID, messageBuffer, messageBuffer.Length, out error);
        }
    }


    public void EnableClientControls() {
        string toBeSent = "2";
        SendJSONMessage(toBeSent);
    }
}
