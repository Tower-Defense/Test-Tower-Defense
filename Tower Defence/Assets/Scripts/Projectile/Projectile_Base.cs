using UnityEngine;
using System.Collections;

public class Projectile_Base : MonoBehaviour {

	public GameObject myExplosion;
	public Transform myTarget;
	public float mySpeed = 10f;
	public float myRange = 10f;
	
	public float myDamageAmount = 25f;
	
	protected float myDist;

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Air Enemy" || other.gameObject.tag == "Ground Enemy") {
			Explode ();
			// Вызвать метод
			other.gameObject.SendMessage("TakeDamage",myDamageAmount, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// Взрыв 
	/// </summary>
	protected void Explode() {
		Instantiate (myExplosion, transform.position, Quaternion.identity);
		Destroy (gameObject);		
	}

	/// <summary>
	/// Actions when update.
	/// </summary>
	protected void OnUpdate() {
		// distance after one update
		float distPerUpdate = Time.deltaTime * mySpeed;
		// move
		transform.Translate (Vector3.forward * distPerUpdate);
		myDist += distPerUpdate;
		


	}
}
