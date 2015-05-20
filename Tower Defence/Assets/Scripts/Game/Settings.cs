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
        _sound = PlayerPrefs.GetInt("Sound", 1) == 0;
        _music = PlayerPrefs.GetInt("Music", 1) == 0;

        SetSound();        
        SetMusic();

    }

    public void SetMusic()
    {
        _music = !_music;
        MusicYes.SetActive(_music);
        MusicNo.SetActive(!_music);
        

        PlayerPrefs.SetInt("Music", _music ? 1 : 0);
        Camera.main.GetComponent<AudioSource>().enabled = _music;

        Debug.Log("SetMusic:" + _music.ToString());
    
    }

    public void SetSound()
    {
        _sound = !_sound;
        SoundYes.SetActive(_sound);
        SoundNo.SetActive(!_sound);

        PlayerPrefs.SetInt("Sound", _sound ? 1 : 0);

        Debug.Log("SetSound:" + _sound.ToString());
    }
}
