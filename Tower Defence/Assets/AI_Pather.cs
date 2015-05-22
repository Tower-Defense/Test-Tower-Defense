using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

public class AI_Pather : MonoBehaviour {

	public Vector3 target;
    public Transform enemyFront;
    public float turnSpeed = 10.0f;

	Seeker seeker;
    Path path;
    int currentWaypoint;

    float speed = 1f;

    CharacterController characterController;

    float maxWaypointDistance = 3f;

	void Start() {
        target = GameObject.FindWithTag("target").transform.position;
		seeker = GetComponent<Seeker>();
		seeker.StartPath( transform.position, target, OnPathComplete );
        characterController = GetComponent<CharacterController>();
	}

	public void OnPathComplete( Path p ) {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            Debug.Log(path.vectorPath.Count);
        }
        else
        {
            Debug.Log(p.error);
        }
	}

    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        

        transform.position = path.vectorPath[currentWaypoint];

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized * speed * Time.fixedDeltaTime;
       
        characterController.SimpleMove(dir);

        enemyFront.LookAt(path.vectorPath[currentWaypoint]);
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                             enemyFront.rotation,
                                             Time.deltaTime * turnSpeed);

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < maxWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}