using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StressBarUI : SceneUI
{
    [SerializeField] private Image fill;

    [SerializeField] private List<TextMeshProUGUI> keyList;
    private List<int> keyNum = new List<int>();

    private void OnEnable()
    {
        GameManager.Instance.OnKeyIncrease += UpdateCurGetKeyNum;

        if (1 > keyNum.Count)
        {
            for (KeyIndex i = KeyIndex.FirstKey; i < KeyIndex.KEYEND; ++i)
                keyNum.Add(0);
        }
        else
        {
            for (int i = 0; i < (int)KeyIndex.KEYEND; ++i)
                keyNum[i] = 0;
        }
    }

    private void Update()
    {
        if(GameManager.Instance.PlayerDoll != null)
            fill.fillAmount = GameManager.Instance.PlayerDoll.CurStress / 100f;
    }

    private void UpdateCurGetKeyNum(int key)
    {
        keyNum[key]++;

        for(int i = 0; i < (int)KeyIndex.KEYEND; i++)
        {
            keyList[i].text = keyNum[i].ToString();
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.OnKeyIncrease -= UpdateCurGetKeyNum;
    }
}


