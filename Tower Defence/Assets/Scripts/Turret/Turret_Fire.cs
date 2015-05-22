using UnityEngine;
using System.Collections;

public class Turret_Fire : Turret_Barrel_Base
{

    public Transform turretBall;

    protected float nextMoveTime;
    protected Quaternion desiredRotation;

    private bool isFire = false;
    private FireRay fireRay;


    void Start()
    {
        fireRay = myProjectile.GetComponent<FireRay>();
     
    }

    // Update is called once per frame
    void Update()
    {
        if (myTarget != null)
        {
            if (Time.time >= nextMoveTime)
            {
                CalculateAimPosition(myTarget.position);
                turretBall.rotation = Quaternion.Lerp(turretBall.rotation, desiredRotation, Time.deltaTime * turnSpeed);
            }
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (myTarget != null)
        {
            nextFireTime = Time.time + (reloadTime * 0.5f);

        }
        else
        {
            if (TargetEnemyTags.Contains(other.gameObject.tag))
            {
                myTarget = other.gameObject.transform;
                fireRay.StartFire(myTarget);
                if (audio != null)
                    audio.Play();
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform == myTarget)
        {
            myTarget = null;
            fireRay.myTarget = null;
            if (audio != null)
                audio.Stop();
        }

    }

    private void CalculateAimPosition(Vector3 targetPos)
    {
        var aimPoint = new Vector3(targetPos.x - transform.position.x + aimError,
                                    targetPos.y - transform.position.y + aimError,
                                    targetPos.z - transform.position.z + aimError);
        desiredRotation = Quaternion.LookRotation(aimPoint);
    }


    protected override void FireProjectile()
    {
        base.FireProjectile();

        CalculateAimError();
    }
}
