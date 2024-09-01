using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneUI : SceneUI
{
    [SerializeField] private TMP_InputField playerNickName;
    [SerializeField] private GameObject roomListView;

    private List<GameObject> rooms = new List<GameObject>();
    private GameObject roomSlot;

    private void Start()
    {
        LobbyManager.Instance.roomListViewPort = roomListView;
        playerNickName.characterLimit = 8;
        playerNickName.onValueChanged.AddListener(OnInputValueChanged);
    }

    //public void ShowRoomList(Dictionary<string, RoomInfo> cachedRoom)
    //{
    //    foreach (RoomInfo info in cachedRoom.Values)
    //    {
    //        roomSlot = GameManager.Instance.ObjectPool.SpawnFromPool("RoomSlot");
    //        roomSlot.GetComponent<RoomSlot>().Initialize(info.Name, (int)info.PlayerCount);
    //        roomSlot.transform.SetParent(roomListView.transform);
    //        rooms.Add(roomSlot);
    //    }
    //}

    [SerializeField] private GameObject slotPrefab;

    public void ShowRoomList(Dictionary<string, RoomInfo> cachedRoom)
    {
        foreach (RoomInfo info in cachedRoom.Values)
        {
            roomSlot = Instantiate(slotPrefab, roomListView.transform);
            roomSlot.GetComponent<RoomSlot>().Initialize(info.Name, (int)info.PlayerCount);
            rooms.Add(roomSlot);
        }
    }

    public void ClearRoom()
    {
        foreach (GameObject room in rooms)
        {
            room.SetActive(false);
            room.transform.SetParent(GameManager.Instance.transform);
        }

        rooms.Clear();
    }

    public void OnClickCreateRoom()
    {
        GameManager.Instance.ShowUI<PopupCreateRoomUI>(UI.Popup);
    }

    public void OnClickBack()
    {
        LobbyManager.Instance.Disconnect();

        GameManager.Instance.GoToPrevScene();
    }

    public void OnClickQuickMacth()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

        PhotonNetwork.JoinRandomRoom();
    }

    private void OnInputValueChanged(string word)
    {
        string NameWord = Regex.Replace(word, @"[^0-9a-zA-Z가-힣]", "");

        if (playerNickName.text != word)
        {
            playerNickName.text = NameWord;
            playerNickName.caretPosition = playerNickName.text.Length;
        }
    }
    public void OnNameTextValueChange()
    {
        if(playerNickName.text.Length > 0)
        {
            PhotonNetwork.LocalPlayer.NickName = playerNickName.text;
        }
        else
        {
            playerNickName.text = "Guest";
            PhotonNetwork.LocalPlayer.NickName = playerNickName.text;
        }

    }
}