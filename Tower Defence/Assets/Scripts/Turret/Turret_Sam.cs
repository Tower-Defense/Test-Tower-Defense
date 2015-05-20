using UnityEngine;
using System.Collections;

public class Turret_Sam : Turret_Base {

	public GameObject myProjectile;
	/// <summary>
	/// The reload time.
	/// How much time need to reload before fire
	/// </summary>
	public float reloadTime = 1f;
	/// <summary>
	/// The turn speed.
	/// How quickly you can turn
	/// </summary>
	public float turnSpeed = 5f;
	public float firePauseTime = .25f;
	public GameObject muzzleEffect;
	public float errorAmount = 0.001f;
	public Transform myTarget;
	public Transform[] muzzlePositions;
	public Transform pivot_Tilt;
	public Transform pivot_Pan;
	public Transform aim_Tilt;
	public Transform aim_Pan;	
	
	private AudioSource audio;
	private float nextFireTime;
	private Vector3 desiredRotation;


	// Use this for initialization
	void Start () {
		nextFireTime = 0.0f;
		audio = GetComponent<AudioSource> ();
	}
	
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
/*
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Enemy") {
			nextFireTime = Time.time +(reloadTime * 0.5f);
			myTarget = other.gameObject.transform;
		}
	}
	*/
	void OnTriggerStay(Collider other) {
		if (!myTarget) {
			if (other.gameObject.tag == "Air Enemy") {
				nextFireTime = Time.time + (reloadTime * 0.5f);
				myTarget = other.gameObject.transform;
			}
		}
	}

	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.transform == myTarget) {
			myTarget = null;
		}
	}

	private void FireProjectile() {
		audio.Play ();
		nextFireTime = Time.time + reloadTime;

        int rand = Random.Range(0, muzzlePositions.Length);
		GameObject newMissile = Instantiate (myProjectile, muzzlePositions [rand].position, muzzlePositions [rand].rotation) as GameObject;
		var projectile = newMissile.GetComponent<Projectile_Missile> ();
		projectile.myTarget = myTarget;
	}
}
