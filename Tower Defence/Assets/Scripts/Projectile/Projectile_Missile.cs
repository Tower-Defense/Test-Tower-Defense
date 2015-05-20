using UnityEngine;
using System.Collections;

public class Projectile_Missile : Projectile_Base {

    protected override void Explode()
    {
        base.Explode();
        Destroy(gameObject);
    }

	// Update is called once per frame
	void Update () {

		OnUpdate ();

		// destroy
		if (myDist >= myRange) {
			Explode ();
		}

		if (myTarget) {
			transform.LookAt (myTarget);
		} else {
			Explode ();
		}	
	}

}
