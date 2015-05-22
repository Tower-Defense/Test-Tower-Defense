using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public enum DifficultiyTypes
    {
        Easy = 0,
        Middle = 1,
        Hard = 2
    };
    DifficultiyTypes _difficultyType = DifficultiyTypes.Easy;

    public float[] difficultyMultipliers = { 1.0f, 
                                               2.0f, 
                                               3.0f};

    public Toggle Music;
    public Toggle Sound;
    public Text Difficuty; 

    private string _difficulty;
    
    void Awake ()
    {
        Music.isOn = GameController.isMusic;
        Sound.isOn = GameController.isSound;
        _difficultyType = (DifficultiyTypes) PlayerPrefs.GetInt("DiffcicultyType", 0);
        Difficuty.text = _difficultyType.ToString();
    }

    public void SetMusic()
    {        
        GameController.isMusic = Music.isOn;
    }

    public void SetSound()
    {
        GameController.isSound = Sound.isOn;
    }


    public void ChooseDifficulty(int difficultyType)
    {
        _difficultyType = (DifficultiyTypes)difficultyType;
        Difficuty.text = _difficultyType.ToString();
        PlayerPrefs.SetInt("DiffcicultyType", (int)_difficultyType);
        PlayerPrefs.SetFloat("DifficultyMultiplier", difficultyMultipliers[(int)_difficultyType]);
        Debug.Log("ChooseDifficultyType: " + _difficultyType);

    }
}
