using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailSegment : Enemy
{
    public Transform followTransform;

    public float followDistance;

    private Vector3 followPosition;
    private float currentDistance;

    public int listPosition = 0;
    public TailManager tailManager;


    void Start()
    {
        transform.SetParent(null);
    }


    public override void FixedUpdate()
    {

        if (followTransform != null)
        {
            // A point in space behind the follow target
            Vector3 targetPos = followTransform.position - followTransform.forward * followDistance;

            currentDistance = Vector3.Distance(transform.position, targetPos);

            //transform.LookAt(targetPos, transform.up);

            Vector3 directionToTarget = (targetPos - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);


            if (currentDistance > 0)
            {
                var moveForce = currentDistance * thrust;

                if (moveForce > thrust * 4f)
                {
                    moveForce = thrust * 4f;
                }

                rb.AddForce(transform.forward * moveForce);
                //rb.AddForce(transform.right * Random.Range(-0.3f, 0.3f));
            }
        }
        else 
        {
            MoveTowardsPlayer(thrust);
        }

    }

    public override void Die()
    {
        tailManager.RemoveFromList(listPosition);

        Destroy(gameObject);

        Instantiate(deathEffect, transform.position, transform.rotation);

        AudioManager.Instance.Play("PlayerHurt");

    }



}
