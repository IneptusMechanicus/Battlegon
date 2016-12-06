using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    protected float Speed;
    protected float Range;
    protected Vector3 initPosition;


    void SetBulletOrigin()
    {
        initPosition = transform.position;
    }

    void hitscanFly()
    {
        RaycastHit hit;
    }


    void ProjectileFly()
    {

        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, initPosition) > Range)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void Fly()
    {
        ProjectileFly();
    }

    void FixedUpdate()
    {
        Fly();
    }
}
