using UnityEngine;
using System.Collections;

public class PlayerBase : MonoBehaviour {

	private LevelMaster levelMaster;

	void OnTriggerEnter(Collider other) {
        if (levelMaster == null)
        {
            levelMaster = LevelMaster.Instance;
        }
	//	Debug.Log("Trigger");
		if (other.gameObject.tag == "Ground Enemy" 
		    || other.gameObject.tag == "Air Enemy") {
	//		Debug.Log("Boom");
            Destroy(other.gameObject);
			levelMaster.enemyCount--;
			levelMaster.livesCount--;
			levelMaster.UpdateGUI ();
		}
	}
}
