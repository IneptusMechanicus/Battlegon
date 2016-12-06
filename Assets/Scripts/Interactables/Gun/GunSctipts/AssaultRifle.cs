using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AssaultRifle : GunController
{
    public AssaultRifleBullet b;
    // Use this for initialization
    void Start ()
    {
        RateOfFire = 400;
        TimeBetweenShots = 60 / RateOfFire;   
	}

    void FixedUpdate()
    {
        Fire(IsFiring, b);
    }
}
