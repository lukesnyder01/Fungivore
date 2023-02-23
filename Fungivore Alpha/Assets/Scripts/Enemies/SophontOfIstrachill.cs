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
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Calculate the angle between the object's forward vector and the direction to target
        float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, transform.up);

        // Adjust the target rotation to keep the object's z-axis rotation fixed
        Vector3 euler = targetRotation.eulerAngles;
        euler.z -= angleToTarget;
        targetRotation = Quaternion.Euler(euler);

        // Apply the modified target rotation to the object's rotation using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }


    private void MoveTowardsTarget(float thrustMultiplier)
    {
        rb.AddForce(directionToTarget * thrust * thrustMultiplier);
    }

}
