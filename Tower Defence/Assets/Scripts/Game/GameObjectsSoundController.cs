using UnityEngine;
using System.Collections;

public class GameObjectsSoundController : MonoBehaviour
{
    private AudioSource audio;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        if(audio ==null)
        {
            enabled = false;
        }

    }

    // Update is called once per frame
    void OnGUI()
    {
        if (GameController.isSound)
            audio.mute = false;
        else
            audio.mute = true;
    }
}
