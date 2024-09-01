using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSlot : BaseUI
{
    [SerializeField] private TextMeshProUGUI RoomNameText;
    [SerializeField] private TextMeshProUGUI RoomPlayersText;
    [SerializeField] private Button JoinRoomButton;

    private string roomName;

    public void Initialize(string name, int currentPlayers)
    {
        roomName = name;

        RoomNameText.text = name;
        RoomPlayersText.text = currentPlayers.ToString();

        JoinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();

            PhotonNetwork.JoinRoom(roomName);
        });
    }
}
