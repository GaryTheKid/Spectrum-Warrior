using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // Room info
    public static PhotonRoom room;

    // Game
    public int currentScene;

    // PV
    public PhotonView PV;

    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are in " + PhotonNetwork.CurrentRoom.Name + " now!");
        PhotonNetwork.NickName = PhotonNetwork.PlayerList.Length.ToString();

        // check if the room is full
        if (PhotonNetwork.CurrentRoom.IsOpen && PhotonNetwork.PlayerList.Length >= MultiplayerSetting.multiplayerSetting.maxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Debug.Log("Room is full now!");
        }

        StartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room");
    }

    public void StartGame()
    {
        // load on Master client
        if (!PhotonNetwork.IsMasterClient)
            return;

        // load level
        PhotonNetwork.LoadLevel(MultiplayerSetting.multiplayerSetting.multiplayerScene);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mod)
    {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSetting.multiplayerSetting.multiplayerScene)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        Debug.Log("Spawn Player");
        PhotonNetwork.Instantiate(Path.Combine("Player", "Player"), new Vector3(10.5f, -6.8f, -29.5f), Quaternion.identity, 0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + "has left the game");
        if (!PhotonNetwork.CurrentRoom.IsOpen && PhotonNetwork.PlayerList.Length < MultiplayerSetting.multiplayerSetting.maxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            Debug.Log("Room is avaliable now!");
        }
    }
}
