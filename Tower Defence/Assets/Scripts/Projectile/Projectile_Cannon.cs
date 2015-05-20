using UnityEngine;
using System.Collections;

public class Projectile_Cannon : Projectile_Base {

    protected override void Explode()
    {
        base.Explode();
        Destroy(gameObject);
    }

	// Update is called once per frame
	void  Update () {

		// destroy
		if (myDist >= myRange) {
			Destroy (gameObject);
		}
		OnUpdate ();		
	}
}
