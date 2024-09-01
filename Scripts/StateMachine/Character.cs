using System.Collections;
using UnityEngine;

public enum State
{
    Idle,
    Move,
    Attack,
    TakeDamage,
    Crazy
}

public class Character : BaseObject
{
    private Animator anim;

    private string idleParameterName = "Idle";
    private string moveParameterName = "Move";
    private string attackParameterName = "Attack";
    private string takeDamageParameterName = "TakeDamage";
    private string crazyParameterName = "Crazy";

    public State state { get; private set; }

    void Start()
    {
        anim = GetComponent<Animator>();
        state = State.Idle;
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            anim.SetBool(idleParameterName, false);
            anim.SetBool(moveParameterName, false);
            anim.SetBool(attackParameterName, false);
            anim.SetBool(takeDamageParameterName, false);
            anim.SetBool(crazyParameterName, false);

            switch (state)
            {
                case State.Idle:
                    anim.SetBool(idleParameterName, true);
                    break;

                case State.Move:
                    anim.SetBool(moveParameterName, true);
                    break;

                case State.Attack:
                    anim.SetBool(attackParameterName, true);
                    break;

                case State.TakeDamage:
                    anim.SetBool(takeDamageParameterName, true);
                    break;

                case State.Crazy:
                    anim.SetBool(crazyParameterName, true);
                    break;
            }

            yield return null;
        }
    }

    public void ChangeState(State newState)
    {
        if (state != newState)
        {
            state = newState;
        }
    }
}
