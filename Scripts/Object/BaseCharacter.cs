using UnityEngine;

public abstract class BaseCharacter : BaseObject
{
    [field: SerializeField] public BaseStat Stat { get; private set; }
    [field: SerializeField] public GameObject LookPosition {  get; protected set; }

    public Animator Animator { get; private set; }

    

    protected virtual void Start()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    protected abstract void FixedUpdate();
    protected abstract void Update();
}