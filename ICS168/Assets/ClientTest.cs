using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientTest : MonoBehaviour {

 //   [SerializeField] private string serverIP = "";      //Server IP address
 //   [SerializeField] private int _bufferSize = 3000;    //Maximum size of receiving buffer
 //   private int _maxConnections = 1;                    //Maximum umber of connection

 //   private int UDP_ChannelIDFrag = -1;                 //UDP communication channel for large messages
 //   [SerializeField] private int _connectionID = -1;                        //Connection ID

 //   [SerializeField] private int _socketID = -1;        //Socket ID
 //   [SerializeField] private int _socketPort = 8888;    //Port number

 //   // Use this for initialization
 //   void Start () {
 //       NetworkTransport.Init();
 //       ConnectionConfig connectionConfig = new ConnectionConfig();
 //       UDP_ChannelIDFrag = connectionConfig.AddChannel(QosType.ReliableFragmented);
 //       HostTopology hostTopology = new HostTopology(connectionConfig, _maxConnections);
 //       _socketID = NetworkTransport.AddHost(hostTopology, _socketPort);

 //       Application.runInBackground = true;
 //       Connect();
 //   }
	
	//// Update is called once per frame
	//void Update () {
		
	//}

 //   public void Connect() {

 //       byte error = 0;
 //       _connectionID = NetworkTransport.Connect(_socketID, serverIP, _socketPort, 0, out error);
 //   }
}
