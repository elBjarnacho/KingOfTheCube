﻿using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    string[] names = new string[]
    {
        "Bjarni",
        "Henrique",
        "Sonia",
        "Rafa",
        "Julien"
    };

    public bool IsAr = false;
    public Canvas LobbyCanvas;
    public GameObject JoinGamePanel;
    public GameObject LaunchGamePanel;
    public GameObject CountdownTimer;
    public GameObject ReadyMessage;

    const string VERSION = "0.0.1";
    private string RoomName = "PrivateRoom";
    private string PlayerName = "Player";
    private bool HasJoinedRoom = false;
    private bool CountdownTimerActive = false;
    private byte MaxPlayerNumber = 5;
    private int LastKnownPlayerCount = -1;
    private float CountdowntimerValue = 10;

    void Start ()
    {
        Debug.Log("STARTING UP");
        
        PhotonNetwork.autoJoinLobby = true;
        //PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings(VERSION);

    }

    void Update ()
    {
        if (!HasJoinedRoom) return;

        int NumberOfPlayers = PhotonNetwork.room.PlayerCount;
        if(NumberOfPlayers > LastKnownPlayerCount)
        {
            string PlayerNames = "";
            foreach (PhotonPlayer Player in PhotonNetwork.playerList)
            {
                PlayerNames += names[Convert.ToInt32(Player.NickName)] + Player.NickName;
                PlayerNames += "\n";
            }
            GameObject.Find("PlayerList").GetComponent<Text>().text = PlayerNames;
            GameObject.FindGameObjectWithTag("NumberOfPlayersText").GetComponent<Text>().text = NumberOfPlayers.ToString();
            LastKnownPlayerCount = NumberOfPlayers;
        }

        if(!CountdownTimerActive)
        { 
            bool AllPlayersReady = true;
            foreach (PhotonPlayer Player in PhotonNetwork.playerList)
            {
                bool PlayerReady = (bool) Player.CustomProperties["Ready"];
                AllPlayersReady = AllPlayersReady && PlayerReady;
            }
            if(AllPlayersReady)
            {
                CountdowntimerValue = 10;
                CountdownTimerActive = true;
                CountdownTimer.GetComponent<Text>().text = "Starting in " + ((int)Math.Round(CountdowntimerValue)).ToString();
            }
        }
        else
        {
            CountdowntimerValue -= Time.deltaTime;
            CountdownTimer.GetComponent<Text>().text = "Starting in " + ((int)Math.Round(CountdowntimerValue)).ToString();
            if (CountdowntimerValue < 0.0f)
                LaunchGame();
        }
    }

    public void JoinGame()
    {
        JoinGamePanel.SetActive(false);
        LaunchGamePanel.SetActive(true);

        GetComponent<LobbyManager>().enabled = true;                
    }

    public void PlayerReady()
    {
        if (LastKnownPlayerCount < 2) return;

        ReadyMessage.GetComponent<Text>().text = "Ready, waiting for other players";
        ExitGames.Client.Photon.Hashtable PropertyTable = new ExitGames.Client.Photon.Hashtable();
        PropertyTable.Add("Ready", true);
        PhotonNetwork.player.SetCustomProperties(PropertyTable);
    }

    void LaunchGame()
    {
        //if (PhotonNetwork.isMasterClient)
            if(IsAr)
                PhotonNetwork.LoadLevel("AR_OnlineScene");
            else
                PhotonNetwork.LoadLevel("AssembleCube_AI_test");        
    }

    void OnJoinedLobby()
    {
        Debug.Log("JOINED LOBBY");

        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = MaxPlayerNumber };
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }
	
	void OnJoinedRoom()
    {
        Debug.Log("JOINING ROOM");
        HasJoinedRoom = true;

        string RoomName = PhotonNetwork.room.Name;
        int NumberOfPlayers = PhotonNetwork.room.PlayerCount;
        int PlayerIndex = NumberOfPlayers - 1;
        PhotonNetwork.player.NickName = PlayerIndex.ToString();

        ExitGames.Client.Photon.Hashtable PropertyTable = new ExitGames.Client.Photon.Hashtable();
        PropertyTable.Add("Ready", false);        
        PhotonNetwork.player.SetCustomProperties(PropertyTable);

        GameObject.FindGameObjectWithTag("RoomNameText").GetComponent<Text>().text = RoomName;
        GameObject.FindGameObjectWithTag("NumberOfPlayersText").GetComponent<Text>().text = NumberOfPlayers.ToString();
    }   
}
