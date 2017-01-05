using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class GunController : CombatItem
{                                          
    [SerializeField]
    public Transform Origin;
    [SerializeField]
    GameObject Bullet;   
    public GameObject ToSpawn;              

    float ROFCountDown;
    float TimeBetweenShots;
    public float RateOfFire;
    bool SingleShot;
    bool IsFiring;          

    void Start()
    {
        TimeBetweenShots = 60 / RateOfFire;
    }

    void Update()
    {
        ToSpawn = Fire();                                
    }

    public GameObject Fire()
    {
        ROFCountDown -= Time.deltaTime;
        if (IsFiring)
        { 
            if (ROFCountDown <= 0)
            {
                ROFCountDown = TimeBetweenShots;
                return Bullet; 
            }
            return null;
        }
        return null;
    }

    public override void SpawnProjectile()
    {
        if(ToSpawn != null)
        {

        }
    }

    [Command]
    public void CmdNetProjectile()
    {
        if (ToSpawn != null)
        {
            GameObject projectile = Instantiate(ToSpawn, Origin.position, Origin.rotation) as GameObject;
            NetworkServer.Spawn(projectile);
        }
    }

    public override void Action1(bool on)
    {
        IsFiring = on;
        CmdNetProjectile();     
    }

    public override void Action2(bool on)
    {

    }
}
