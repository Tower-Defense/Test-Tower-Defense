using UnityEngine;
using System.Collections;

public class FireRay : MonoBehaviour
{
    public int myDamageAmount = 1;
    public Transform myTarget;

    void OnParticleCollision(GameObject collision)

    {
    //    if (collision.gameObject.tag == "Air Enemy" || collision.gameObject.tag == "Ground Enemy")
        {
            collision.transform.parent.gameObject.SendMessage("TakeDamage", myDamageAmount, SendMessageOptions.DontRequireReceiver);
    //        myTarget = collision.gameObject.transform;
        }

    }

    void Update()
    {
        if (myTarget)
        {
            transform.LookAt(myTarget);
        }
        else
        {
        //    Destroy(gameObject);
        }
    }
}
