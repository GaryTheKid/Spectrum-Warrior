using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject connectingText;
    public GameObject startButton;
    public GameObject loadingText;

    private void Awake()
    {
        lobby = this; //create the singleton
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connect to the master server
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        connectingText.SetActive(false);
        startButton.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Start!");

        startButton.SetActive(false);
        loadingText.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    // if no room exist
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    // create a new room
    void CreateRoom()
    {
        Debug.Log("Creating a new Room...");
        int randomRoomID = 1; //Random.Range(0, 10000);// create a new room with random name
        RoomOptions roomOption = new RoomOptions()
        { IsVisible = true, IsOpen = true, MaxPlayers = (byte)8 };
        PhotonNetwork.CreateRoom("Room" + randomRoomID, roomOption);
        Debug.Log(randomRoomID);
    }

    // if failed, try create again (in case of generating the same room id)
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom();
    }
}
