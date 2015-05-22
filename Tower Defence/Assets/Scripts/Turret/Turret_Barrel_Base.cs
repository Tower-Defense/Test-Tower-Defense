using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret_Barrel_Base : Turret_Base
{

    public GameObject myProjectile;
    /// <summary>
    /// How much time need to reload before fire
    /// </summary>
    public float reloadTime = 1f;
    /// <summary>
    /// How quickly you can turn
    /// </summary>
    public float turnSpeed = 5f;
    public float firePauseTime = .125f;    
    public float errorAmount = 0.01f;

    // 
    public List<string> TargetEnemyTags;
    public Transform myTarget;

    public GameObject muzzleEffect;
    public Transform[] muzzlePositions;

    protected AudioSource audio = null;
    protected float nextFireTime;

    /// <summary>
    /// The aim error.
    /// Погрешность в выстреле
    /// </summary>
    protected float aimError;


    void Awake()
    {
        CalculateAimError();
        nextFireTime = 0.0f;
        audio = GetComponent<AudioSource>();

    }


    void OnTriggerStay(Collider other)
    {
        if (myTarget == null)
        {
            if (TargetEnemyTags.Contains(other.gameObject.tag))
            {

                myTarget = other.gameObject.transform;
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform == myTarget)
        {
            myTarget = null;
        }

    }

    protected void CalculateAimError()
    {
        aimError = Random.Range(-errorAmount, errorAmount);
    }

    protected virtual void FireProjectile()
    {
        if (audio != null)
        {
            audio.Play();
        }
        nextFireTime = Time.time + reloadTime;
    }

}
