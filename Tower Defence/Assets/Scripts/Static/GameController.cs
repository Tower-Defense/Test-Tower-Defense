using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GameController 
{
    public delegate void VoidEventHandler();

    public static event VoidEventHandler OnPause;
    public static event VoidEventHandler OnResume;
    public static event VoidEventHandler OnReplay;
    public static event VoidEventHandler BeforeGoToMainMenu;
    public static event VoidEventHandler BeforeGoNextLevel;

    public static int currentLevel = 1;
    public static int prevLevel = 0;
    

    public static bool isSound
    {
        get
        {
            return PlayerPrefs.GetInt("Sound", 1) != 0;
        }
        set
        {
            PlayerPrefs.SetInt("Sound", value ? 1 : 0);
            Debug.Log("Audio:" + value.ToString());
        }
    }

    public static bool isMusic
    {
        get
        {
            return isMusic = PlayerPrefs.GetInt("Music", 1) != 0;
        }
        set
        {
            PlayerPrefs.SetInt("Music", value ? 1 : 0);

            foreach (var audio in Camera.main.GetComponents<AudioSource>())
            {
                audio.enabled = value;

            }
            Debug.Log("Music:" + value.ToString());
        }
    }

    private static bool _isPause = false;
    public static bool IsPause
    {
        get
        {
            return _isPause;
        }
        set
        {
            if(value)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public static void PauseGame()
    {
        _isPause = true;
        Time.timeScale = 0;
        if (OnPause != null)
            OnPause ();
    }
    public static void ResumeGame()
    {
        _isPause = false;
        Time.timeScale = 1;
        if (OnResume != null)
            OnResume ();
    }

    public static void ReplayGame()
    {
        if(_isPause)
            ResumeGame();
        PlayerPrefs.SetInt("LoadLevel", Application.loadedLevel);
        Application.LoadLevel("Loading");
    
        if (OnReplay != null)
            OnReplay ();
    }

    public static void GoToMenu()
    {
        if (_isPause)
            ResumeGame();
        if (BeforeGoToMainMenu != null)
            BeforeGoToMainMenu(); 

        PlayerPrefs.SetInt("LoadLevel", 1);
        Application.LoadLevel("Loading");       
    }


    public static void GoNextLevel(int levelInt)
    {
        if (_isPause)
            ResumeGame();

        if (BeforeGoNextLevel != null)
              BeforeGoNextLevel();
        prevLevel = currentLevel;
        currentLevel = levelInt;
        Application.LoadLevel(levelInt);
    }
   
}
