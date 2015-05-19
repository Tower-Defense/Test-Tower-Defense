/*
using UnityEngine;
using System.Collections;
public class NetworkPlayer : Photon.MonoBehaviour
{
    public GameObject myCamera;
    bool isAlive = true;
    public float lerpSmoothing = 10.0f;
    public Vector3 position;
    public Quaternion rotation;
    // Use this for initialization
    void Start()
    {
        if (photonView.isMine)
        {
            gameObject.name = "Me";
            myCamera.SetActive(true);
 
        }
        else
        {
            gameObject.name = "Network Player";
            StartCoroutine("Alive");
        }
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
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
*/