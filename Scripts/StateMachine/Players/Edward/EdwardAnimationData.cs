using System;
using UnityEngine;

[Serializable]
public class EdwardAnimationData : BaseAnimationData
{
    [SerializeField] private string attackParameterName = "Attack";
    [field: SerializeField] public float AttackExitTime { get; private set; }

    public int AttackParameterHash { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        AttackParameterHash = Animator.StringToHash(attackParameterName);
    }
}