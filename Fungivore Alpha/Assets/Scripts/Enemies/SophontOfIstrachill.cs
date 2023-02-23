using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SophontOfIstrachill : Enemy
{
    private Vector3 targetPosition;
    private Vector3 directionToTarget;

    private Vector3 homePosition;

    public float targetDistance = 2;


    void Start()
    {
        homePosition = transform.position;
        targetPosition = homePosition;
    }

    public override void UpdateStateMachine()
    {
        directionToTarget = (targetPosition - transform.position).normalized;

        PointAtPlayer();

        Spin();

        switch (state)
        {
            default:

            case State.Idle:

                MoveTowardsTarget(wanderSpeed);

                if (distanceFromPlayer < detectionRange)
                {
                    state = State.MaintainDistance;
                }

                break;


            case State.Hunt:

                break;


            case State.MaintainDistance:

                if (distanceFromPlayer < minimumDistance)
                {
                    EvadePlayer();
                }
                else
                {
                    MoveTowardsTarget(chargeSpeed);

                    if (distanceFromPlayer > detectionRange)
                    {
                        state = State.Idle;
                    }
                }

                break;


            case State.Charge:

                break;
        }
    }


    private void PointAtTarget()
    {
        transform.forward = Vector3.Slerp(transform.forward, directionToTarget, Time.deltaTime * turnSpeed);
    }


    private void MoveTowardsTarget(float thrustMultiplier)
    {
        rb.AddForce(directionToTarget * thrust * thrustMultiplier);
    }

}
