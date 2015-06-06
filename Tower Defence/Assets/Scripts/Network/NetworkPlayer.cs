
using UnityEngine;
using System.Collections;
public class NetworkPlayer : Photon.MonoBehaviour
{
    public GameObject myCamera;
    bool isAlive = true;
    public float lerpSmoothing = 10.0f;
    public Vector3 position;
    public Rigidbody rigidbody;
    public Quaternion rotation;
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
       /* if (photonView.isMine)
        {
            
            myCamera.SetActive(true);
 
        }
        else
        {
            
            StartCoroutine("Alive");
        }
        */
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {/*
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }*/

        if (stream.isWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(rigidbody.velocity);
        }
        else
        {
            Vector3 syncPosition = (Vector3)stream.ReceiveNext();
            Vector3 syncVelocity = (Vector3)stream.ReceiveNext();

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = rigidbody.position;
        }

    }
    //while alive do this state-machine
    IEnumerator Alive()
    {
        while (isAlive)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpSmoothing);
            yield return null;
        }
    }
}
