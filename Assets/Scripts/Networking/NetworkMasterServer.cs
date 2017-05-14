using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Mono.Nat;




public class Rooms
{
	public string name;
	public Dictionary<string, MasterMsgTypes.Room> rooms = new Dictionary<string, MasterMsgTypes.Room>();

	public bool AddHost(string gameName, string comment, string hostIp, int hostPort,int playerLimit, int connectionId)
	{
		if (rooms.ContainsKey(gameName))
		{
			return false;
		}

		MasterMsgTypes.Room room = new MasterMsgTypes.Room();
		room.name = gameName;
		room.comment = comment;
		room.hostIp = hostIp;
		room.hostPort = hostPort;
		room.playerLimit = playerLimit;
		room.connectionId = connectionId;
		rooms[gameName] = room;

		return true;
	}

	public MasterMsgTypes.Room[] GetRooms()
	{
		return rooms.Values.ToArray();
	}
}

public class NetworkMasterServer : MonoBehaviour
{
	int MasterServerPort = 20200;

	// map of gameTypeNames to rooms of that type
	Dictionary<string, Rooms> gameTypeRooms = new Dictionary<string, Rooms>();

	public void InitializeServer()
	{
		if (NetworkServer.active)
		{
			Debug.LogError("Already Initialized");
			return;
		}

		NetworkServer.Listen(MasterServerPort);

		// system msgs
		NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
		NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnect);
		NetworkServer.RegisterHandler(MsgType.Error, OnServerError);

		// application msgs
		NetworkServer.RegisterHandler(MasterMsgTypes.RegisterHostId, OnServerRegisterHost);
		NetworkServer.RegisterHandler(MasterMsgTypes.UnregisterHostId, OnServerUnregisterHost);
		NetworkServer.RegisterHandler(MasterMsgTypes.RequestListOfHostsId, OnServerListHosts);

		DontDestroyOnLoad(gameObject);
	}

	public void ResetServer()
	{
		NetworkServer.Shutdown();
	}

	Rooms EnsureRoomsForGameType(string gameTypeName)
	{
		if (gameTypeRooms.ContainsKey(gameTypeName))
		{
			return gameTypeRooms[gameTypeName];
		}

		Rooms newRooms = new Rooms();
		newRooms.name = gameTypeName;
		gameTypeRooms[gameTypeName] = newRooms;
		return newRooms;
	}

	// --------------- System Handlers -----------------

	void OnServerConnect(NetworkMessage netMsg)
	{
		Debug.Log("Master received client");
	}

	void OnServerDisconnect(NetworkMessage netMsg)
	{
		Debug.Log("Master lost client");

		// remove the associated host
		foreach (var rooms in gameTypeRooms.Values)
		{
			foreach (var room in rooms.rooms.Values)
			{
				if (room.connectionId == netMsg.conn.connectionId)
				{
					// tell other players?

					// remove room
					rooms.rooms.Remove(room.name);

					Debug.Log("Room ["+room.name+"] closed because host left");
					break;
				}
			}
		}

	}

	void OnServerError(NetworkMessage netMsg)
	{
		Debug.Log("ServerError from Master");
	}

	// --------------- Application Handlers -----------------

	void OnServerRegisterHost(NetworkMessage netMsg)
	{
		Debug.Log("OnServerRegisterHost");
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisterHostMessage>();
		var rooms = EnsureRoomsForGameType(msg.gameTypeName);

		int result = (int)MasterMsgTypes.NetworkMasterServerEvent.RegistrationSucceeded;
		if (!rooms.AddHost(msg.gameName, msg.comment, netMsg.conn.address, msg.hostPort, msg.playerLimit, netMsg.conn.connectionId))
		{
			result = (int)MasterMsgTypes.NetworkMasterServerEvent.RegistrationFailedGameName;
		}

		var response = new MasterMsgTypes.RegisteredHostMessage();
		response.resultCode = result;
		netMsg.conn.Send(MasterMsgTypes.RegisteredHostId, response);
	}



	void OnServerUnregisterHost(NetworkMessage netMsg)
	{
		Debug.Log("OnServerUnregisterHost");
		var msg = netMsg.ReadMessage<MasterMsgTypes.UnregisterHostMessage>();

		// find the room
		var rooms = EnsureRoomsForGameType(msg.gameTypeName);
		if (!rooms.rooms.ContainsKey(msg.gameName))
		{
			//error
			Debug.Log("OnServerUnregisterHost game not found: " + msg.gameName);
			return;
		}

		var room = rooms.rooms[msg.gameName];
		if (room.connectionId != netMsg.conn.connectionId)
		{
			//err
			Debug.Log("OnServerUnregisterHost connection mismatch:" + room.connectionId);
			return;
		}
		rooms.rooms.Remove(msg.gameName);

		// tell other players?

		var response = new MasterMsgTypes.RegisteredHostMessage();
		response.resultCode = (int)MasterMsgTypes.NetworkMasterServerEvent.UnregistrationSucceeded;
		netMsg.conn.Send(MasterMsgTypes.UnregisteredHostId, response);
	}

	void OnServerListHosts(NetworkMessage netMsg)
	{
		Debug.Log("OnServerListHosts");
		var msg = netMsg.ReadMessage<MasterMsgTypes.RequestHostListMessage>();
		if (!gameTypeRooms.ContainsKey(msg.gameTypeName))
		{
			var err = new MasterMsgTypes.ListOfHostsMessage();
			err.resultCode = -1;
			netMsg.conn.Send(MasterMsgTypes.ListOfHostsId, err);
			return;
		}

		var rooms = gameTypeRooms[msg.gameTypeName];
		var response = new MasterMsgTypes.ListOfHostsMessage();
		response.resultCode = 0;
		response.hosts = rooms.GetRooms();
		netMsg.conn.Send(MasterMsgTypes.ListOfHostsId, response);
	}

	//Discovers active networking device (router)
	// and Calls the DeviceFound method
	public void OpenPort()
	{
		NatUtility.StartDiscovery();
		NatUtility.DeviceFound += DeviceFound;
	}

	//Implements the port forwarding procedure
	void DeviceFound(object sender, DeviceEventArgs args)
	{
		INatDevice device = args.Device;
		if(device.GetSpecificMapping(Protocol.Udp, MasterServerPort).PublicPort == -1)
		{
			device.CreatePortMap(new Mapping(Protocol.Udp, MasterServerPort, MasterServerPort));
		}
	}

	void Start()
	{
		Application.runInBackground = true;
		OpenPort();
		InitializeServer();
		Debug.Log("Server running at" + MasterServerPort);
	}
}
