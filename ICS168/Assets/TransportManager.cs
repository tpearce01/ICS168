using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class TransportManager : Singleton<TransportManager> {


    private class ServerObject {
        public float time;
        public string imageFilePath;
    }

    public Image testImage;

    //[SerializeField] private int _bufferSize = 1024;
    [SerializeField]
    private int _bufferSize = 3000;
    [SerializeField] private int _maxConnections = 0;

    private int TCP_ChannelID = -1;
    private int UDP_ChannelID = -1;
    private int UDP_ChannelIDFrag = -1;

    private int _socketID = -1;
    [SerializeField] private int _socketPort = 8888;

    private int _connectionID = -1;

    // IN START, WE SETUP AND START THE SERVER
    private void Start() {
        /*
         * INITIALIZATION
         * When initializing the Network Transport Layer, you can choose between the default initialization, with no arguments, 
         * or you can provide parameters which control the overall behaviour of the network layer, such as the maximum packet size and the thread timout limit.
         */
        NetworkTransport.Init();

        /*
         * CONFIGURATION
         * The next step is configuration of connection between peers. You may want to define several communication channels, 
         * each with a different quality of service level specified to suit the specific types of messages that you want to send, 
         * and their relative importance within your game. We can define two communication channels with different quality of service values. 
         * “QosType.Reliable” will deliver message and assure that the message is delivered, while “QosType.Unreliable” will send message without any assurance, 
         * but will do this faster. It’s also possible to specify configuration settings specifically for each connection, 
         * by adjusting properties on the ConnectionConfig object. However, when making a connection from one client to another, 
         * the settings should be the same for both connected peers or the connection will fail with a CRCMismatch error.
         */
        ConnectionConfig connectionConfig = new ConnectionConfig();
        TCP_ChannelID = connectionConfig.AddChannel(QosType.Reliable);
        UDP_ChannelID = connectionConfig.AddChannel(QosType.Unreliable);
        UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.UnreliableFragmented);

        /*
         * TOPOLOGY
         * The final step of network configuration is topology definition. Network topology defines how many 
         * connections allowed and what connection configuration will used. Here we created topology which allow up to "_maxConnections" connections, 
         * each of them will configured by parameters defines in previous step.
         */
        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);

        /*
         * HOST CREATING
         * As all preliminary steps have done, we can create host (open socket). Here we add new host on port 8888 and any ip addresses. 
         * This host will support up to 10 connection, and each connection will have parameters as we defined in config object.
         */
        _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);

        Debug.Log("Socket Open. SocketID is: " + _socketID);

        // Test the connection.
        Connect();

        // Start a coroutine which waits for the connection to be established before sending any messages.
        //StartCoroutine(WaitToSendMessage());
    }

    // Use the Update function so we can continually check for incoming messages.
    private void Update() {
         test("ss.png");
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
                Debug.Log("incoming connection event received");
                break;

            /*
             * Data received. In this case incomingSocketID will define host, incomingConnectionID will define connection, 
             * incomingChannelID will define channel; dataSize will define size of the received data. 
             * If incomingMessageBuffer is big enough to contain data, data will be copied in the buffer. 
             * If not, error will contain MessageToLong error and you will need reallocate buffer and call this function again.
             * This is where a received message is handled and the game must do something based on the information.
             */
            case NetworkEventType.DataEvent:
                //Stream stream = new MemoryStream(incomingMessageBuffer);
                //BinaryFormatter formatter = new BinaryFormatter();
                //string message = formatter.Deserialize(stream) as string;
                //byte[] message = formatter.Deserialize(stream) as byte[];
                Debug.Log("Message received. Message size: " + incomingMessageBuffer.Length);
                //Debug.Log("Message received. Message contents: " + message);

                /*
                 * Testing Image Conversion && JSON HANDLING
                 */
                Texture2D testTex = new Texture2D(0, 0);

                //JSON PART
                // 1. Convert the byte stream back into a string
                Stream stream = new MemoryStream(incomingMessageBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string jsonMessage = formatter.Deserialize(stream) as string;

                // 2. Convert the string back into the original object (ServerObject)
                ServerObject incomingJsonData = JsonUtility.FromJson<ServerObject>(jsonMessage);

                // 3. Convert the imageFilePath to a byte array.
                byte[] imageByteArray = File.ReadAllBytes(incomingJsonData.imageFilePath);

                // Image Conversion Part
                // 3. Access the byte array from the original ServerObject to load the incoming screenshot.
                testTex.LoadImage(imageByteArray);
                testImage.sprite = Sprite.Create(testTex, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("remote client event disconnected");
                break;
        }
    }

    // CONNECT TO THE SERVER
    public void Connect() {

        /*
         * COMMUNICATION
         *  Send connect request to peer with ip “localhost” and port "_socketPort". It will return id assigned to this connection.
         *  We need to keep the connectionID available to all methos because we'll use it to send messages.
         *  
         *  NetworkTransport.Connect() connects to another server. Pass in the _socketID, the IP for the remote device, the _socketPOrt
         *  of the remote machine, 0 (default value, probably shouldn't change it), and a spot for error reporting.
         */
        byte error = 0;
        _connectionID = NetworkTransport.Connect(_socketID, "127.0.0.1", _socketPort, 0, out error);
        Debug.Log("Connect to server. ConnectionID: " + _connectionID);
    }

    // DISCONNECT FROM THE SERVER
    public void Disconnect() {
        byte error = 0;
        NetworkTransport.Disconnect(_socketID, _connectionID, out error);
    }

    public void SendMessage() {

        byte error = 0;
        byte[] messageBuffer = new byte[_bufferSize];
        Stream stream = new MemoryStream(messageBuffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, "");

        NetworkTransport.Send(_socketID, _connectionID, UDP_ChannelID, messageBuffer, _bufferSize, out error);
    }

    // SEND MESSAGE TO THE SERVER (UPDATED TO HANDLE JSON)
    public void SendJSONMessage(string jsonObject) {
        /*
         * TODO: WE SHOULD PROABABLY CHANGE THIS TO USE JSON OR SOMETHING COMPARABLE. STRING IS NOT EFFECIENT.
         */

        byte error = 0;
        byte[] messageBuffer = new byte[_bufferSize];
        Stream stream = new MemoryStream(messageBuffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, jsonObject);

        NetworkTransport.Send(_socketID, _connectionID, UDP_ChannelIDFrag, messageBuffer, _bufferSize, out error);
    }

        // SEND MESSAGE TO THE SERVER
    public void SendMessageLarge(byte[] message)
    {
        byte error = 0;
        Debug.Log("SendMessageLarge Message size: " + message.Length + ", " + _bufferSize);
        NetworkTransport.Send(_socketID, _connectionID, UDP_ChannelIDFrag, message, _bufferSize, out error);
    }

    private IEnumerator WaitToSendMessage() {

        yield return new WaitForSeconds(1.0f);

        // Send Test message.
        SendMessage();
    }

    void test(string filePath)
    {
        Application.CaptureScreenshot(filePath);
        //byte[] asByteArray = File.ReadAllBytes(filePath);

        //JSON testing
        ServerObject toBeSent = new ServerObject();
        toBeSent.time = Time.time;
        toBeSent.imageFilePath = filePath;
        string jsonToBeSent = JsonUtility.ToJson(toBeSent);


        //Send message

        SendJSONMessage(jsonToBeSent);

        //Separate client and server here
        //Debug.Log("Sending message. Byte Array Length: " + asByteArray.Length);
        //SendMessageLarge(asByteArray);

        //Receive message
        //Texture2D testTex = new Texture2D(0,0);
        //testTex.LoadImage(asByteArray);
        //testImage.sprite = Sprite.Create(testTex, new Rect(0,0,Screen.width, Screen.height), Vector2.zero);        
    }
}
