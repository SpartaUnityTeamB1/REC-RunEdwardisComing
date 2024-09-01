using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupGameResultUI : PopupUI
{
    [SerializeField]
    private TextMeshProUGUI winner;

    private void OnEnable()
    {
        winner.text = GameManager.Instance.Winner;
    }

    public void OnClickReturnRoom()
    {
        GameManager.Instance.GameEnd();
        GameManager.Instance.isGameOver = false;
    }

    public override void OnClickClose()
    {
    
    }
}
