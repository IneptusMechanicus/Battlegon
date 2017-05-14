using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[NetworkSettings(channel = 3, sendInterval = 0.1f)]
public class PlayerStats : NetworkBehaviour {

    [SerializeField]
    int health = 100;
    [SerializeField]
    int kills = 0;
    [SerializeField]
    int deaths = 0;

    [SerializeField]
    GameObject HUD;

    OnlineSetup setup;
    public PlayerStats killer;
	// Use this for initialization
	void Start ()
    {
        if(isLocalPlayer)
        {
            setup = GetComponent<OnlineSetup>();	
            HUD = Instantiate(HUD, GameObject.Find("Canvas").transform) as GameObject;
            HUD.GetComponent<Canvas>().enabled = true;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    HUD.transform.GetChild(0).GetComponent<Text>().text = health.ToString();
        HUD.transform.GetChild(1).GetComponent<Text>().text = kills.ToString();
        HUD.transform.GetChild(2).GetComponent<Text>().text = deaths.ToString();
        if(health <= 0)
        {
            //killer.AddKill();
            AddDeath();
            StartCoroutine(Wait());
        }
	}

    IEnumerator Wait()
    {
        print(Time.time);
        yield return new WaitForSeconds(1);
        print(Time.time);
    }

    public void AddKill()
    {
        kills++;
    }

    public void AddDeath()
    {
        setup.Respawn();
        health = 100;
        deaths++;
    }

    public int ReduceHealth(int Damage)
    {
        health -= Damage;
        return health;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetKills()
    {
        return kills;
    }

    void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Bullet")
        {
            Debug.Log(c.gameObject.GetComponent<Bullet>().player);
            killer = c.gameObject.GetComponent<Bullet>().player;
            ReduceHealth(c.gameObject.GetComponent<Bullet>().GetBulletDamage());
            NetworkServer.Destroy(c.gameObject);
        }
    }
}
