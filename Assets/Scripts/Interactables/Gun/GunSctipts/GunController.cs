using UnityEngine;
using System.Collections;
using System;

public class GunController : CombatItem
{                                          
    

    
    [SerializeField]
    Transform Origin;
    [SerializeField]
    Bullet b;

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
        Fire(IsFiring);
    }

    public void Fire(bool firing)
    {
        ROFCountDown -= Time.deltaTime;
        if (IsFiring)
        { 
            if (ROFCountDown <= 0)
            {
                ROFCountDown = TimeBetweenShots;
                Bullet newBullet = Instantiate(b, Origin.position, Origin.rotation) as Bullet;   
            }
        }          
    }

    public override void Action1(bool on)
    {
        IsFiring = on;
    }

    public override void Action2(bool on)
    {

    }
}
