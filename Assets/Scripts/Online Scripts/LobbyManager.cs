﻿using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    string[] names = new string[]
    {
        "Pooria",
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

    [HideInInspector] public bool GameOver;

    const string VERSION = "0.0.1";
    private string RoomName = "PrivateRoom";
    private string PlayerName = "Player";
    private bool HasJoinedRoom = false;
    private bool CountdownTimerActive = false;
    private byte MaxPlayerNumber = 5;
    private int LastKnownPlayerCount = -1;
    private float CountdowntimerValue = 10;
    private long PlayerID;

    void Start ()
    {
        Debug.Log("STARTING UP");       

        ExitGames.Client.Photon.Hashtable PropertyTable = new ExitGames.Client.Photon.Hashtable();
        PropertyTable.Add(GameConstants.NetworkedProperties.Ready, false);
        PropertyTable.Add(GameConstants.NetworkedProperties.Stamp, null);
        PropertyTable.Add(GameConstants.NetworkedProperties.Inactive, false);
        PropertyTable.Add(GameConstants.NetworkedProperties.InGame, false);

        PhotonNetwork.player.SetCustomProperties(PropertyTable);
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings(VERSION);
    }

    void Update ()
    {
        if (!HasJoinedRoom) return;

        if (GameOver)
        { 
            ReloadLobby();
            return;
        }

        AssignPlayerIndices();

        if (!CountdownTimerActive)
        { 
            bool AllPlayersReady = true;
            foreach (PhotonPlayer Player in PhotonNetwork.playerList)
            {
                object check = Player.CustomProperties[GameConstants.NetworkedProperties.Ready];
                if (check == null) return;

                bool PlayerReady = (bool) Player.CustomProperties[GameConstants.NetworkedProperties.Ready];
                AllPlayersReady = AllPlayersReady && PlayerReady;
            }
            if(AllPlayersReady)
            {
                CountdowntimerValue = 10;
                CountdownTimerActive = true;
                CountdownTimer.GetComponent<Text>().text = "Starting in " + ((int)Math.Round(CountdowntimerValue)).ToString();
            }
        }
        else// Countdown timer active
        {
            CountdowntimerValue -= Time.deltaTime;
            CountdownTimer.GetComponent<Text>().text = "Starting in " + ((int)Math.Round(CountdowntimerValue)).ToString();
            if (CountdowntimerValue < 0.0f)
                LaunchGame();
        }
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = MaxPlayerNumber };
        bool success = PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
        if(!success)
        {
            Debug.Log("Failed to join room");
            return;
        }

        Debug.Log("Player joining game");
        JoinGamePanel.SetActive(false);
        LaunchGamePanel.SetActive(true);
    }

    public void PlayerReady()
    {
        if (LastKnownPlayerCount < 2) return;

        ReadyMessage.GetComponent<Text>().text = "Ready, waiting for other players";
        ExitGames.Client.Photon.Hashtable PropertyTable = new ExitGames.Client.Photon.Hashtable();
        PropertyTable.Add(GameConstants.NetworkedProperties.Ready, true);
        PropertyTable.Add(GameConstants.NetworkedProperties.Stamp, PlayerID);
        PropertyTable.Add(GameConstants.NetworkedProperties.Inactive, false);
        PhotonNetwork.player.SetCustomProperties(PropertyTable);
    }

    void LaunchGame()
    {
        ExitGames.Client.Photon.Hashtable PropertyTable = new ExitGames.Client.Photon.Hashtable();
        PropertyTable.Add(GameConstants.NetworkedProperties.InGame, true);
        PhotonNetwork.player.SetCustomProperties(PropertyTable);

        if (IsAr)
            PhotonNetwork.LoadLevel(GameConstants.SceneNames.OnlineAR);
        else
            PhotonNetwork.LoadLevel(GameConstants.SceneNames.OnlineVR);
    }

    Dictionary<long, PhotonPlayer> GetPlayerTimestampMap()
    {
        Dictionary<long, PhotonPlayer> res = new Dictionary<long, PhotonPlayer>();
        foreach (PhotonPlayer Player in PhotonNetwork.playerList)
        {
            //Will be null if other player hasn't had time to start up
            object check = Player.CustomProperties[GameConstants.NetworkedProperties.Stamp];
            if (check == null) return null;

            long Stamp = (long)Player.CustomProperties[GameConstants.NetworkedProperties.Stamp];
            res.Add(Stamp, Player);
        }
        return res;
    }

    void AssignPlayerIndices()
    {
        int NumberOfPlayers = PhotonNetwork.room.PlayerCount;

        if (NumberOfPlayers != LastKnownPlayerCount)
        {
            Dictionary<long, PhotonPlayer> PlayerTimestampMap = GetPlayerTimestampMap();
            if (PlayerTimestampMap == null) return;

            List<long> MapKeys = new List<long>(PlayerTimestampMap.Keys);
            MapKeys.Sort();

            int PlayerIndex = 0;
            string PlayerNames = "";
            foreach (long Key in MapKeys)
            {
                PhotonPlayer Player = PlayerTimestampMap[Key];
                if (Player.ID == PhotonNetwork.player.ID)
                {
                    //Static ID used to instantiate character in game scene
                    GameConstants.NetworkedPlayerID = PlayerIndex;
                }

                PlayerNames += names[Convert.ToInt32(PlayerIndex)] + PlayerIndex;
                PlayerNames += "\n";
                PlayerIndex++;
            }

            GameObject.Find(GameConstants.GameObjectsTags.playerListText).GetComponent<Text>().text = PlayerNames;
            GameObject.FindGameObjectWithTag(GameConstants.GameObjectsTags.playerNumText).GetComponent<Text>().text = NumberOfPlayers.ToString();
            LastKnownPlayerCount = NumberOfPlayers;
        }
    }

    void OnJoinedLobby()
    {
        Debug.Log("JOINED LOBBY");
    }
    
	void OnJoinedRoom()
    {
        Debug.Log("JOINING ROOM");

        HasJoinedRoom = true;
        PlayerID = DateTime.Now.Ticks;
        string RoomName = PhotonNetwork.room.Name;
        int NumberOfPlayers = PhotonNetwork.room.PlayerCount;
        
        ExitGames.Client.Photon.Hashtable PropertyTable = new ExitGames.Client.Photon.Hashtable();
        PropertyTable.Add(GameConstants.NetworkedProperties.Ready, false);
        PropertyTable.Add(GameConstants.NetworkedProperties.Stamp, PlayerID);
        PropertyTable.Add(GameConstants.NetworkedProperties.Inactive, false);
        PhotonNetwork.player.SetCustomProperties(PropertyTable);

        GameObject.FindGameObjectWithTag(GameConstants.GameObjectsTags.roomNameText).GetComponent<Text>().text = RoomName;
        GameObject.FindGameObjectWithTag(GameConstants.GameObjectsTags.playerNumText).GetComponent<Text>().text = NumberOfPlayers.ToString();
    }

    public void ReloadLobby()
    {
        //Have to wait until all players have exited game
        foreach(PhotonPlayer Player in PhotonNetwork.otherPlayers)
        {
            bool PlayerInGame = (bool)Player.CustomProperties[GameConstants.NetworkedProperties.InGame];
            if (PlayerInGame) return;
        }
        Debug.Log("Leaving room " + PhotonNetwork.countOfPlayers);
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        PhotonNetwork.LeaveRoom();
    }    

    public void OnLeftRoom()
    {
        Debug.Log("Loading lobby scene");
        PhotonNetwork.LoadLevel(GameConstants.SceneNames.Lobby);
    }
}
