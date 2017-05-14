using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Mono.Nat;
using UnityEngine.UI;
using System.Net;
using System;

public class NetworkMasterClient : MonoBehaviour
{

	//This code is based on the MasterServer sample code, freely distributed by Unity on it's forums
	//Modifications have been made to integrate the master server topology into the game
	
	public bool dedicatedServer;
	public string MasterServerIpAddress;
	public int MasterServerPort;
	public int updateRate;
	public string gameTypeName;
	public string gameName;
	public int gamePort;

	[SerializeField]
	RectTransform ContentWindow;
	[SerializeField]
	Button ServerButton;
	[SerializeField]
	InputField PortNumber;
	[SerializeField]
	InputField PlayerLimit;
	[SerializeField]
	InputField ServerName;
	[SerializeField]
	InputField ExtraInfo;
	[SerializeField]
	Dropdown MapSelect;

	string HostGameType = "";
	string HostGameName = "";

	string comment;

	

	MasterMsgTypes.Room[] hosts = null;

	public NetworkClient client = null;

	static NetworkMasterClient singleton;


	void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	public void InitializeClient()
	{
		if (client != null)
		{
			Debug.LogError("Already connected");
			return;
		}
		Debug.Log ("Attempting to connect");
		client = new NetworkClient();
		client.Connect(MasterServerIpAddress, MasterServerPort);

		// system msgs
		client.RegisterHandler(MsgType.Connect, OnClientConnect);
		client.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
		client.RegisterHandler(MsgType.Error, OnClientError);

		// application msgs
		client.RegisterHandler(MasterMsgTypes.RegisteredHostId, OnRegisteredHost);
		client.RegisterHandler(MasterMsgTypes.UnregisteredHostId, OnUnregisteredHost);
		client.RegisterHandler(MasterMsgTypes.ListOfHostsId, OnListOfHosts);

		DontDestroyOnLoad(gameObject);
	}

	public void ResetClient()
	{
		if (client == null)
			return;

		client.Disconnect();
		client = null;
		hosts = null;
	}

	public bool isConnected
	{
		get
		{
			if (client == null) 
				return false;
			else 
				return client.isConnected;
		}
	}

	// --------------- System Handlers -----------------

	void OnClientConnect(NetworkMessage netMsg)
	{
		Debug.Log("Client Connected to Master");
	}

	void OnClientDisconnect(NetworkMessage netMsg)
	{
		Debug.Log("Client Disconnected from Master");
		ResetClient();
		OnFailedToConnectToMasterServer();
	}

	void OnClientError(NetworkMessage netMsg)
	{
		Debug.Log("ClientError from Master");
		OnFailedToConnectToMasterServer();
	}

	// --------------- Application Handlers -----------------

	void OnRegisteredHost(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisteredHostMessage>();
		OnServerEvent((MasterMsgTypes.NetworkMasterServerEvent)msg.resultCode);
	}

	void OnUnregisteredHost(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisteredHostMessage>();
		OnServerEvent((MasterMsgTypes.NetworkMasterServerEvent)msg.resultCode);
	}

	void OnListOfHosts(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MasterMsgTypes.ListOfHostsMessage>();
		hosts = msg.hosts;
		OnServerEvent(MasterMsgTypes.NetworkMasterServerEvent.HostListReceived);
	}

	public void ClearHostList()
	{
		if (!isConnected)
		{
			Debug.LogError("ClearHostList not connected");
			return;
		}
		hosts = null;

	}

	public MasterMsgTypes.Room[] PollHostList()
	{
		if (!isConnected)
		{
			Debug.LogError("PollHostList not connected");
			return null;
		}
		return hosts;
	}

	public void RegisterHost(string gameTypeName, string gameName, string comment, bool passwordProtected, int playerLimit, int port)
	{
		if (!isConnected)
		{
			Debug.LogError("RegisterHost not connected");
			return;
		}

		var msg = new MasterMsgTypes.RegisterHostMessage();
		msg.gameTypeName = gameTypeName;
		msg.gameName = gameName;
		msg.comment = comment;
		msg.passwordProtected = passwordProtected;
		msg.playerLimit = playerLimit;
		msg.hostPort = port;
		client.Send(MasterMsgTypes.RegisterHostId, msg);

		HostGameType = gameTypeName;
		HostGameName = gameName;
	}

	public void RequestHostList(string gameTypeName)
	{
		if (!isConnected)
		{
			Debug.LogError("RequestHostList not connected");
			return;
		}

		var msg = new MasterMsgTypes.RequestHostListMessage();
		msg.gameTypeName = gameTypeName;
		client.Send(MasterMsgTypes.RequestListOfHostsId, msg);
	}

