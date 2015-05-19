using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MultiPlayer : Photon.MonoBehaviour
{
    //--- Start/Join room
    public Text PlayerName;
    public Text RoomName;
    public Toggle IsVisible;

    public Button Btn_CreateRoom;
    public Button Btn_JoinRoom;
    public Button Btn_JoinRandom;

    public InfoPanel infoPanel;

    public int levelNum = 1;
    //---

    //---Server Browser
    public GameObject RowPrefab;
    public GameObject GridDataBrowser;
    //---

    //---Photon Settings
    private const int maxPlayers = 2;
    const string VERSION = "v0.0.1";
    //---



    #region Network messages
 
    void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        
        Debug.Log("OnPhotonPlayerConnected");
        StartGame();

    }
    
    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.Log("OnFailedToConnectToPhoton");

        GUIWithoutConnection(cause.ToString());
    }

    void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
    }

    
    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPhotonPlayerDisconnected");

        PhotonNetwork.LoadLevel(0);

    }

    void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");
    }

    void OnConnectedToPhoton()
    {
        Debug.Log("OnConnectedToPhoton");

    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined to room");
    }


    void OnConnectionFail(DisconnectCause cause)
    {
        GUIWithoutConnection(cause.ToString());
    }

    void GUIWithoutConnection(string cause)
    {
        infoPanel.OpenInfoPanel(cause, Color.red);
        Btn_CreateRoom.enabled = false;
        Btn_JoinRandom.enabled = false;
        Btn_JoinRoom.enabled = false;
    }

    #endregion
    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel()
        // on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client
        // was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved 
            // in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings(VERSION);
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PlayerName.text = PhotonNetwork.playerName = "Guest" + UnityEngine.Random.Range(1, 9999);
        }

    }


    public void CreateRoom()
    {
        if(PhotonNetwork.connectedAndReady)
        {
            PhotonNetwork.CreateRoom(
                RoomName.text == "" ? "New Room" + UnityEngine.Random.Range(1, 9999) : RoomName.text,
                new RoomOptions() { maxPlayers = maxPlayers, isVisible = IsVisible.isOn },
                null);

            PhotonNetwork.playerName = PlayerName.text;
            // Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
            
            infoPanel.OpenInfoPanel("Connected.\nWaiting player 2...", Color.green);
        }
        else
        {
            Debug.Log("You aren't ready to create room");
        }

    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomName.text);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(levelNum);
    }

    // Call after Master Server Event HostListReceived
    public void UpdateServerBrowser()
    {
        Debug.Log(PhotonNetwork.GetRoomList().Length.ToString());

        foreach (Transform child in GridDataBrowser.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
        {

            if (roomInfo.playerCount > 1 || !roomInfo.open)
                return;

            GameObject newRowPrefab = Instantiate(RowPrefab) as GameObject;

            string name_ = roomInfo.name;


            Text[] texts = newRowPrefab.GetComponentsInChildren<Text>();
            texts[0].text = roomInfo.name;
            texts[1].text = roomInfo.playerCount.ToString();
            texts[2].text = roomInfo.maxPlayers.ToString();
            texts[3].text = roomInfo.open ? "No" : "Yes";

            newRowPrefab.GetComponent<Button>().onClick
                .AddListener(() => { 
                    PhotonNetwork.JoinRoom(roomInfo.name); });

            newRowPrefab.transform.SetParent(GridDataBrowser.transform);

            RectTransform rectTransform = (RectTransform)newRowPrefab.transform;
            rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

    }

}
