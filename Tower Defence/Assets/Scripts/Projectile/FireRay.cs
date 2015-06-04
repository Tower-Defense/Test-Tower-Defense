using UnityEngine;
using System.Collections;

public class FireRay : MonoBehaviour
{
    public int myDamageAmount = 1;
    public Transform myTarget;
    private ParticleSystem myParticleSystem;

    void Start()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject collision)

    {

         collision
             .transform
             .parent
             .gameObject
             .SendMessage("TakeDamage", myDamageAmount, SendMessageOptions.DontRequireReceiver);


    }
    public void StartFire( Transform target_)
    {
        myTarget = target_;
        myParticleSystem.Play(true);
    }

    void Update()
    {
        if (myTarget == null)
        {
            myParticleSystem.Stop(true);
        }
    }
}
