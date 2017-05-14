using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


[NetworkSettings(channel = 2, sendInterval = 0.1f)]
public class Bullet : NetworkBehaviour
{
	//The bullet's parameters, set by it's GunController on instantiation
    [SerializeField]
	float Speed; 

    public float Range;
    public int Damage;
    Vector3 InitPosition;

    //The PlayerStats component of the person shooting. Used to score kills
    public PlayerStats player;

    void Start()
    {
        InitPosition = transform.position;     
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, InitPosition) > Range)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    public int GetBulletDamage()
    {
        return Damage;
    }

	public void SetPlayer(PlayerStats p)
	{
		player = p;
        Debug.Log("player should be seeeeet");
	}

    void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "Player")
        {
            PlayerStats p = c.gameObject.GetComponent<PlayerStats>();
            p.killer = player;
            p.ReduceHealth(Damage);
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
