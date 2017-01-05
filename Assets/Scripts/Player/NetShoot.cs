using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetShoot : NetworkBehaviour {

    [SerializeField]
    GunController g;

    [Command]
    public void CmdNetProjectile()
    {
        if(g.ToSpawn != null)
        {
            GameObject projectile = Instantiate(g.ToSpawn, g.Origin.position, g.Origin.rotation) as GameObject;
            NetworkServer.Spawn(projectile);
        }
    }
}
