using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edward : BaseCharacter
{
    [field: SerializeField] public EdwardAnimationData AnimationData { get; private set; }
    [field: SerializeField] public Transform CameraPos { get; set; }
    [field: SerializeField] public float MinXLook { get; private set; }
    [field: SerializeField] public float MaxXLook { get; private set; }

    [field: SerializeField] private string cullingLayerName;

    public EdwardController PlayerController { get; private set; }

    private EdwardStateMachine stateMachine;
    private EdwardStat stat;

    public bool CanAttack { get; private set; } = true;

    protected override void Start()
    {
        if (!photonView.IsMine)
            return;

        base.Start();

        photonView.RPC("SetPlayerList", RpcTarget.All);

        PlayerController = GetComponent<EdwardController>();

        Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer(cullingLayerName));

        AnimationData.Initialize();

        if (Stat is EdwardStat)
            stat = (EdwardStat)Stat;

        stateMachine = new EdwardStateMachine(this);

        stateMachine.ChangeState(stateMachine.IdleState);
    }

    protected override void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        stateMachine?.PhysicsUpdate();
    }

    protected override void Update()
    {
        if (!photonView.IsMine)
            return;

        stateMachine?.HandleInput();

        stateMachine?.Update();
    }

    public void CantAttack()
    {
        StartCoroutine(DelayAttack(stat.delay));
    }

    private IEnumerator DelayAttack(float delay)
    {
        CanAttack = false;

        yield return new WaitForSeconds(delay);

        CanAttack = true;
    }

    [PunRPC]
    private void SetPlayerList()
    {
        GameManager.Instance.PlayerList.Add(this);
    }
}
