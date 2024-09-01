using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class RoomSceneUI : SceneUI
{
    [SerializeField] private GameObject playerListView;
    [SerializeField] public GameObject startBtn;

    private Dictionary<int, GameObject> slots = new Dictionary<int, GameObject>();
    private GameObject playerSlot;

    private HashTable initialProps = new HashTable() { { "isReady", false } };

    private bool isReady;

    private void OnEnable()
    {
        ShowPlayers(PhotonNetwork.PlayerList);
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        isReady = false;
    }

    //public void ShowPlayers(Player[] players)
    //{
    //    foreach (Player player in players)
    //    {
    //        playerSlot = GameManager.Instance.ObjectPool.SpawnFromPool("PlayerSlot");
    //        if(player.NickName == null || player.NickName.Length == 0)
    //        {
    //            player.NickName = "Guest";
    //        }
    //        playerSlot.GetComponent<PlayerSlot>().Initialize(player.NickName);

    //        object isPlayerReady;
    //        if (player.CustomProperties.TryGetValue("isReady", out isPlayerReady))
    //        {
    //            playerSlot.GetComponent<PlayerSlot>().OnReady((bool)isPlayerReady);
    //        }
    //        playerSlot.transform.SetParent(playerListView.transform);
    //        slots.Add(player.ActorNumber, playerSlot);
    //    }
    //}

    [SerializeField] private GameObject slotPrefab;
    public void ShowPlayers(Player[] players)
    {
        foreach (Player player in players)
        {
            playerSlot = Instantiate(slotPrefab, playerListView.transform);
            //GameManager.Instance.ObjectPool.SpawnFromPool("PlayerSlot");
            playerSlot.GetComponent<PlayerSlot>().Initialize(player.NickName);

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue("isReady", out isPlayerReady))
            {
                playerSlot.GetComponent<PlayerSlot>().OnReady((bool)isPlayerReady);
            }
            //playerSlot.transform.SetParent(playerListView.transform);
            slots.Add(player.ActorNumber, playerSlot);
        }
    }

    public void ClearPlayers()
    {
        foreach (GameObject obj in slots.Values)
        {
            obj.SetActive(false);
            obj.transform.SetParent(GameManager.Instance.transform);
        }

        slots.Clear();
    }

    public void OnClickBack()
    {
        PhotonNetwork.LeaveRoom();

        GameManager.Instance.HideUI<RoomSceneUI>();
        GameManager.Instance.ShowUI<LobbySceneUI>(UI.Scene);
    }

    public void OnClickStart()
    {
        //GameManager.Instance.SetSync(true);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            GameManager.Instance.MakePlayers();

            GameManager.Instance.GameStart();
        }
    }

    public void OnclickReady()
    {
        isReady = !isReady;
        HashTable prop = new HashTable() { { "isReady", isReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnDisable()
    {
        ClearPlayers();
    }


    public void PlayerReadyUpdate(Player targetPlayer, HashTable changedProps)
    {
        GameObject entry = slots[targetPlayer.ActorNumber];

        object isPlayerReady;
        if (changedProps.TryGetValue("isReady", out isPlayerReady))
        {
            entry.GetComponent<PlayerSlot>().OnReady((bool)isPlayerReady);
        }
    }
}