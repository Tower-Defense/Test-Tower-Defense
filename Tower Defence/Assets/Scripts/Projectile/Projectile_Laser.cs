using UnityEngine;
using System.Collections;

public class Projectile_Laser : Projectile_Base
{
    // Update is called once per frame
    void Update()
    {
        // destroy
        if (myDist >= myRange)
        {
            Destroy(gameObject);
        }
        OnUpdate();
    }
}
