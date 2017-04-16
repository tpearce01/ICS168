using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TransportManager : Singleton<TransportManager> {

    [SerializeField] private int _maxConnections = 0;

    private int TCP_ChannelID = 0;
    private int UDP_ChannelID = 0;

    private int _socketID = 0;
    [SerializeField] private int _socketPort = 8888;

    private int _connectionID = 0;

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
    }

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
        _connectionID = NetworkTransport.Connect(_socketID, "localhost", _socketPort, 0, out error);
        Debug.Log("Connect to server. ConnectionID: " + _connectionID);
    }
}
