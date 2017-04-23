using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class ClientConnection : MonoBehaviour {

    private class ServerObject {
        public float time;
        public byte[] image;
    }

    [SerializeField] private string serverIP = "";
    [SerializeField]
    private int _bufferSize = 3000;
    [SerializeField] private int _maxConnections = 0;

    //private int UDP_ChannelID = -1;
    private int UDP_ChannelIDFrag = -1;

    [SerializeField] private int _socketID = -1;
    [SerializeField] private int _socketPort = 8888;

    private int _connectionID = -1;

    public Image testImage;

    // Use this for initialization
    void Start() {

        Debug.Log(System.Net.IPAddress.Loopback);

        NetworkTransport.Init();
        ConnectionConfig connectionConfig = new ConnectionConfig();
        //UDP_ChannelID = connectionConfig.AddChannel(QosType.Unreliable);
        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.UnreliableFragmented);

        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);

        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);

        Connect();
    }

    // Update is called once per frame
    void Update() {
        /*
         * COMMUNICATION
         * For checking host status you can use two functions:
         * NetworkTransport.Receive() or NetworkTransport.ReceiveFromHost
         * Both of them returns event, first function will return events from any host (and return host id via recHostId) 
         * the second form check host with id recHostId.
         */
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

            /*
             * Connection event come in. It can be new connection, or it can be response on previous connect command:
             */
            case NetworkEventType.ConnectEvent:
                Debug.Log("client incoming connection event received");
                break;

            /*
             * Data received. In this case incomingSocketID will define host, incomingConnectionID will define connection, 
             * incomingChannelID will define channel; dataSize will define size of the received data. 
             * If incomingMessageBuffer is big enough to contain data, data will be copied in the buffer. 
             * If not, error will contain MessageToLong error and you will need reallocate buffer and call this function again.
             * This is where a received message is handled and the game must do something based on the information.
             */
            case NetworkEventType.DataEvent:

                Debug.Log("client: Message received. Message size: " + incomingMessageBuffer.Length);

                Texture2D testTex = new Texture2D(0, 0);
                //Stream stream = new MemoryStream(incomingMessageBuffer);
                //BinaryFormatter formatter = new BinaryFormatter();

                //string jsonMessage = formatter.Deserialize(stream) as string;
                //ServerObject incomingJsonData = JsonUtility.FromJson<ServerObject>(jsonMessage);

                //testTex.LoadImage(incomingJsonData.image);
                testTex.LoadImage(incomingMessageBuffer);

                testImage.sprite = Sprite.Create(testTex, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("client: remote client event disconnected");
                break;
        }
    }

    public void Connect() {
        byte error = 0;
        _connectionID = NetworkTransport.Connect(_socketID, serverIP, _socketPort, 0, out error);
        //Debug.Log("Connect to server. ConnectionID: " + _connectionID);

        StartCoroutine(delay());
    }

    private IEnumerator delay() {
        yield return new WaitForSeconds(2.0f);

        byte error = 0;
        byte[] messageBuffer = new byte[_bufferSize];
        Stream stream = new MemoryStream(messageBuffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, "hello");
        NetworkTransport.Send(_socketID, _connectionID, UDP_ChannelIDFrag, messageBuffer, _bufferSize, out error);
    }
}
