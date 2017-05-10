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

    [SerializeField] private string serverIP = "";		//Server IP address
	[SerializeField] private int _bufferSize = 3000;	//Maximum size of receiving buffer
	private int _maxConnections = 1;	                //Maximum umber of connection

	private int UDP_ChannelIDFrag = -1;					//UDP communication channel for large messages
	private int _connectionID = -1;						//Connection ID

	[SerializeField] private int _socketID = -1;		//Socket ID
	[SerializeField] private int _socketPort = 8888;	//Port number

	[SerializeField] private Image _renderTo;								//Image to render to

	private void Start() {
		NetworkTransport.Init();
		ConnectionConfig connectionConfig = new ConnectionConfig();
		UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);
		HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
		_socketID = NetworkTransport.AddHost(hostTopology, _socketPort);
		Connect();
	}

	private void Update() {
		int incomingSocketID = 0;
		int incomingConnectionID = 0;
		int incomingChannelID = 0;
		byte[] incomingMessageBuffer = new byte[_bufferSize];
		int dataSize = 0;
		byte error;

		NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
			out incomingChannelID, incomingMessageBuffer, _bufferSize, out dataSize, out error);

		switch (incomingNetworkEvent) {
		    case NetworkEventType.Nothing:
			    break;

		    case NetworkEventType.ConnectEvent:
			    Debug.Log("client incoming connection event received");
			    break;

            //0 for username/password info, 1 for PlayerIO
		    case NetworkEventType.DataEvent:

                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
                string prefix = message.Substring(0, 1);
                string newMessage = message.Substring(1);

                if (prefix == "0") {
                    WindowManager.Instance.ToggleWindows(WindowIDs.Login, WindowIDs.None);
                    _gameCanvas.enabled = true;
                    break;
                }
                else if (prefix == "1") {
                    Texture2D gameTexture = new Texture2D(0, 0);

                    ServerObject JSONdata = JsonUtility.FromJson<ServerObject>(message);
                    byte[] textureByteArray = Convert.FromBase64String(JSONdata.texture);


                    gameTexture.LoadImage(textureByteArray);
                    _renderTo.GetComponent<CanvasRenderer>().SetTexture(gameTexture);
                    break;
                }
                break;

		    case NetworkEventType.DisconnectEvent:
			    Debug.Log("client: remote client event disconnected");
                _gameCanvas.enabled = false;
                WindowManager.Instance.ToggleWindows(WindowIDs.None, WindowIDs.StartWindow);
			    break;
		    }
	}

	private void Connect() {
		byte error = 0;
		_connectionID = NetworkTransport.Connect(_socketID, serverIP, _socketPort, 0, out error);
	}

    private void SendJSONMessage(string JSONobject) {
        byte error = 0;
        //byte[] messageBuffer = new byte[_bufferSize];
        //Stream stream = new MemoryStream(messageBuffer);
        //BinaryFormatter formatter = new BinaryFormatter();
        //formatter.Serialize(stream, JSONobject);
        //Test Code
        byte[] messageBuffer = Encoding.UTF8.GetBytes(JSONobject);
        Debug.Log("Sending message of length " + messageBuffer.Length);

        //End Test Code

        NetworkTransport.Send(_socketID, _connectionID, UDP_ChannelIDFrag, messageBuffer, messageBuffer.Length/*_bufferSize*/, out error);
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
}
