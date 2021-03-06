﻿using UnityEngine;
using System.Collections;

public class Turret_Cannon : Turret_Barrel_Base
{
    public Transform turretBall;

    protected float nextMoveTime;
    protected Quaternion desiredRotation;


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

            if (Time.time >= nextFireTime)
            {
                FireProjectile();
            }
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
        nextMoveTime = Time.time + firePauseTime;
        CalculateAimError();

        foreach (var theMuzzlePos in muzzlePositions)
        {
            Instantiate(myProjectile, theMuzzlePos.position, theMuzzlePos.rotation);
            if (muzzleEffect != null)
                Instantiate(muzzleEffect, theMuzzlePos.position, theMuzzlePos.rotation);
        }
    }
}
