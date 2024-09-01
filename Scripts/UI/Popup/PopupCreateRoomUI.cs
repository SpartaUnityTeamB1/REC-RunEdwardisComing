using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCreateRoomUI : PopupUI
{
    [SerializeField] private TMP_InputField inputRoomName;

    private void Start()
    {
        inputRoomName.characterLimit = 8;
        inputRoomName.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void OnEnable()
    {
        inputRoomName.text = string.Empty;
    }

    private void OnInputValueChanged(string word)
    {
        string NameWord = Regex.Replace(word, @"[^0-9a-zA-Z가-힣]", "");

        if (inputRoomName.text != word)
        {
            inputRoomName.text = NameWord;
            inputRoomName.caretPosition = inputRoomName.text.Length;
        }
    }
    public override void OnClickClose()
    {
        GameManager.Instance.HideUI<PopupCreateRoomUI>();
    }

    public void OnClickCreateRoomBtn()
    {
        int rand = Random.Range(0, 3);
        GameManager.Instance.HideUI<PopupCreateRoomUI>();
        if(inputRoomName.text == null || inputRoomName.text.Length == 0)
        {
            switch (rand)
            {
                case 0:
                    inputRoomName.text = "쫄?";
                    break;
                case 1:
                    inputRoomName.text = "B1조는 반드시 이긴다.";
                    break;
                case 2:
                    inputRoomName.text = "예쁘게 봐주세요.";
                    break;
            }

        }
        LobbyManager.Instance.CreateRoom(inputRoomName.text);
    }
}
