using UnityEngine;
using System.Collections;

public class Projectile_Missile : Projectile_Base {

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
