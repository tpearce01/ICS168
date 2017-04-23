using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ServerConnection : MonoBehaviour {

    private int i = 0;

    private class ServerObject {
        public float time;
        public Texture2D texture2d;
    }

    private class ClientInfo {
        public int socketID = -1;
        public int ConnectionID = -1;
        public int ChannelID = -1;
    }

    [SerializeField]
    private int _bufferSize = 3000;
    [SerializeField] private int _maxConnections = 0;


    private int UDP_ChannelID = -1;                 // This channel should be reserved for small message
    private int UDP_ChannelIDFrag = -1;             // This channel should be reserved for larger messages
    private int _socketID = -1;
    [SerializeField] private int _socketPort = 8888;
    private int _connectionID = -1;

    private HashSet<ClientInfo> _clientSocketIDs = new HashSet<ClientInfo>();
    private int _numberOfConnections = -1;

    // Use this for initialization
    void Start () {
        NetworkTransport.Init();
        ConnectionConfig connectionConfig = new ConnectionConfig();

        //UDP_ChannelID = connectionConfig.AddChannel(QosType.Unreliable);
        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.UnreliableFragmented);
        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);
    }
	
	// Update is called once per frame
	void Update () {
        CaptureFrame("ss.png");

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
                break;

            case NetworkEventType.DataEvent:
                //Debug.Log("server: Message received. Message size: " + incomingMessageBuffer.Length);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("server: remote client event disconnected");
                break;
        }
    }

    public void SendJSONMessage(string jsonObject) {

        if (_numberOfConnections > 0) {
            byte error = 0;
            byte[] messageBuffer = new byte[_bufferSize];
            Stream stream = new MemoryStream(messageBuffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, jsonObject);

            foreach (ClientInfo client in _clientSocketIDs) {
                NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, messageBuffer, _bufferSize, out error);
            }
        }
    }

    void CaptureFrame(string filePath) {

        Application.CaptureScreenshot(filePath);
        byte[] asByteArray = File.ReadAllBytes(filePath);

        //JSON testing
        ServerObject toBeSent = new ServerObject();
        toBeSent.time = Time.time;
        Texture2D textureToBeSent = new Texture2D(0, 0);
        textureToBeSent.LoadImage(asByteArray);
        toBeSent.texture2d = textureToBeSent;

        string jsonToBeSent = JsonUtility.ToJson(toBeSent);

        SendJSONMessage(jsonToBeSent);    
    }
}
