using ExitGames.Client.Photon.Encryption;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private static LobbyManager instance;

    public static LobbyManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = (LobbyManager)FindObjectOfType(typeof(LobbyManager));

                if (null == instance)
                {
                    GameObject obj = new GameObject(typeof(LobbyManager).Name, typeof(LobbyManager));
                    instance = obj.GetComponent<LobbyManager>();
                }
            }

            return instance;
        }
    }

    private int MAX_USERS = 4;
    private bool connect = false;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private List<GameObject> roomSlotList;

    public GameObject roomListViewPort;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomSlotList = new List<GameObject>();
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Server");
        connect = true;

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        ClearRoomList();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomList();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Create Room Failed : {message}");

        GameManager.Instance.HideUI<PopupCreateRoomUI>();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Room Failed : {message}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom($"Room {Random.Range(1, 1000)}", new RoomOptions { MaxPlayers = MAX_USERS }, null);
    }

    public override void OnJoinedRoom()
    {
        cachedRoomList.Clear();

        GameManager.Instance.HideUI<LobbySceneUI>();
        GameManager.Instance.ShowUI<RoomSceneUI>(UI.Scene);

        //int currentPlayers = PhotonNetwork.CurrentRoom.Players.Count;

        //if (currentPlayers == PhotonNetwork.CurrentRoom.MaxPlayers)
        //    PhotonNetwork.LoadLevel(1);
        //else
        //{
        //    Debug.Log(currentPlayers + " / " + PhotonNetwork.CurrentRoom.MaxPlayers + " players joined. Waiting for more...");
        //}
    }

    public override void OnLeftRoom()
    {
        GameManager.Instance.ObjectPool.ReleaseNetworkObjects();

        // 다운캐스팅
        RoomSceneUI roomUI = (RoomSceneUI)GameManager.Instance.GetUIByType<RoomSceneUI>();

        if (null != roomUI)
        {
            GameManager.Instance.HideUI<RoomSceneUI>();
            roomUI.ClearPlayers();
        }
        
        GameManager.Instance.ShowUI<LobbySceneUI>(UI.Scene);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //PhotonNetwork.CurrentRoom.IsOpen = false;
        //PhotonNetwork.LoadLevel(1);

        //int currentPlayers = PhotonNetwork.CurrentRoom.Players.Count;

        //if (currentPlayers == PhotonNetwork.CurrentRoom.MaxPlayers)
        //    PhotonNetwork.LoadLevel(1);

        // 다운캐스팅
        RoomSceneUI roomUI = (RoomSceneUI)GameManager.Instance.GetUIByType<RoomSceneUI>();

        roomUI.ClearPlayers();

        roomUI.ShowPlayers(PhotonNetwork.PlayerList);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomSceneUI roomUI = (RoomSceneUI)GameManager.Instance.GetUIByType<RoomSceneUI>();

        roomUI.ClearPlayers();

        roomUI.ShowPlayers(PhotonNetwork.PlayerList);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, HashTable changedProps)
    {
        RoomSceneUI roomUI = (RoomSceneUI)GameManager.Instance.GetUIByType<RoomSceneUI>();

        if (null == roomUI)
        {
            GameManager.Instance.AddUI(UI.Scene, typeof(RoomSceneUI).Name);
            roomUI = (RoomSceneUI)GameManager.Instance.GetUIByType<RoomSceneUI>();
        }

        roomUI.PlayerReadyUpdate(targetPlayer, changedProps);

        if (PhotonNetwork.IsMasterClient)
        {
            roomUI.startBtn.SetActive(CheckPlayersReady());
        }
    }

    public void JoinRoom()
    {
        if (connect)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Entered");
        }
        else
        {
            Debug.LogWarning("Not Connected");
        }
    }

    //private void ClearPlayerList()
    //{
    //    // 다운캐스팅
    //    RoomSceneUI roomUI = (RoomSceneUI)GameManager.Instance.GetUIByType<RoomSceneUI>();

    //    roomUI.ClearPlayers();
    //}

    public void CreateRoom(string roomName)
    {
        RoomOptions option = new RoomOptions { MaxPlayers = MAX_USERS };
        PhotonNetwork.CreateRoom(roomName,option);
    }
    
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(info.Name))
                    cachedRoomList.Remove(info.Name);

                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
                cachedRoomList[info.Name] = info;
            else
                cachedRoomList.Add(info.Name, info);
        }
    }

    private void UpdateRoomListView()
    {
        LobbySceneUI lobbyUI = (LobbySceneUI)GameManager.Instance.GetUIByType<LobbySceneUI>();

        lobbyUI.ShowRoomList(cachedRoomList);
    }

    private void ClearRoomList()
    {
        LobbySceneUI lobbyUI = (LobbySceneUI)GameManager.Instance.GetUIByType<LobbySceneUI>();

        lobbyUI.ClearRoom();
    }

    public void LocalPlayerPropertiesUpdated()
    {

    }

    private bool CheckPlayersReady()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;

            if (p.CustomProperties.TryGetValue("isReady", out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                    return false;
            }
        }

        return true;
    }

    public void Disconnect()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
        connect = false;
    }
}