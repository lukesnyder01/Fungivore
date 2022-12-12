using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViperWraithBodySegment : Enemy
{
    public Transform followTransform;

    public float minFollowDistance;

    private Vector3 followPosition;
    private float currentDistance;

    public int listPosition = 0;
    public ViperWraithTailManager tailManager;


    public override void FixedUpdate()
    {
        Spin();

        if (followTransform != null)
        {
            followPosition = followTransform.position;

            currentDistance = Vector3.Distance(transform.position, followTransform.position);

            transform.LookAt(followPosition, transform.up);

            if (currentDistance > minFollowDistance)
            {
                var moveForce = 3 * currentDistance * thrust;

                if (moveForce > thrust * 2f)
                {
                    moveForce = thrust * 2f;
                }

                rb.AddForce(transform.forward * moveForce);
                rb.AddForce(transform.right * Random.Range(-0.3f, 0.3f));
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

        FindObjectOfType<AudioManager>().Play("PlayerHurt");

    }



}
