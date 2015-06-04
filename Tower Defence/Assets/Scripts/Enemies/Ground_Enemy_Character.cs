using UnityEngine;
using System.Collections;

public class Ground_Enemy_Character : CharacterEnemy_Base 

{
    private AI_Pather pather;
    private Rigidbody rigidBody;

    protected override void Start()
    {
        base.Start();
        pather = GetComponent<AI_Pather>();
        rigidBody = GetComponent<Rigidbody>();
    }
 
    protected override void ExplosionEffect()
    {
        if (rigidBody != null)
            rigidBody.velocity = Vector3.zero;

        var collider = GetComponents<Collider>();
        foreach(var col in collider)
        {
            col.enabled = false;
        }

        base.ExplosionEffect();
    }
}
