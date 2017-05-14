using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Mono.Nat;

public class MyNetManager : NetworkManager
{

    [SerializeField]
    Canvas browser;

    [SerializeField]
    Canvas HUD;

    GameObject map;

//    string[] maps = {"PentagonMap", "HexagonMap", "OctagonMap"};

    public override void OnStartServer()
    {
        NatUtility.StartDiscovery();
		NatUtility.DeviceFound += DeviceFound;
        base.OnStartServer();
    }


    public override void OnStartClient(NetworkClient client)
    {

    	Debug.Log("Connected!!!!");
        browser.enabled = false;
        HUD.enabled = true;
        base.OnStartClient(client);
        //NetworkServer.Spawn(map);
    }

    public override void OnStopClient()
    {
        Debug.Log("GAME OVER!!!");
        HUD.enabled = false;
        browser.enabled = true;
        base.OnStopClient();
    }

    //Port forwarding procedure
    void DeviceFound(object sender, DeviceEventArgs args)
    {
		Debug.Log("Hello");
        INatDevice device = args.Device;
		if (device.GetSpecificMapping (Protocol.Udp, this.networkPort).PublicPort == -1) {
			Debug.Log ("Forwarding" + this.networkPort);
			device.CreatePortMap (new Mapping (Protocol.Udp, this.networkPort, this.networkPort));
		} else {
			Debug.Log ("Map is " + device.GetSpecificMapping (Protocol.Udp, this.networkPort).PublicPort);
		}
    }        
}