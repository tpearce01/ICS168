using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerMiddleMan : Singleton<ServerMiddleMan> {

    [SerializeField] private int _bufferSize = 3000;    //Maximum size of receiving buffer
    private int _maxConnections = 4;                    //Maximum umber of connection

    private int TCP_ChannelID = -1;                 //UDP communication channel for large messages
    HostTopology hostTopology;

    [Space]
    [Header("Connection IDs")]
    [SerializeField] private int MS_connectionID = -1;

    [Space]
    [Header("Port Numbers")]
    [SerializeField] private int MS_socketPort = 8888;
    [SerializeField] private int GS_socketPort = -1;
    [SerializeField] private int Outgoing_socketPort = -1;

    [Space]
    [Header("Socket IDs")]
    [SerializeField] private int MS_socketID = -1;


    // Update is called once per frame
    void Update() {
        // Incoming Connection IDs
        int incomingSocketID = -1;
        int incomingConnectionID = -1;
        int incomingChannelID = -1;
        byte[] incomingMessageBuffer = new byte[1024];
        int dataSize = 0;
        byte error;

        NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
            out incomingChannelID, incomingMessageBuffer, 1024, out dataSize, out error);

        switch (incomingNetworkEvent) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log(incomingConnectionID);
                if (incomingConnectionID == 1) {
                    // Send the port number of this game instance to the Master Server
                    string jsonToBeSent = "2";
                    jsonToBeSent += JsonUtility.ToJson(new PortID(GS_socketPort));
                    byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
                    NetworkTransport.Send(MS_socketID, MS_connectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
                }
                //if (incomingSocketID == 0) {
                //    Debug.Log("MiddleWare: Game Server Connected.");
                //}
                //else if (incomingSocketID == 1) {
                //    Debug.Log("MiddleWare: Master Server Connected.");

                //    // Send the port number of this game instance to the Master Server
                //    string jsonToBeSent = "2";
                //    PortID portID = new PortID(GS_socketPort);
                //    jsonToBeSent += JsonUtility.ToJson(portID);
                //    byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonToBeSent);
                //    NetworkTransport.Send(MS_socketID, MS_connectionID, TCP_ChannelID, messageBuffer, messageBuffer.Length, out error);
                //}
                break;

            case NetworkEventType.DataEvent:
                Debug.Log("server test: dataevent");
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("server test: disconnect");
                break;
        }
    }

    public void StartMiddleMan(int portNumber) {

        NetworkTransport.Init();

        GS_socketPort = portNumber;
        Outgoing_socketPort = GS_socketPort - 1110;
        Connect();
    }

    private void Connect() {

        ConnectionConfig connectionConfig = new ConnectionConfig();
        TCP_ChannelID = connectionConfig.AddChannel(QosType.Reliable);
        hostTopology = new HostTopology(connectionConfig, _maxConnections);
        //GS_socketID = NetworkTransport.AddHost(hostTopology, To_GS_SocketPort);
        MS_socketID = NetworkTransport.AddHost(hostTopology, Outgoing_socketPort);

        byte error = 0;
        //GS_connectionID = NetworkTransport.Connect(GS_socketID, "127.0.0.1", GS_socketPort, 0, out error);
        MS_connectionID = NetworkTransport.Connect(MS_socketID, "127.0.0.1", MS_socketPort, 0, out error);
    }
}
