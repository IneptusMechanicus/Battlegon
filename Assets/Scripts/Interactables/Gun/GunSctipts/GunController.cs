using UnityEngine;
using System.Collections;
using System;

public class GunController : CombatItem
{                                          
    

    
    [SerializeField]
    protected Transform Origin;

    protected float ROFCountDown;
    protected float TimeBetweenShots;
    protected float RateOfFire;

    protected bool SingleShot;
    protected bool IsFiring;
    protected bool IsHitscanGun;
    
    
    public void Fire(bool firing, Bullet bullet)
    {
        ROFCountDown -= Time.deltaTime;
        if (IsFiring)
        { 
            if (ROFCountDown <= 0)
            {
                ROFCountDown = TimeBetweenShots;
                Bullet newBullet = Instantiate(bullet, Origin.position, Origin.rotation) as Bullet;   
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
