using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {

	private bool isPause = false;

	//--- Settings
	// Размер экрана
	private int screenWidth;
	private int screenHeight;
	private bool fullScreen;
	//
	// Скорость воспроизведения
	private float timeScale = 1;
	// Audio
	private bool isAudio;
	//--


	// Для selection Grid
	private int selGridInt = 0; // Какое окно выделено 
	private string [] selStrings = new string[] {"Pause Menu","Settings"};
	//

	// Use this for initialization
	void OnGUI()
	{
		if (isPause) {
			Time.timeScale = 0;

			GUI.BeginGroup(new Rect(50,25, Screen.width/2, Screen.width/2));
			if(GUI.Button(new Rect(0,0,20,20), "X"))
			{
				Time.timeScale =1;
			//	Time.timeScale*= 2;
				isPause =false;
			}

			selGridInt = GUI.SelectionGrid(new Rect(5,12,100*3,23), selGridInt, selStrings, 3);
			if(selGridInt==1)
			{
				GUI.Button(new Rect(Screen.width/4, Screen.height/4, 100, 40), "Settings");

			}
			GUI.EndGroup();
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			isPause = true;

		}

		if (Input.GetMouseButtonDown(1)) {
			Time.timeScale*= 2;
		}
	}
}
