using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

public class AI_Pather : MonoBehaviour {

	public Transform target;

	Seeker seeker;
    Path path;
    int currentWaypoint;

    float speed = 1;

    CharacterController characterController;

    float maxWaypointDistance = 2f;

	void Start() {
		seeker = GetComponent<Seeker>();
		seeker.StartPath( transform.position, target.position, OnPathComplete );
        characterController = GetComponent<CharacterController>();
	}

	public void OnPathComplete( Path p ) {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
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

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < maxWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}