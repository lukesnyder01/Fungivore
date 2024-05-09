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

            case State.Hunt:

                if (distanceFromPlayer > detectionRange)
                {
                    state = State.Idle;
                }


                if (shotTimer <= 0 && canSeePlayer)
                {
                    shotTimer = timeBetweenShots + Random.Range(0,0.1f);
                    ShootAtPlayer();
                }

                break;


            case State.Idle:

                if (distanceFromPlayer < detectionRange)
                {
                    state = State.Hunt;
                }

                break;

        }

    }

    public void ShootAtPlayer()
    {
        //Debug.Log("shot at player");
        var spawnLocation = transform.position + transform.forward;
        var newBullet = Instantiate(bulletPrefab, spawnLocation, transform.rotation);
        newBullet.velocity = (Vector3.up + Random.insideUnitSphere * aimRandomness) * bulletSpeed;

    }



}
