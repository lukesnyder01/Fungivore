﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetheredEnemy : Enemy
{
    private Vector3 targetPosition;
    private Vector3 dirToTether;

    private float distFromTether;

    public Transform tether;

    private Vector3 tetherPosition;

    public float maxDistFromTether = 2;


    void Start()
    {
        tetherPosition = tether.position;
        targetPosition = tetherPosition;
        transform.parent = transform.parent.parent;
    }

    public override void UpdateStateMachine()
    {

        targetPosition = tether.position;

        dirToTether = (targetPosition - transform.position).normalized;
        distFromTether = DistanceFromTarget();

        MoveTowardsTarget(wanderSpeed * distFromTether);

        Spin(rotationSpeed);
        PointAtPlayer();

        switch (state)
        {
            default:

            case State.Idle:

                if (distanceFromPlayer < detectionRange)
                {
                    state = State.MaintainDistance;
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
                        MoveTowardsTarget(rapidEvasionSpeed);
                    }
                }

                break;


            case State.MaintainDistance:


                if (!CanSeePlayer())
                {
                    state = State.Hunt;
                }

                if (distanceFromPlayer < minimumDistance)
                {
                    EvadePlayer();
                }
                else
                {
                    if (distFromTether < maxDistFromTether)
                    {
                        MoveTowardsPlayer(wanderSpeed);
                    }
                    else
                    {
                        MoveTowardsTarget(rapidEvasionSpeed);
                    }

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
        Quaternion targetRotation = Quaternion.LookRotation(dirToTether, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }


    private void MoveTowardsTarget(float thrustMultiplier)
    {
        rb.AddForce(dirToTether * thrust * thrustMultiplier);
    }

    private float DistanceFromTarget()
    {
        return Vector3.Distance(transform.position, tetherPosition);
    }

}
