﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    private int playerNumber = 1;
    const string VERSION = "0.0.1";
    public string roomName = "myRoom";
    public GameObject player;
    public GameObject garden;
    //public string playerPrefabName = "character";

	// Use this for initialization
	void Start () {
        Debug.Log("STARTING UP");
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.autoJoinLobby = true;
	}

    void OnJoinedLobby()
    {
        Debug.Log("JOINED LOBBY");
        RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 5 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
	
	void OnJoinedRoom()
    {
        Debug.Log("JOINING ROOM");

        //TODO Remove the code below when running AR 
        int numberOfPlayers = PhotonNetwork.countOfPlayers;
        Vector3 spawn = GameObject.FindWithTag("Cube").GetComponent<LevelInstatiator>().instantiateSpawnPoint(numberOfPlayers);

        var newPlayer = PhotonNetwork.Instantiate("UnityPlayer", Vector3.zero, Quaternion.identity, 0);
        newPlayer.transform.SetParent(GameObject.Find("Wrapper").transform, false);
        newPlayer.transform.localPosition = spawn;
        
        newPlayer.GetComponent<Rigidbody>().useGravity = true;
        PlayerController_AssemCube c = newPlayer.GetComponent<PlayerController_AssemCube>();
        c.enabled = true;                
    }

}