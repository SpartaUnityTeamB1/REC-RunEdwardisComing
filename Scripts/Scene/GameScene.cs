using Cinemachine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class GameScene : BaseScene
{
    [SerializeField]
    private CinemachineFreeLook cinemachineCam;

    [SerializeField] private SpawnData pills;
    [SerializeField] private SpawnData firstKeys;
    [SerializeField] private SpawnData secondKeys;
    [SerializeField] private SpawnData players;
    [SerializeField] private SpawnData AIs;
    [SerializeField] private List<GameObject> checkPointList;

    [SerializeField] private Vector3 edwardPosition;
    [SerializeField] private Vector3 escapeKeyPosition;

    [SerializeField] private string spawnerTag;
    [SerializeField] private string escapeKeyTag;

    [SerializeField] private List<int> needKeyCount = new List<int>();

    private List<int> keyList = new List<int>();

    private GameObject playerObj;
    private GameObject spawnerObj;
    private Spawner spawner;

    private HashTable playerProps = new HashTable() { { "isReady", false } };

    private int maxPlayerNum;
    private int curDeadPlayerNum;
    private int curEscapePlayerNum;
    private int rand;

    protected override void Start()
    {
        base.Start();

        GameManager.Instance.AICheckPointList = checkPointList;

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        GameManager.Instance.SetSync(false);

        GameManager.Instance.ObjectPool.InitializeNetwork();

        InitializeGameScene();

        GameManager.Instance.PlayerDoll = null;

        RandomPlayer();

        if (null != GameManager.Instance.PlayerDoll)
            GameManager.Instance.PlayerDoll.GetCinemachineCamera(cinemachineCam);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnDead -= IncreaseDeadPlayer;
        GameManager.Instance.OnEscape -= IncreaseEscapePlayer;
        GameManager.Instance.OnCheckOpen -= CheckOpenDoor;
        GameManager.Instance.OnKeyIncrease -= IncreaseKeyCount;
    }

    private void RandomPlayer()
    {
        int idx = -1;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                idx = i;
                break;
            }
        }

        if (-1 == idx)
        {
            Debug.LogAssertion("Can't find ActorNumber");
            return;
        }

        int model;
        if (int.TryParse(GameManager.Instance.RandomIndex[idx].ToString(), out model))
        {
            if (1 == model)
            {
                playerObj = PhotonNetwork.Instantiate("Edward", edwardPosition, Quaternion.identity);
                //GameManager.Instance.AddPlayerList(playerObj);

                // 조준점 UI
                //GameManager.Instance.ShowUI<>(UI.Scene);
            }
            else
            {
                while (true)
                {
                    rand = Random.Range(0, players.positions.Count);

                    if (!GameManager.Instance.UsePositions[rand])
                        break;
                }

                playerObj = PhotonNetwork.Instantiate("Player", players.positions[rand], Quaternion.identity);
                //GameManager.Instance.AddPlayerList(playerObj);

                GameManager.Instance.SyncPlayerPositions(rand);

                GameManager.Instance.ShowUI<StressBarUI>(UI.Scene);
            }
        }
    }

    private void InitializeGameScene()
    {
        maxPlayerNum = PhotonNetwork.PlayerList.Length - 1;

        GameManager.Instance.ReadyPlayerPosition(players.positions.Count);

        curDeadPlayerNum = 0;
        curEscapePlayerNum = 0;

        for (KeyIndex i = KeyIndex.FirstKey; i < KeyIndex.KEYEND; ++i)
            keyList.Add(0);

        GameManager.Instance.OnDead += IncreaseDeadPlayer;
        GameManager.Instance.OnEscape += IncreaseEscapePlayer;
        GameManager.Instance.OnCheckOpen += CheckOpenDoor;
        GameManager.Instance.OnKeyIncrease += IncreaseKeyCount;

        if (PhotonNetwork.IsMasterClient)
        {
            spawnerObj = GameManager.Instance.ObjectPool.SpawnFromNetworkPool(spawnerTag);
            spawner = spawnerObj.GetComponent<Spawner>();

            spawner.Spawn(pills);
            spawner.Spawn(firstKeys);
            spawner.Spawn(secondKeys);
            spawner.Spawn(AIs);

            spawner.Spawn(escapeKeyPosition, escapeKeyTag);
        }
    }

    private void IncreaseDeadPlayer()
    {
        ++curDeadPlayerNum;

        Debug.Log($"max : {maxPlayerNum} dead : {curDeadPlayerNum} escape : {curEscapePlayerNum}");

        if (maxPlayerNum == (curDeadPlayerNum + curEscapePlayerNum))
        {
            if (0 < curEscapePlayerNum)
            {
                Cursor.lockState = CursorLockMode.Confined;

                // 게임 종료 UI (에드워드 패, 플레이어 승)
                GameManager.Instance.Winner = "Doll Win";
                GameManager.Instance.isGameOver = true;
                GameManager.Instance.ShowUI<PopupGameResultUI>(UI.Popup);
                
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;

                // 게임 종료 UI (에드워드 승, 플레이어 패)
                GameManager.Instance.Winner = "Edward Win";
                GameManager.Instance.isGameOver = true;
                GameManager.Instance.ShowUI<PopupGameResultUI>(UI.Popup);
            }
        }
    }

    private void IncreaseEscapePlayer()
    {
        ++curEscapePlayerNum;

        Debug.Log($"max : {maxPlayerNum} dead : {curDeadPlayerNum} escape : {curEscapePlayerNum}");

        if (maxPlayerNum == (curDeadPlayerNum + curEscapePlayerNum))
        {
            Cursor.lockState = CursorLockMode.Confined;

            // 게임 종료 UI (에드워드 패, 플레이어 승)
            GameManager.Instance.Winner = "Doll Win";
            GameManager.Instance.isGameOver = true;
            GameManager.Instance.ShowUI<PopupGameResultUI>(UI.Popup);
        }
    }

    private bool CheckOpenDoor(int index)
    {
        return keyList[index] == needKeyCount[index];
    }

    private void IncreaseKeyCount(int index)
    {
        ++keyList[index];
    }
}