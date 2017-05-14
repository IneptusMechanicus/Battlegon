using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class GunController : CombatItem
{   
	//The position where the bullet will be instantiated
	[SerializeField]
	Transform Origin;

	//The bullet prefab, which this gun fires
   	[SerializeField]
    GameObject Bullet; 
    
    //The gun's rate of fire in Rounds per Minute
    [SerializeField]
    float RateOfFire;

	//The gun's range	
	[SerializeField]
	int Range;
	
	//The damage each bullet inflicts on the player
	// 1 damage = 1 health
	[SerializeField]
	int DamagePerRound;
	
	//Reference to the current map for reparenting
	[SerializeField]
	GameObject Map;

	//Controll parameters for the gun firing
    float ROFCountDown;
    float TimeBetweenShots;
    bool SingleShot;
    bool IsFiring;
    bool CanFire;

    void Start()
    {
        TimeBetweenShots = 60 / RateOfFire;
        Bullet.GetComponent<Bullet> ().Range = Range;
        Bullet.GetComponent<Bullet> ().Damage = DamagePerRound;
    }

    void FixedUpdate()
    {
		AutoFire();
    }

    public void EquipSetup()
    {
    	if (equiped)
		{
			Bullet.GetComponent<Bullet> ().SetPlayer (player);
			GetComponent<Collider> ().enabled = false;
		}
		else
		{
			player = null;
			GetComponent<Collider> ().enabled = true;
			Bullet.GetComponent<Bullet>().SetPlayer(null);
			transform.parent = Map.transform;
		}
    }

    //Implements automatic weapon fire 
    //using two booleans to control bullet instantiation
	void AutoFire()
	{

		ROFCountDown -= Time.deltaTime;
		if (IsFiring)
		{
			if (ROFCountDown <= 0)
			{    
				CanFire = true;
				ROFCountDown = TimeBetweenShots; 
			}
			else
			{
				CanFire = false;
			}
		}
	}

	//Instantiates bullet and spawns it on the network
    protected override void Fire()
    {                          
        if (IsFiring && CanFire)
        {                             
			GameObject projectile = Instantiate(Bullet, Origin.position, Origin.rotation);
            NetworkServer.Spawn(projectile);  
        }
        Debug.Log("Gun Fired");
    }              

    public override void Action1(bool on)
    {
        IsFiring = on;    
        Debug.Log("Gun Fired in Action1");         
        Fire();          
    }

    public override void Action2(bool on)
    {

    }

    //Sets the Gun's parent back to the current Map
    public void Drop()
    {
    	this.transform.parent = Map.transform;
    }
}
