using UnityEngine;
using System.Collections;

public class Turret_Sam : Turret_Barrel_Base
{
	public Transform pivot_Tilt;
	public Transform pivot_Pan;
	public Transform aim_Tilt;
	public Transform aim_Pan;	
	
	private Vector3 desiredRotation;

	
	// Update is called once per frame
	void Update () {	
		if (myTarget) {
			aim_Pan.LookAt (myTarget);
			aim_Pan.eulerAngles = new Vector3(0, aim_Pan.eulerAngles.y, 0);
			aim_Tilt.LookAt (myTarget);

			pivot_Pan.rotation = Quaternion.Lerp (pivot_Pan.rotation, aim_Pan.rotation, Time.deltaTime * turnSpeed);
			pivot_Tilt.rotation = Quaternion.Lerp (pivot_Tilt.rotation, aim_Tilt.rotation, Time.deltaTime * turnSpeed);
		

			if (Time.time >= nextFireTime) {
				FireProjectile ();
			}
		}

	}

	protected override void FireProjectile() {
        base.FireProjectile();

        int rand = Random.Range(0, muzzlePositions.Length);
		GameObject newMissile = Instantiate (myProjectile, muzzlePositions [rand].position, muzzlePositions [rand].rotation) as GameObject;
		var projectile = newMissile.GetComponent<Projectile_Missile> ();
		projectile.myTarget = myTarget;
	}
}
