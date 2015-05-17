using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	//--- Windows type
	private enum WindowTypes{
		MainMenu = 0,
		NewGame = 1,
		Multiplayer = 2,
		Settings = 3,
		Score = 4,
		Exit = -1
	};

	private WindowTypes currentwindow;
	// ---




	//--- Master Server settings
	public string ipAddress = "52.17.244.15";
	public int masterServerPort = 23466;
	public int facilitatorPort = 50005;
	//---

	private bool isFirst;


	void OnMasterServerEvent()
	{
		Debug.Log ("register");
	}


	// Use this for initialization
	void Start () {
		currentwindow = WindowTypes.MainMenu;


		isFirst = true;
		// Master Server
		MasterServer.ipAddress = ipAddress;
		MasterServer.port = 23466;

		// Facilitator
		Network.natFacilitatorIP = ipAddress;
		Network.natFacilitatorPort = 50005;


	
	}


	void OnGUI()
	{
		switch (currentwindow) {
		case WindowTypes.MainMenu:

			break;
		case WindowTypes.NewGame:

			break;

		case WindowTypes.Multiplayer:

			break;

		case WindowTypes.Score:

			break;

		case WindowTypes.Settings:

			break;

		case WindowTypes.Exit:
			Application.Quit();
			break;

		default:
		{
			Debug.Log ("Undefined WindowType: " + currentwindow.ToString());
			break;
		}
		}


		if (isFirst) {
			Debug.Log ("hi");
			Network.InitializeServer (2, 25000, true);
		
			MasterServer.RegisterHost ("MyRoom", "Name", "ddd");
			isFirst =false;
		}
	}

	
}
