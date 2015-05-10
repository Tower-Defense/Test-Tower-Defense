using UnityEngine;
using System.Collections;

public class PlayerBase : MonoBehaviour {

	public LevelMaster levelMaster;
	// Use this for initialization
	void Start () {
		//connect to levelmaster
		levelMaster = GameObject.FindWithTag ("LevelMaster").GetComponent <LevelMaster>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Ground Enemy" 
		    || other.gameObject.tag == "Air Enemy") {
			Destroy(other.gameObject);
			levelMaster.enemyCount--;
			levelMaster.healthCount--;
			levelMaster.UpdateGUI ();
		}
	}
}
