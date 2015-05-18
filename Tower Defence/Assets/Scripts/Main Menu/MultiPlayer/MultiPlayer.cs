using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MultiPlayer : MonoBehaviour {
	

	//--- Master Server settings
	public string masterServerIp = "52.17.244.15";
	public int masterServerPort = 23466;
	public int facilitatorPort = 50005;
	private string gameTypeName = "My room";
	//---

	//--- Start server settings
	public Text serverPort;
	public Text serverName;
	public Text serverDescription;

	public Text serverIP;
	public Text connection;
	public Button Btn_startServer;
	public Button Btn_shutdownServer;
	public Button Btn_startGame;
	//---

	//--- Connection settings
	public Text connectPort;
	public Text connectIP;

	public Text connectionInfo;
	public GameObject infoPanel;

	private const int maxConnections = 1;
	private int port;
	private string ip;
	//---

	//---Server Browser
	public GameObject row;
	public GameObject grid;
	public GameObject passwordFrame;
	//---
	

	#region Network messages
	void OnConnectedToServer()
	{
		connectionInfo.color = Color.green;
		connectionInfo.text = "Connected.\nWaiting start game...";

	}

	void OnPlayerConnected (NetworkPlayer player)
	{
		UpdateConnection(true);
	}

	void OnFailedToConnect (NetworkConnectionError error)
	{
		connectionInfo.color = Color.red;
		connectionInfo.text = "Failed!\n" + error.ToString (); 
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		// GUI
		UpdateConnection(false);
		// Network
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (!Network.isServer) {
			if (info == NetworkDisconnection.LostConnection) {
				connectionInfo.color = Color.red;
				connectionInfo.text = "Failed!\n" + 
					"Lost connection to the server";
			}
		} else {
			// Update GUI
			Btn_startServer.gameObject.SetActive(true);
			Btn_shutdownServer.gameObject.SetActive(false);
			Btn_startGame.gameObject.SetActive(false);

			UpdateConnection(false);
		}

	}

	void OnServerInitialized() {
		MasterServer.RegisterHost(gameTypeName, 
		                          serverName.text != "" ? serverName.text : "Undefined", 
		                          serverDescription.text);

		// Update GUI
		Btn_startServer.gameObject.SetActive(false);
		Btn_shutdownServer.gameObject.SetActive(true);

		UpdateConnection(false);
	}
	
	//Master server
	void OnMasterServerEvent(MasterServerEvent msEvent) {
		switch (msEvent) {
		case MasterServerEvent.RegistrationSucceeded:
			Debug.Log ("Server success registered!");
			break;
		case MasterServerEvent.HostListReceived:
			UpdateServerBrowser();
			break;

		}
	}
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Debug.Log("Couldn't connect to master server! " + info);
	}

	/// <summary>
	/// Updates the connection text if player 2 was connected.
	/// </summary>
	/// <param name="isConnected">If set to <c>true</c> is connected.</param>
	private void UpdateConnection(bool isConnected) {
		if (isConnected) {
			Btn_startGame.gameObject.SetActive(true);
			connection.color = Color.green;
			connection.text = "Second player connected. \n You can start game!";
		} else {
			Btn_startGame.gameObject.SetActive(false);
			connection.color = Color.red;
			connection.text = "No player connected.";
		}
	}
	
	#endregion
	
	void Start()
	{
		#region Setup Master Server
		//--- Master Sever
		MasterServer.ipAddress = masterServerIp;
		MasterServer.port = masterServerPort;
		//---
		//--- Facilitator
		Network.natFacilitatorPort = facilitatorPort;
		//---
		#endregion

		//--- GUI
		Btn_startServer.gameObject.SetActive(true);
		Btn_shutdownServer.gameObject.SetActive(false);
		Btn_startGame.gameObject.SetActive(false);

		serverIP.text = "IP: " + Network.player.ipAddress;
		//---
	}


	public void StartServer(Text password)
	{
		int port = 0;;
		if (int.TryParse (serverPort.text, out port)) {
			// Start server
			Network.incomingPassword = password.text;
			Network.InitializeSecurity();
			Network.InitializeServer (maxConnections, port, true);
		} else {
			Debug.Log("Wrong Port! It must be integer");
		}

	}

	public void ShutdownServer()
	{
		// disconnect
		Network.Disconnect ();
	}

	public void ConnectToServerFromTCP(Text password)
	{
		if (int.TryParse (connectPort.text, out port)) {
			ip = connectIP.text;
			ConnectToServer(password);
		} else {
			Debug.Log("Incorrect parsing! Port must be numerical!");
		}
	}

	public void ConnectToServer(Text password)
	{
		Network.Connect (ip, port, password.text);
		infoPanel.SetActive(true);
	}

	public void DisconnectFromServer(GameObject infoPanel)
	{
		Network.Disconnect();	
		infoPanel.SetActive(false);
	}

	public void StartGame(int level = 1)
	{
		GetComponent<NetworkView> ().RPC ("LoadLevel", RPCMode.All, level);
	}

	[RPC]
	private void LoadLevel(int level)
	{
		Application.LoadLevel (level);
	}

	public void UpdateHostList()
	{
		// Update after Master Server Event HostListReceived
		// This event call UpdateServerBrowser ()
		MasterServer.RequestHostList (gameTypeName);
	}



	// Call after Master Server Event HostListReceived
	void UpdateServerBrowser() 
	{
		HostData [] hostData = MasterServer.PollHostList ();

		foreach (Transform child in grid.transform) {
			Destroy(child.gameObject);
		}

		for (int i=0; i< hostData.Length; i++ ) {

			if(hostData[i].connectedPlayers > 1)
				return;

			GameObject newRow = Instantiate(row) as GameObject;

			string ip_ = hostData[i].ip[0];
			int port_ = hostData[i].port;
			bool isProtected = hostData[i].passwordProtected;

			Text [] texts = newRow.GetComponentsInChildren<Text>();
			texts[0].text = hostData[i].gameName;
			texts[1].text = ip_ + ":" + port_.ToString();
			texts[2].text = hostData[i].comment;
			texts[3].text = isProtected ? "Yes" : "No";




			newRow.GetComponent<Button>().onClick
				.AddListener(() => ConnectToServerFromBrowser(ip_, port_, isProtected));

			newRow.transform.SetParent (grid.transform);

			RectTransform rectTransform = (RectTransform)newRow.transform;
			rectTransform.anchoredPosition3D = new Vector3(0,0,0);
			rectTransform.localScale = new Vector3(1,1,1);
		}

	}

	void ConnectToServerFromBrowser(string ip_,int port_, bool isProtected) {
		if (!isProtected) {
			Network.Connect (ip_, port_, "");
			infoPanel.SetActive(true);
		}
		else {
			passwordFrame.SetActive(true);
			this.ip= ip_;
			this.port = port_;
		}
			

	}
	
}
