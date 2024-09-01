using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Doll Doll {  get; private set; }

    public Vector2 MovementInput { get; set; }

    public PlayerIdleState IdleState { get; private set; }

    public PlayerMoveState MoveState { get; private set; }

    public PlayerCrazyState CrazyState { get; private set; }
    
    public PlayerDeadState DeadState { get; private set; }

    public PlayerOpenDoorState OpenDoorState { get; private set; }

    public PlayerEscapeState EscapeState { get; private set; }

    public PlayerStateMachine(Doll player)
    {
        Doll = player;

        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        CrazyState = new PlayerCrazyState(this);
        DeadState = new PlayerDeadState(this);
        OpenDoorState = new PlayerOpenDoorState(this);
        EscapeState = new PlayerEscapeState(this);
    }
}