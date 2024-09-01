using System;
using UnityEngine;

[Serializable]
public class PlayerAnimationData : BaseAnimationData
{
    [SerializeField] private string crazyParameterName = "Crazy";
    [SerializeField] private string deadParameterName = "TakeDamage";
    [SerializeField] private string openDoorParameterName = "Open";
    [SerializeField] private string escapeParameterName = "Escape";

    public int CrazyParameterHash {  get; private set; }
    public int DeadParameterHash { get; private set; }
    public int OpenDoorParameterHash { get; private set; }
    public int EscapeParameterHash { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        CrazyParameterHash = Animator.StringToHash(crazyParameterName);
        DeadParameterHash = Animator.StringToHash(deadParameterName);
        OpenDoorParameterHash = Animator.StringToHash(openDoorParameterName);
        EscapeParameterHash = Animator.StringToHash(escapeParameterName);
    }
}