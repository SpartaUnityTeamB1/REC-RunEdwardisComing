using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseAnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string moveParameterName = "Move";

    public int IdleParameterHash { get; private set; }
    public int MoveParameterHash { get; private set; }

    public virtual void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        MoveParameterHash = Animator.StringToHash(moveParameterName);
    }
}
