using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour
{

    public GameObject MusicYes;
    public GameObject MusicNo;
    public GameObject SoundYes;
    public GameObject SoundNo;

    private bool _sound;
    private bool _music;


    void Awake()
    {
        _sound = !GameController.isSound;
        _music = !GameController.isMusic;

        SetSound();        
        SetMusic();

    }

    public void SetMusic()
    {
        _music = !_music;
        MusicYes.SetActive(_music);
        MusicNo.SetActive(!_music);

        GameController.isMusic =_music;
    
    }

    public void SetSound()
    {
        _sound = !_sound;
        SoundYes.SetActive(_sound);
        SoundNo.SetActive(!_sound);

        GameController.isSound = _sound;
    }
}
