﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all enemies
/// </summary>
public class Enemy_Base : MonoBehaviour {

	public LevelMaster levelMaster;

	public ParticleEmitter smokeTrail;
	public GameObject explosionEffect;

	public int myCashValue = 50;
	public Vector2 speedRange = new Vector2(7.0f, 10.0f);
	protected float forwardSpeed = 10.0f;
	public float health = 100f;	
	protected float maxHealth = 100f;

	//Before Start()
	//When script initial
	void Awake() {
		//connect to levelmaster
		levelMaster = GameObject.FindWithTag ("LevelMaster").GetComponent <LevelMaster>();

		//set health and speed
		maxHealth = health;
		forwardSpeed = Random.Range (speedRange.x, speedRange.y);
		
		//multiply the speed and health based on difficulty
		forwardSpeed *= levelMaster.difficultyMultiplier;
		health *= levelMaster.difficultyMultiplier;
		maxHealth *= levelMaster.difficultyMultiplier;
	}


	public void TakeDamage(float damageAmount) {
		if (health > 0) {
			health -= damageAmount;
			if (health <= 0) {
				Explode ();
				return;
			} else if (health / maxHealth <= 0.75) {
				// smoke
				smokeTrail.emit = true;
			}
		}
	}
	
	/// <summary>
	/// Explode this instance.
	/// Boom!
	/// </summary>
	protected void Explode () {
		
		//tell the level master an enemy was destroyed
		levelMaster.enemyCount--;
		levelMaster.cashCount += myCashValue;
		levelMaster.scoreCount += (int)(maxHealth + forwardSpeed * levelMaster.difficultyMultiplier);
		levelMaster.UpdateGUI ();
		
		Instantiate (explosionEffect, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}

}
