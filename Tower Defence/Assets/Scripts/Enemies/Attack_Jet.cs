using UnityEngine;
using System.Collections;

public class Attack_Jet : Enemy_Base {

	public Transform compass;
	public Vector2 heightRange = new Vector2(10.0f, 18.0f);
	private float height =  10f;

	// Use this for initialization
	void Start () {
		height = Random.Range (heightRange.x, heightRange.y);
		//choose random height
		transform.position
			.Set ( transform.position.x,
			      height,
			      transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		// поворачиваем к базе
		var target = GameObject.FindWithTag ("PlayerBase").transform;
		compass.LookAt (target);
		compass.eulerAngles = new Vector3(0, compass.eulerAngles.y, 0);
		transform.rotation = Quaternion.Lerp (transform.rotation, 
		                                     compass.rotation, 
		                                     Time.deltaTime * 10.0f);

		transform.Translate (Vector3.forward*(forwardSpeed*Time.deltaTime));	
	}


}
