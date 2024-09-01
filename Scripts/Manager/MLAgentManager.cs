using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class MLAgentManager : Agent
{
    private float speed = 2.0f;
    [SerializeField] private List<GameObject> checkpoints;
    private HashSet<GameObject> visitedCheckpoints;
    private Rigidbody rb;
    private float reward = 200.0f;
    private float obstaclePenalty = -10.0f;
    // private float stepPenalty = -0.01f;
    // private float rotationPenalty = -0.02f;
    private int step;

    private Character character;
    private State state;
    private float stateChangeTimer;
    private float stateChangeInterval;

    public override void Initialize()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
        checkpoints = GameManager.Instance.AICheckPointList;
        visitedCheckpoints = new HashSet<GameObject>();
        stateChangeInterval = Random.Range(3f, 15f);
        stateChangeTimer = Time.time + stateChangeInterval;
    }

    public override void OnEpisodeBegin()
    {
        visitedCheckpoints.Clear();
        rb.velocity = rb.angularVelocity = Vector3.zero;
        step = MaxStep;
        stateChangeInterval = Random.Range(5f, 15f);
        stateChangeTimer = Time.time + stateChangeInterval;
        state = State.Idle;
        character.ChangeState(state);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.1f))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // 장애물이 가까울 때 음수 보상
                AddReward(-1.0f);
                EndEpisode();
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        step--;

        if (Time.time >= stateChangeTimer)
        {
            if (state == State.Move)
            {
                state = State.Idle;
            }
            else if (state == State.Idle)
            {
                state = State.Move;
            }
            stateChangeInterval = Random.Range(5f, 15f);
            stateChangeTimer = Time.time + stateChangeInterval;
            character.ChangeState(state);
        }

        Vector3 moveDir = Vector3.zero;
        // float rotationSpeed = 120.0f;
        // float smoothFactor = 0.1f;

        switch (state)
        {
            case State.Move:
                var actionDir = actions.DiscreteActions[0];
                var actionRot = actions.DiscreteActions[1];

                if (actionDir == 0) moveDir = transform.forward;

                switch (actionRot)
                {
                    case 0:
                        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * Time.fixedDeltaTime * 70.0f));
                        break;
                    case 1:
                        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * -(Time.fixedDeltaTime * 70.0f)));
                        break;
                }

                rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
                // AddReward(stepPenalty + rotationPenalty);
                break;

            // Idle 상태에서는 가만히 애니메이션만 작동되게 할라고
            case State.Idle:
                break;
        }

        if (transform.localPosition.y < -5.0f)
        {
            EndEpisode();
        }
    }

    //public override void OnActionReceived(ActionBuffers actions)
    //{
    //    step--;

    //    Vector3 moveDir = Vector3.zero;

    //    switch (state)
    //    {
    //        case State.Move:
    //            var actionDir = actions.DiscreteActions[0];
    //            var actionRot = actions.DiscreteActions[1];

    //            if (actionDir == 0) moveDir = transform.forward;

    //            switch (actionRot)
    //            {
    //                case 0:
    //                    rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * Time.fixedDeltaTime * 70.0f));
    //                    break;
    //                case 1:
    //                    rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * -(Time.fixedDeltaTime * 70.0f)));
    //                    break;
    //            }

    //            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    //            break;

    //        case State.Idle:
    //            break;
    //    }

    //    if (transform.localPosition.y < -5.0f)
    //    {
    //        EndEpisode();
    //    }
    //}


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            action[1] = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            action[1] = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            var checkpoint = other.gameObject;

            if (!visitedCheckpoints.Contains(checkpoint))
            {
                visitedCheckpoints.Add(checkpoint);
                AddReward(visitedCheckpoints.Count * reward);
            }

            if (visitedCheckpoints.Count == checkpoints.Count)
            {
                AddReward(1000.0f);
                EndEpisode();
            }
        }
        else if (other.CompareTag("Wall"))
        {
            AddReward(obstaclePenalty);
            EndEpisode();
        }
    }

}
