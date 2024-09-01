using Photon.Pun;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.CurrentScene = SceneIndex.Lobby;

        if (PhotonNetwork.InRoom)
            GameManager.Instance.ShowUI<RoomSceneUI>(UI.Scene);
        else
            GameManager.Instance.ShowUI<LobbySceneUI>(UI.Scene);

        Cursor.lockState = CursorLockMode.None;
        //Cursor.lockState = CursorLockMode.Confined;
    }
}