	public void UnregisterHost()
	{
		if (!isConnected)
		{
			Debug.LogError("UnregisterHost not connected");
			return;
		}

		var msg = new MasterMsgTypes.UnregisterHostMessage();
		msg.gameTypeName = HostGameType;
		msg.gameName = HostGameName;
		client.Send(MasterMsgTypes.UnregisterHostId, msg);
		HostGameType = "";
		HostGameName = "";

		Debug.Log("send UnregisterHost");
	}

	public virtual void OnFailedToConnectToMasterServer()
	{
		Debug.Log("OnFailedToConnectToMasterServer");
	}

	public virtual void OnServerEvent(MasterMsgTypes.NetworkMasterServerEvent evt)
	{
		Debug.Log("OnServerEvent " + evt);

		if (evt == MasterMsgTypes.NetworkMasterServerEvent.HostListReceived)
		{
			foreach (var h in hosts)
			{
				Debug.Log("Host:" + h.name + "addr:" + h.hostIp + ":" + h.hostPort);
			}
		}

		if (evt == MasterMsgTypes.NetworkMasterServerEvent.RegistrationSucceeded)
		{
			if (NetworkManager.singleton != null)
			{
				NetworkManager.singleton.StartServer();
				Debug.Log(NetworkManager.singleton.maxConnections);
			}
		}

		if (evt == MasterMsgTypes.NetworkMasterServerEvent.UnregistrationSucceeded)
		{
			if (NetworkManager.singleton != null)
			{
				NetworkManager.singleton.StopServer();
			}
		}
	}


	//Lists active servers on the server browser
	//Reconnects client to master if disconnected
	void ListHostsOnUI()
	{
		if(!isConnected)
		{
			InitializeClient();
		}
		RequestHostList("BattlegonGame");
		Invoke("GenerateList", 2);		
	}

	//Generates list of Server buttons on the server browser ScrollView
	void GenerateList()
	{
		MasterMsgTypes.Room[] servers = PollHostList();
		Debug.Log(hosts);

		foreach(Transform child in ContentWindow) 
		{
  			Destroy(child.gameObject);
		}

		if(servers != null)
		{
			Debug.Log("helllllllllo");
			int ListPos = 20;
			RectTransform cont = ContentWindow.GetComponent<RectTransform>();
			foreach(var h in servers)
			{
				ListPos -= 40;
				Button b = Instantiate(ServerButton, cont.transform) as Button;

				b.transform.GetChild(3).GetComponent<Text>().text = h.name;
				b.transform.GetChild(4).GetComponent<Text>().text = h.comment;
				Debug.Log(h.playerLimit);
				b.transform.GetChild(5).GetComponent<Text>().text = h.playerLimit.ToString();
				b.onClick.AddListener(delegate{ConnectToServer(h.hostIp, h.hostPort, h.playerLimit);});

				
				ContentWindow.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(
				ContentWindow.GetComponent<RectTransform>().sizeDelta.x, 
				ContentWindow.GetComponent<RectTransform>().sizeDelta.y + 20);
				
				b.GetComponent<RectTransform>().transform.SetParent(ContentWindow.transform);
				b.GetComponent<RectTransform>().transform.localPosition = new Vector3(
				b.GetComponent<RectTransform>().transform.localPosition.x,
				ListPos,
				b.GetComponent<RectTransform>().transform.localPosition.z);
			}
		}
		else
		{
			Debug.Log("KYS");
		}
	}

	//Sets up the network manager client and connects it to a game server
	void ConnectToServer(string ip, int port, int playerLimit)
	{
		NetworkManager.singleton.networkAddress = ip;
		NetworkManager.singleton.networkPort = port;
		NetworkManager.singleton.maxConnections = playerLimit;
		NetworkManager.singleton.StartClient();
		Debug.Log("Connecting to:" + ip + " at " + port);

	}

	//Start's a game server
	public void InitGameServer()
	{
		if(!isConnected)
		{
			InitializeClient();
		}
		Invoke("StartGameServer", 2);
	}


	//Takes input from the game server setup menu
	//Starts game server and registers it with the Master Server
	public void StartGameServer()
	{
		Debug.Log("Port: " + PortNumber.transform.GetChild(2).GetComponent<Text>().text);
		gamePort = Int32.Parse(PortNumber.transform.GetChild(2).GetComponent<Text>().text);
		int playerLimit = Int32.Parse(PlayerLimit.transform.GetChild(2).GetComponent<Text>().text);
		Debug.Log(playerLimit);
		comment = ExtraInfo.transform.GetChild(2).GetComponent<Text>().text;
		gameName = ServerName.transform.GetChild(2).GetComponent<Text>().text;

		NetworkManager.singleton.networkPort = gamePort;
		NetworkManager.singleton.maxConnections = playerLimit;
		RegisterHost("BattlegonGame", gameName, comment, false, playerLimit, gamePort);
		
	}

	void Start()
	{
		InitializeClient();
	}
}
