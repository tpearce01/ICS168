﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using System.Text;

public class ServerConnection : MonoBehaviour
{
    public RenderTexture rt;    //Target render texture
    public Camera cam;          //Camera to render from

    private class ServerObject {
        public float time;
        public byte[] image;
    }

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
                Stream stream = new MemoryStream(incomingMessageBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                Debug.Log(message);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("server: remote client event disconnected");
                break;
        }
    }

    public void SendJSONMessage(string image) {
        if (_numberOfConnections > 0) {
            byte error = 0;
            byte[] messageBuffer = new byte[_bufferSize];
            Stream stream = new MemoryStream(messageBuffer);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, image);

            foreach (ClientInfo client in _clientSocketIDs) {
                NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, messageBuffer, _bufferSize, out error);
                Debug.Log("Message Sent");
            }
        }
    }

    void CaptureFrame() {
        RenderTexture.active = rt;
        Camera.main.Render();
        Texture2D tex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0,0,cam.targetTexture.width, cam.targetTexture.height), 0,0);
        tex.Apply();
        byte[] image = tex.EncodeToPNG();
        Debug.Log("Message length: " + image.Length);
        //SendJSONMessage(Convert.ToBase64String(image));
        byte error;
        foreach (ClientInfo client in _clientSocketIDs)
        {
            NetworkTransport.Send(client.socketID, client.ConnectionID, client.ChannelID, image, image.Length, out error);
        }
    }

}
