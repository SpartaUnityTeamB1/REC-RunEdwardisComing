using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class Doll : BaseCharacter
{
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
    public Rigidbody RigidBody { get; private set; }
    public Camera Cam { get; private set; }
    public PlayerController PlayerController { get; private set; }

    public PlayerStateMachine stateMachine;
    public CinemachineFreeLook cineMachine;

    private float maxStress;
    private float curStress;
    private float decreseStressSpeed;

    public float CurStress { get { return curStress; } }
    public bool IsCrazy {  get; set; } = false;

    public bool IsObserve { get; set; } = false;

    public int index;

    private void Awake()
    {
        if (!photonView.IsMine)
            return;

        GameManager.Instance.PlayerDoll = this;
    }

    protected override void Start()
    {
        if (!photonView.IsMine)
            return;

        base.Start();

        photonView.RPC("SetPlayerList", RpcTarget.All);

        PlayerController = GetComponent<PlayerController>();


        Cam = Camera.main;

        RigidBody = GetComponent<Rigidbody>();

        AnimationData.Initialize();

        stateMachine = new PlayerStateMachine(this);
        stateMachine.ChangeState(stateMachine.IdleState);

        if(Stat is PlayerStat playerStat)
        {
            maxStress = playerStat.stress;
            decreseStressSpeed = playerStat.stressDecreaseSpeed;
        }
        curStress = maxStress;

        StartCoroutine(DecreaseStress());
    }


    protected override void FixedUpdate()
    {
        if(!photonView.IsMine) 
            return;

        stateMachine?.PhysicsUpdate();
    }

    protected override void Update()
    {
        if(!photonView.IsMine)
            return;

        stateMachine?.HandleInput();

        stateMachine?.Update();
    }

    public void HealStress()
    {
        curStress = maxStress;
    }

    IEnumerator DecreaseStress()
    {
        while(true)
        { 
            curStress -= decreseStressSpeed * Time.deltaTime;

            if(curStress < 0)
            {
                IsCrazy = true;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (IsLayerMatched(targetLayer.value, other.gameObject.layer) && GameManager.Instance.OnHit && !IsObserve)
        {
            transform.LookAt(other.transform.position);
            stateMachine.ChangeState(stateMachine.DeadState);
            GameManager.Instance.PlayerList.RemoveAt(index);

            IsObserve = true;

            curStress = 0;

            GameManager.Instance.DollDead();
        }
    }

    public void GetCinemachineCamera(CinemachineFreeLook cam)
    {
        cineMachine = cam;

        cineMachine.Follow = transform;
        cineMachine.LookAt = LookPosition.transform;
        ++cineMachine.Priority;
    }

    public void OpenDoor()
    {
        stateMachine.ChangeState(stateMachine.OpenDoorState);
    }

    public void CancelOpenDoor()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    [PunRPC]
    private void SetPlayerList()
    {
        index = GameManager.Instance.PlayerList.Count;
        GameManager.Instance.PlayerList.Add(this);
    }
}