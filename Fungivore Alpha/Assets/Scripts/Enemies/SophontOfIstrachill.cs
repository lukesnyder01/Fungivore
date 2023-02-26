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

        Spin(rotationSpeed);

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

                PointAtPlayer();

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
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }


    private void MoveTowardsTarget(float thrustMultiplier)
    {
        rb.AddForce(directionToTarget * thrust * thrustMultiplier);
    }

}
