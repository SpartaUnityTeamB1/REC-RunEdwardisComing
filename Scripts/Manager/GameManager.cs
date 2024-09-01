using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = (GameManager)FindObjectOfType(typeof(GameManager));

                if (null == instance)
                {
                    GameObject obj = new GameObject(typeof(GameManager).Name, typeof(GameManager));
                    instance = obj.GetComponent<GameManager>();
                }
            }

            return instance;
        }
    }

    public ObjectPool ObjectPool { get; private set; }

    [SerializeField] private List<AudioMixerGroup> audioMixerGroups = new List<AudioMixerGroup>();
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private int queueCount = 0;

    private ResourcesManager resourcesMgr;
    private GamesceneManager sceneMgr;
    private SoundManager soundMgr;
    private UIManager uiMgr;

    public Action OnDead;
    public Action OnEscape;
    public Func<int, bool> OnCheckOpen;
    public Action<int> OnKeyIncrease;

    public SceneIndex CurrentScene { get; set; } = SceneIndex.SCENEEND;
    public SceneIndex NextScene { get; set; }
    public Doll PlayerDoll { get; set; }
    public string RandomIndex { get; private set; }
    public string Winner { get; set; }
    public bool OnHit { get; private set; } = false;
    public List<bool> UsePositions { get; private set; } = new List<bool>();
    public bool isGameOver { get; set; } = false;

    public List<GameObject> AICheckPointList { get; set; }
    public List<BaseCharacter> PlayerList { get; set; } = new List<BaseCharacter> { };

    private StringBuilder sb;

    private void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);

        resourcesMgr = new ResourcesManager();
        soundMgr = new SoundManager();
        uiMgr = new UIManager();
        sceneMgr = new GamesceneManager();
    }

    private void Start()
    {
        ObjectPool = GetComponent<ObjectPool>();

        soundMgr.InitSoundMgr(audioMixer, audioMixerGroups, queueCount);
        soundMgr.SettingVolumes();
    }

    public T InstantiateObject<T>(T obj) where T : UnityEngine.Object
    {
        return Instantiate(obj);
    }

    public void DontDestroy(GameObject obj)
    {
        DontDestroyOnLoad(obj);
    }

    public Coroutine CoroutineStart(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void CoroutineStop(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    public void MakePlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MakeRandom();
            photonView.RPC("SendRandom", RpcTarget.All, sb.ToString());
        }
    }

    private void MakeRandom()
    {
        sb = new StringBuilder();

        bool isOne = false;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if ((i + 1) == PhotonNetwork.PlayerList.Length)
            {
                if (!isOne)
                    sb.Append("1");
                else
                    sb.Append("0");
            }
            else
            {
                if (!isOne)
                {
                    int random = UnityEngine.Random.Range(0, 2);

                    if (1 == random)
                        isOne = true;

                    sb.Append(random.ToString());
                }
                else
                    sb.Append("0");
            }
        }
    }

    [PunRPC]
    public void SendRandom(string send)
    {
        RandomIndex = send;
    }

    [PunRPC]
    public void Clear()
    {
        ObjectPool.Clear();
        resourcesMgr.Clear();
    }

    public void SceneChange(BaseScene scene)
    {
        sceneMgr.SceneChange(scene);
    }

    public void GoToNextScene()
    {
        Clear();
        sceneMgr.GoToNextScene();
    }

    public void GoToPrevScene()
    {
        Clear();
        sceneMgr.GoToPrevScene();
    }

    public bool CheckUIInDic(string uiName)
    {
        return resourcesMgr.CheckUIInDic(uiName);
    }

    public BaseUI GetUIResource(string uiName)
    {
        return resourcesMgr.GetUIInDic(uiName);
    }

    public void AddUI(UI type, string name)
    {
        resourcesMgr.AddUIInDic(type, name);
    }

    public void ShowUI<T>(UI uiType)
    {
        uiMgr.ShowUI<T>(uiType);
    }

    public void HideUI<T>()
    {
        uiMgr.HideUI<T>();
    }

    public BaseUI GetUIByType<T>()
    {
        return uiMgr.GetUIByType<T>();
    }

    public void HitChange(bool isTrue)
    {
        photonView.RPC("ChangeHit", RpcTarget.All, isTrue);
    }

    [PunRPC]
    public void ChangeHit(bool isTrue)
    {
        OnHit = isTrue;
    }

    public void DollDead()
    {
        photonView.RPC("DeadPlayerNumIncrease", RpcTarget.All);
    }

    [PunRPC]
    public void DeadPlayerNumIncrease()
    {
        OnDead?.Invoke();
    }

    public void DollEscape()
    {
        photonView.RPC("EscapePlayerNumIncrease", RpcTarget.All);   
    }

    [PunRPC]
    public void EscapePlayerNumIncrease()
    {
        OnEscape?.Invoke();
    }

    public bool CheckOpenDoor(KeyIndex index)
    {
        return (bool)(OnCheckOpen?.Invoke((int)index));
    }

    public void IncreaseKey(int index)
    {
        photonView.RPC("IncreaseKeyCount", RpcTarget.All, index);
    }

    [PunRPC]
    private void IncreaseKeyCount(int index)
    {
        OnKeyIncrease?.Invoke(index);
    }

    public void GameStart()
    {
        photonView.RPC("Clear", RpcTarget.All);

        sceneMgr.GoToNextScene();
    }

    public void GameEnd()
    {
        Clear();

        //PhotonNetwork.CurrentRoom.IsOpen = true;
        //PhotonNetwork.CurrentRoom.IsVisible = true;

        //PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        PhotonNetwork.LeaveRoom();
        sceneMgr.GoToNextScene();
    }

    public void ReadyPlayerPosition(int count)
    {
        UsePositions.Clear();

        for (int i = 0; i < count; ++i)
            UsePositions.Add(false);
    }

    public void SyncPlayerPositions(int index)
    {
        photonView.RPC("UsePosition", RpcTarget.All, index);
    }

    [PunRPC]
    private void UsePosition(int index)
    {
        UsePositions[index] = true;
    }

    public void SetSync(bool isOn)
    {
        photonView.RPC("Sync", RpcTarget.All, isOn);
    }

    [PunRPC]
    private void Sync(bool isOn)
    {
        PhotonNetwork.AutomaticallySyncScene = isOn;
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        soundMgr.PlayBGM(bgmClip);
    }

    public void StopBGM()
    {
        soundMgr.StopBGM();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        soundMgr.PlaySFX(sfxClip);
    }

    public void SetBGMVolume(float volume)
    {
        soundMgr.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        soundMgr.SetSFXVolume(volume);
    }

    public float GetBGMVolume()
    {
        return soundMgr.GetBGMVolume();
    }

    public float GetSFXVolume()
    {
        return soundMgr.GetSFXVolume();
    }
}
