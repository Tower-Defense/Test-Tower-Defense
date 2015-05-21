using UnityEngine;
using System.Collections;

public class LaserExplosion : MonoBehaviour
{
    public float lifeTime = 1.0f;
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 1.0f);
    }
}
