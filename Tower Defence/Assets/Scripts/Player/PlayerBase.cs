using UnityEngine;
using System.Collections;

public class PlayerBase : MonoBehaviour {

	public LevelMaster levelMaster;
	// Use this for initialization
	void Start () {
		//connect to levelmaster
		levelMaster = GameObject.FindWithTag ("LevelMaster").GetComponent <LevelMaster>();
	}

	void OnTriggerEnter(Collider other) {
	//	Debug.Log("Trigger");
		if (other.gameObject.tag == "Ground Enemy" 
		    || other.gameObject.tag == "Air Enemy") {
	//		Debug.Log("Boom");
			PhotonNetwork.Destroy(other.gameObject);
			levelMaster.enemyCount--;
			levelMaster.livesCount--;
			levelMaster.UpdateGUI ();
		}
	}
}
