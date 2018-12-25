using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    
    public string clientName;
    public bool isHost;
    public string clientNumber;
    public string clientImage;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }

    // Sending messaged to the server
    public void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    // Read messages from the server
    private void OnIncomingData(string data)
    {
        Debug.Log("Client:" + data);
        
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i+=3)
                {
                    UserConnected(aData[i], false,aData[i+1], aData[i+2]);
                }
                Send("CWHO|" + clientName + "|" + ((isHost)?1:0).ToString() + "|" + clientNumber + "|" + clientImage);
                break;
                
            case "SCNN":
                UserConnected(aData[1], false , aData[2] , aData[3]);
                break;

        }
        
    }

    private void UserConnected(string name,bool host,string number,string image)
    {
        
        GameClient c = new GameClient();
        c.name = name;
        c.isHost = host;
        c.number = number;
        c.image = image;

        players.Add(c);

        LobbyPlayer newPlayer = ScriptableObject.CreateInstance<LobbyPlayer>();
        newPlayer.name = name;
        newPlayer.image = Resources.Load<Sprite>("players/" + image);
        newPlayer.guess = number;
        newPlayer.AI = false;
        LobbyManager.LM.CreatePlayer(newPlayer);
        }


    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
    public string number;
    public string image;
    //TODO add all player info
}