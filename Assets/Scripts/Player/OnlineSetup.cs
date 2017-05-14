using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 1, sendInterval = 0.1f)]
public class OnlineSetup : NetworkBehaviour
{     
	[SerializeField] 
	NetworkAnimator mainAnim;

	[SerializeField]
	NetworkAnimator legsAnim;

	[SerializeField]
	PlayerController controller;

	[SerializeField]
	PlayerStats stats;

	[SerializeField]
	GameObject vicPanel;

	[SerializeField]
	GameObject defPanel;

	NetworkStartPosition[] spawnPoints;
    void Start()
    {
        if (!isLocalPlayer)
        {
            transform.GetComponent<PlayerController>().CamSwitch(false);
            transform.GetComponent<PlayerController>().enabled = false;
            //transform.GetComponent<PlayerStats>().enabled = false; 
        }  
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        vicPanel = GameObject.Find("VictoryPanel");
        defPanel = GameObject.Find("DefeatPanel");
        defPanel.SetActive(false);
        vicPanel.SetActive(false);
    }

    void FixedUpdate()
    {
    	
    	if(stats.GetKills() == 10)
    	{
    		vicPanel.SetActive(true);
    		CmdWinGame();
    	}
    } 

    public void Respawn()
    {
    	Vector3 respawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
    	transform.position = respawnPoint;
    }

    [Command]
    void CmdWinGame()
    {
    	RpcDefeat();
    }        

    [ClientRpc]
    void RpcDefeat()
    {
    	defPanel.SetActive(true);
    }

    IEnumerator WaitFunc()
    {
    	yield return new WaitForSeconds(5);
    }
}
