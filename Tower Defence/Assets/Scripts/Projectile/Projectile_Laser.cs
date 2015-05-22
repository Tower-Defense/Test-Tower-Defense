using UnityEngine;
using System.Collections;

public class Projectile_Laser : Projectile_Base
{
    private ParticleSystem crashEffect;

    void Awake()
    {
        crashEffect = myExplosion.GetComponent<ParticleSystem>();
    }
    void Update()
    {
        // destroy
        if (myDist >= myRange && !crashEffect.IsAlive(true))
        {
            Destroy(gameObject);
        }
        OnUpdate();
    }
}
