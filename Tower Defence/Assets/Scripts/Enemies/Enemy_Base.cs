using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all enemies
/// </summary>
public class Enemy_Base : MonoBehaviour {

    private LevelMaster levelMaster;

	

	public int myCashValue = 50;
	public Vector2 speedRange = new Vector2(7.0f, 10.0f);
	protected float forwardSpeed = 10.0f;
	public float health = 100f;	
	protected float maxHealth = 100f;

	//Before Start()
	//When script initial
	void Awake() {
		//connect to levelmaster
		levelMaster = LevelMaster.Instance;

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
            DamageEffect();
			health -= damageAmount;
			if (health <= 0) {
				Explode ();
				return;
			} else if (health / maxHealth <= 0.75) {
				// smoke
                HardDamageEffect();				
			}
		}
	}

    protected virtual void HardDamageEffect() { }
    protected virtual void DamageEffect() { }
	
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

        ExplosionEffect();
        
            
	}
    protected virtual void ExplosionEffect() {
        Destroy(gameObject);
    }

}
