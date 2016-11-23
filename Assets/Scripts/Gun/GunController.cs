using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    public bool isFiring;
    public Bullet b;
    public float speed;

    public float rateOfFire;
    public bool singleShot;
    private float ROFCountDown;  

    public Transform origin;

    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (isFiring)
        {
            ROFCountDown -= Time.deltaTime;
            if (ROFCountDown <= 0)
            {
                ROFCountDown = rateOfFire;
                Bullet newBullet = Instantiate(b, origin.position, origin.rotation) as Bullet;
                newBullet.speed = speed;
            }
        }
        else ROFCountDown = 0;
	}
}
