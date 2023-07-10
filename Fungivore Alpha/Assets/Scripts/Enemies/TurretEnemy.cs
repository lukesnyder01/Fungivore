using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : Enemy
{
    public float timeBetweenShots;
    public float bulletSpeed;
    public float aimRandomness = 0.1f;
    private float shotTimer = 0;


    public Rigidbody bulletPrefab;


    public override void UpdateStateMachine()
    {
        PointAtPlayer();

        shotTimer -= Time.deltaTime;


        Spin(Random.Range(-rotationSpeed, rotationSpeed));


        switch (state)
        {
            default:

            case State.Idle:

                if (distanceFromPlayer < detectionRange)
                {
                    state = State.Hunt;
                }

                break;


            case State.Hunt:

                if (distanceFromPlayer > detectionRange || !canSeePlayer)
                {
                    state = State.Idle;
                }


                if (shotTimer <= 0)
                {
                    shotTimer = timeBetweenShots;
                    ShootAtPlayer();
                }

                break;

        }

    }

    public void ShootAtPlayer()
    {
        //Debug.Log("shot at player");
        var newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        newBullet.velocity = (transform.forward + Random.insideUnitSphere * aimRandomness) * bulletSpeed;

    }



}
