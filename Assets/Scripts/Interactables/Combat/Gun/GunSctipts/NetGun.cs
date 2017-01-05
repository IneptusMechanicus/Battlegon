using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetGun : NetworkBehaviour {

    [SerializeField]
    GunController GC;


	void Start ()
    {
	
	}
	
	void Update ()
    {
        if (GC.equiped)
        {
            this.GetComponent<NetworkTransform>().enabled = false;
        }
        else
        {
            this.GetComponent<NetworkTransform>().enabled = true;
        }
    }
}
