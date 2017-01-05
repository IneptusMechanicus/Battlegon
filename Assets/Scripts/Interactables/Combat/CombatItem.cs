using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CombatItem : NetworkBehaviour
{
    public bool equiped = false;

    public virtual void Action1(bool on){}
    public virtual void Action2(bool on){}  
    public virtual void SpawnProjectile(){} 
}
