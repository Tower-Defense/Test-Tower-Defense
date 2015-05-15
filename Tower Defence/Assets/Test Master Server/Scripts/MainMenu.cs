using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	private bool isFirst;
	private string ipAddress = "52.17.244.15";
	// Use this for initialization
	void Start () {
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
		if (isFirst) {
			Debug.Log ("hi");
			Network.InitializeServer (2, 25000, true);
		
			MasterServer.RegisterHost ("MyRoom", "Name", "ddd");
			isFirst =false;
		}
	}

	void OnMasterServerEvent()
	{
		Debug.Log ("register");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
