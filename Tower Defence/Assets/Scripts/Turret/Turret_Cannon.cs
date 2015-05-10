using UnityEngine;
using System.Collections;

public class Turret_Cannon : Turret_Base {
	
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
	public float firePauseTime = .125f;
	public GameObject muzzleEffect;
	public float errorAmount = 0.01f;
	public Transform myTarget;
	public Transform[] muzzlePositions;
	public Transform turretBall;


	private AudioSource audio;
	private float nextFireTime;
	private float nextMoveTime;
	private Quaternion desiredRotation;
	/// <summary>
	/// The aim error.
	/// Погрешность в выстреле
	/// </summary>
	private float aimError;


	// Use this for initialization
	void Start () {
		CalculateAimError ();
		audio = GetComponent<AudioSource> ();	
	}
	
	// Update is called once per frame
	void Update () {
		if (myTarget != null) {
			if(Time.time >= nextMoveTime)
			{
				CalculateAimPosition(myTarget.position);
				turretBall.rotation = Quaternion.Lerp(turretBall.rotation, desiredRotation, Time.deltaTime*turnSpeed);
			}

			if(Time.time >= nextFireTime)
			{
				FireProjectile();
			}
		}
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Ground Enemy") {
			nextFireTime = Time.time +(reloadTime * 0.5f);
			myTarget = other.gameObject.transform;

		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.transform == myTarget) {
			myTarget = null;
		}
	}

	private void CalculateAimPosition(Vector3 targetPos) {
		var aimPoint = new Vector3 (targetPos.x - transform.position.x + aimError, 
		                            targetPos.y -transform.position.y + aimError, 
		                            targetPos.z - transform.position.z + aimError);
		desiredRotation = Quaternion.LookRotation (aimPoint);
	}

	private void CalculateAimError() {
		aimError = Random.Range (-errorAmount, errorAmount);
	}

	private void FireProjectile()
	{
		audio.Play ();
		nextFireTime = Time.time + reloadTime;
		nextMoveTime = Time.time + firePauseTime;
		CalculateAimError ();

		foreach (var theMuzzlePos in muzzlePositions) {
			Instantiate(myProjectile, theMuzzlePos.position, theMuzzlePos.rotation);
			Instantiate(muzzleEffect, theMuzzlePos.position, theMuzzlePos.rotation);
		}
	}
}
