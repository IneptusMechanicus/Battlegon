using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CombatItem : MonoBehaviour
{
    protected bool equiped = false;
	public PlayerStats player;
    public virtual void Action1(bool on){}
    public virtual void Action2(bool on){}      
    protected virtual void Fire(){} 
    
    public bool GetEquiped()
    {
        return equiped;
    }                  
    
    public void SetEquiped(bool e)
    {
        equiped = e;
    }           
}
