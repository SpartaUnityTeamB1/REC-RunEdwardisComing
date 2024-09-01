using UnityEngine;

public class EdwardStateMachine : StateMachine
{
    public Edward Edward { get; private set; }

    public Vector2 MovementInput { get; set; }
    public Vector2 RotateInput { get; set; }
    public bool AttackInput { get; set; }
    public float CamCurXRot { get; set; }

    public EdwardIdleState IdleState { get; private set; }
    public EdwardRunState RunState { get; private set; }
    public EdwardAttackState AttackState { get; private set; }

    public EdwardStateMachine(Edward edward)
    {
        Edward = edward;

        IdleState = new EdwardIdleState(this);
        RunState = new EdwardRunState(this);
        AttackState = new EdwardAttackState(this);
    }
}