using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

public class AI_Pather : MonoBehaviour {

	public Vector3 targetPosition;
    public Transform enemyFront;
    public float turnSpeed = 10.0f;

	public Seeker seeker;
    public Path path;
    public int currentWaypoint;

    public float speed = 500f;

    public CharacterController characterController;

    public float maxWaypointDistance = 3f;

	void Start() {
        targetPosition = GameObject.FindGameObjectWithTag("PlayerBase")
            .GetComponent<Transform>()
            .position;
        seeker = GetComponent<Seeker>();
        characterController = GetComponent<CharacterController>();
        GetNewPath();
        
	}

    public void GetNewPath()
    {
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
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

        

        // Wtf? Why you do this?
        // It's look like teleport
      //  transform.position = path.vectorPath[currentWaypoint];

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir*= speed * Time.fixedDeltaTime;
       
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