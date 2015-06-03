using UnityEngine;
using System.Collections;

public class MachineEnemy_Base : Enemy_Base
{
    public ParticleEmitter smokeTrail;
    public GameObject explosionEffect;

    protected override void HardDamageEffect()
    {
        smokeTrail.emit = true;
    }

    protected override void ExplosionEffect()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        base.ExplosionEffect();
    }

}
