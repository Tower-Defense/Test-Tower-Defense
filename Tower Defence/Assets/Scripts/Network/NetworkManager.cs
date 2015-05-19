using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {

	const string VERSION = "v0.0.1";
	public string roomName = "Room Name";
	public string playerPrefabName = "goblin";
	public Transform spawnPoint;

	// Use this for initialization
	void Start () {
        // don't auto connect to lobby
        PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.ConnectUsingSettings (VERSION);
	
	}
	
	void OnJoinedLobby(){
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
		PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
	}

	void OnJoinedRoom() {
	//	PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, Quaternion.identity, 0);
	}
}
