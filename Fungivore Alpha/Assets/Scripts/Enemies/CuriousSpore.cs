using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriousSpore : Enemy
{
    private Vector3 targetPosition;
    private Vector3 directionToTarget;

    public float targetDistance = 2;

    public override void UpdateStateMachine()
    {
        targetPosition = cameraTransform.forward * targetDistance + cameraTransform.position;
        directionToTarget = (targetPosition - transform.position).normalized;

        Spin();

        PointAtTarget();

        switch (state)
        {
            default:

            case State.Idle:

                Hover();
                FindOpenDirection();
                EvasiveMove(wanderSpeed);

                if (distanceFromPlayer < detectionRange)
                {
                    state = State.Hunt;
                }

                break;

            case State.Hunt:

                if (distanceFromPlayer > detectionRange)
                {
                    state = State.Idle;
                }

                if (CanSeePlayer())
                {
                    state = State.MaintainDistance;
                }
                else
                {
                    FindOpenDirection();
                    EvasiveMove(rapidEvasionSpeed);

                    if (distanceFromPlayer < minimumDistance)
                    {
                        EvadePlayer();
                    }
                    else
                    {
                        MoveTowardsTarget(chargeSpeed);
                    }
                }

                break;

            case State.MaintainDistance:

                if (distanceFromPlayer < minimumDistance)
                {
                    EvadePlayer();
                }
                else
                {
                    if (distanceFromPlayer > detectionRange)
                    {
                        state = State.Idle;
                    }

                    MoveTowardsTarget(chargeSpeed);
                }

                if (!CanSeePlayer())
                {
                    state = State.Hunt;
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
