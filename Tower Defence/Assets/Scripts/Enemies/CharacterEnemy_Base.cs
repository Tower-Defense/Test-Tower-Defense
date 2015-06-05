using UnityEngine;
using System.Collections;

public class CharacterEnemy_Base : Enemy_Base
{
    protected Animation myAnimation;

    protected virtual void Start()
    {
        myAnimation = GetComponent<Animation>();
    }

    protected override void DamageEffect()
    {
        myAnimation.PlayQueued("Damage", QueueMode.PlayNow);
        myAnimation.PlayQueued("Run", QueueMode.CompleteOthers);
    }

    protected override void ExplosionEffect()
    {
        myAnimation.Stop();
        StartCoroutine(myAnimation.WhilePlaying("Dead", () => { Destroy(gameObject); }));            
    }


}
