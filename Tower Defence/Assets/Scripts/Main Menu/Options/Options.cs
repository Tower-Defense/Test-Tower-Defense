using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Toggle Music;
    public Toggle Sound;
    public Text Difficuty; 

    private bool _music;
    private bool _sound;
    private string _difficulty;
    
    void Awake ()
    {
        Music.isOn = _music = PlayerPrefs.GetInt("Music", 1) > 0;
        Sound.isOn = _sound = PlayerPrefs.GetInt("Sound", 1) > 0;
        Difficuty.text = _difficulty = PlayerPrefs.GetString("Diffciculty", "Easy"); 
    }

    public void SetMusic()
    {
        
        _music = Music.isOn;
        Camera.main.GetComponent<AudioSource>().enabled = _music;
        PlayerPrefs.SetInt("Music", _music ? 1 : 0 );
        Debug.Log("SetMusic:" + _music.ToString());
    }

    public void SetSound()
    {
        
        _sound = Sound.isOn;
        PlayerPrefs.SetInt("Sound", _sound ? 1 : 0);
        Debug.Log("SetSound:" + _sound.ToString());
    }


    public void ChooseDifficulty(Text difficulty)
    {
        
        Difficuty.text = _difficulty = difficulty.text;
        PlayerPrefs.SetString("Diffciculty", _difficulty);
        Debug.Log("ChooseDifficulty" + _difficulty);

    }
}
