using UnityEngine;
using System.Collections;

public class Ground_Enemy_Character : CharacterEnemy_Base 

{
    private AI_Pather pather;

    protected override void Start()
    {
        base.Start();
        pather = GetComponent<AI_Pather>();
    }
 
    protected override void ExplosionEffect()
    {
        var velocity = GetComponent<Rigidbody>().velocity;
        if (velocity != null)
            velocity = Vector3.zero;
        var collider = GetComponents<Collider>();
        foreach(var col in collider)
        {
            col.enabled = false;
        }
        pather.isStop = true;
        base.ExplosionEffect();
    }
}
