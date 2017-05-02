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

public class ServerConnection : MonoBehaviour
{
    [SerializeField] private RenderTexture rt;    //Target render texture
    [SerializeField] private Camera _cam;          //Camera to render from

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
                break;

            case NetworkEventType.DataEvent:
                Debug.Log("server: Message received. Message size: " + incomingMessageBuffer.Length);
                //Stream stream = new MemoryStream(incomingMessageBuffer);
                //BinaryFormatter formatter = new BinaryFormatter();
                //string message = formatter.Deserialize(stream) as string;
                //Test Code
                string message = Encoding.UTF8.GetString(incomingMessageBuffer);
                //End Test Code

                PlayerIO input = JsonUtility.FromJson<PlayerIO>(message);
                Debug.Log(incomingConnectionID);
                GameManager.Instance.PlayerActions(incomingConnectionID, input);

                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("server: remote client event disconnected");
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
        string jsonToBeSent = JsonUtility.ToJson(toBeSent);

        if (_numberOfConnections > 0) {
            SendJSONMessage(jsonToBeSent);
        }

        //byte error;
        //foreach (ClientInfo client in _clientSocketIDs)
        //{
        //    NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, image, image.Length, out error);
        //}
    }

}
