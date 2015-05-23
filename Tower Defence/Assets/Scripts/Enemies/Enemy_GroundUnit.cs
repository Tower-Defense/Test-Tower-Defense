using UnityEngine;
using System.Collections;
using Pathfinding;


public class Enemy_GroundUnit : Enemy_Base {

	public Transform tankTurret;
	public Transform tankBody;
	public Transform tankCompass;
	public float turnSpeed = 10.0f;
	
	public Vector3 targetPosition; // the destination position
	public Seeker seeker; // the seeker component on this object, this aids in building with path
	public CharacterController controller; 
	public Path path; // this will hold the path to follow
	public float nextWaypointDistance = 3.0f; // min distance required to move toward next waypoint
	private int currentWaypoint = 0; // index of the waypoint this object is currently at


	// Use this for initialization
	void Start () {
        targetPosition = GameObject.FindWithTag("PlayerBase").transform.position;
		GetNewPath ();	
	
	}

	public void GetNewPath () {
		seeker.StartPath (transform.position, targetPosition, OnPathComplete);
	}
	
	private void OnPathComplete(Path newPath) {
		if (!newPath.error) {
			path = newPath;
			currentWaypoint = 0;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (path == null) {
			return;
		}
		// reached end of path?
		if (currentWaypoint >= path.vectorPath.Count) {
			return;
		}
		
		//find direction to next waypoint
		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized;
		//find an amount, based on speed, direction, and deltatime, to move
		dir *= forwardSpeed * Time.fixedDeltaTime;
		
		//move!
		controller.SimpleMove (dir);
		
		//rotate to face next waypoint
		tankCompass.LookAt (path.vectorPath [currentWaypoint]);
		transform.rotation = Quaternion.Lerp (transform.rotation, 
		                                     tankCompass.rotation, 
		                                     Time.deltaTime * turnSpeed);
		
		//Check if we are close enough to the next waypoint
		if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++; //if we are proceed to follow the next point
		}
		
	}
}
