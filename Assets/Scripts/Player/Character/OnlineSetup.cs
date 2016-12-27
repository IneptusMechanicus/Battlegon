using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OnlineSetup : NetworkBehaviour
{
    void Start()
    {
        Debug.Log(isLocalPlayer);
        if (!isLocalPlayer)
        {
            transform.GetComponent<PlayerController>().PlayerCam.gameObject.SetActive(false);
            transform.GetComponent<PlayerController>().enabled = false;
        }  
    }       
}
