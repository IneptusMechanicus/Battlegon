using UnityEngine;
using System.Collections;

public class AssaultRifleBullet : Bullet
{
	void Start ()
    {
        initPosition = transform.position;
        Speed = 70;
        Range = 20;
    }
}
