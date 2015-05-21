using UnityEngine;
using System.Collections;

public class Projectile_Laser : Projectile_Base
{
    void Update()
    {
        // destroy
        if (myDist >= myRange && !myExplosion.GetComponent<ParticleSystem>().IsAlive(true))
        {
            Destroy(gameObject);
        }
        OnUpdate();
    }
}
