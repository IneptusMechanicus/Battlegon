using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float speed;
    public float range;                
    Vector3 initPosition;


    void Start ()
    {
        initPosition = transform.position;                                   
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, initPosition) > range)
        {
            Destroy(this.gameObject);
        } 
	}
}
