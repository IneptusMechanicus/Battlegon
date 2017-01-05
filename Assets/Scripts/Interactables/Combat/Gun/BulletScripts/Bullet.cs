using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    public float Speed;
    public float Range;
    Vector3 initPosition; 

    void Start()
    {
        initPosition = transform.position;     
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, initPosition) > Range)
        {
            Destroy(this.gameObject);
        }
    }
}
