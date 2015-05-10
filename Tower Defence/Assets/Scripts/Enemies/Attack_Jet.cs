using UnityEngine;
using System.Collections;

public class Attack_Jet : Enemy_Base {

	public Vector2 heightRange = new Vector2(10.0f, 18.0f);

	// Use this for initialization
	void Start () {
		//choose random height
		transform.position
			.Set ( transform.position.x,
			      Random.Range (heightRange.x, heightRange.y),
			      transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.forward*(forwardSpeed*Time.deltaTime));	
	}


}
