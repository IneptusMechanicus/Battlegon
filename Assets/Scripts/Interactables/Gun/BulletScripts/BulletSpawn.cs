using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BulletSpawn : NetworkBehaviour
{
    [SerializeField]
    Bullet b;

    void Start()
    {
        NetworkServer.Spawn(b.gameObject);
        Debug.Log("should spawn");
    }

    void OnDestroy()
    {
        NetworkServer.UnSpawn(b.gameObject);
    }
}
