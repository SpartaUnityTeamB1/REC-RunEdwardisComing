using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : BaseUI
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private Image readyImg;

    public void Initialize(string playerName)
    {
        if(playerName == null || playerName.Length == 0)
        {
            playerName = "Guest";
            playerNameTxt.text = playerName;
        }
        else
        {
            playerNameTxt.text = playerName;
        }
    }

    public void OnReady(bool isReady)
    {
        readyImg.gameObject.SetActive(isReady);
    }
}
