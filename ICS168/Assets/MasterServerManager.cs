using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MasterServerManager : Singleton<MasterServerManager> {

    [Header("Server Settings")]
    [SerializeField] private int _incomingBufferSize = 3000;    // max buffer size
    [SerializeField] private int _socketPort = 8888;
    [SerializeField] private int _maxGameInstances = 0;

    private int UDP_ChannelID = -1;                         // This channel should be reserved for larger messages
    private int _socketID = -1;
    private int _connectionID = -1;

    [SerializeField] private int _maxConnections = 0;

    [Space]
    [Header("Server Stats")]
    [SerializeField] private int _numberOfConnections = 0;

    // Use this for initialization
    void Start () {
        NetworkTransport.Init();

        ConnectionConfig connectionConfig = new ConnectionConfig();
        UDP_ChannelID = connectionConfig.AddChannel(QosType.ReliableFragmented);

        HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);

        Application.runInBackground = true;
    }
	
	// Update is called once per frame
	void Update () {

        // Incoming Connection IDs
        int incomingSocketID = -1;
        int incomingConnectionID = -1;
        int incomingChannelID = -1;
        byte[] incomingMessageBuffer = new byte[_incomingBufferSize];
        int dataSize = 0;
        byte error;

        NetworkEventType incomingNetworkEvent = NetworkTransport.Receive(out incomingSocketID, out incomingConnectionID,
            out incomingChannelID, incomingMessageBuffer, _incomingBufferSize, out dataSize, out error);

        switch (incomingNetworkEvent) {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                ++_numberOfConnections;
                break;

            case NetworkEventType.DataEvent:
                Debug.Log("hello, world!");
                break;

            case NetworkEventType.DisconnectEvent:
                _numberOfConnections = --_numberOfConnections < 0 ? 0 : _numberOfConnections;
                break;
        }
    }
}
