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

public class ClientConnection : MonoBehaviour {

	private class ServerObject {
		public float 	time;
		public string 	image;
	}

	[SerializeField] private string serverIP = "";		//Server IP address
	[SerializeField] private int _bufferSize = 3000;	//Maximum size of receiving buffer
	[SerializeField] private int _maxConnections = 0;	//Maximum umber of connection

	private int UDP_ChannelIDFrag = -1;					//UDP communication channel for large messages
	private int _connectionID = -1;						//Connection ID

	[SerializeField] private int _socketID = -1;		//Socket ID
	[SerializeField] private int _socketPort = 8888;	//Port number

	public Image renderTo;								//Image to render to

	void Start() {
		NetworkTransport.Init();
		ConnectionConfig connectionConfig = new ConnectionConfig();
		UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);
		HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
		_socketID = NetworkTransport.AddHost(hostTopology, _socketPort);
		Connect();
	}

	void Update() {
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

		case NetworkEventType.DataEvent:
			Debug.Log("client: Message received. Message size: " + dataSize);
			Texture2D testTex = new Texture2D(0, 0);
			testTex.LoadImage(incomingMessageBuffer);
			renderTo.GetComponentInParent<CanvasRenderer>().SetTexture(testTex);
			break;

		case NetworkEventType.DisconnectEvent:
			Debug.Log("client: remote client event disconnected");
			break;
		}
	}

	public void Connect() {
		byte error = 0;
		_connectionID = NetworkTransport.Connect(_socketID, serverIP, _socketPort, 0, out error);
	}
}
