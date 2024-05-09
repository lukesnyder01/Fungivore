using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private bool collidedWithPlayer = false;
    public float damage;
    public float bulletLifespan = 2f;
    private float currentLifespan;

    public Transform player;
    private CharacterController playerController;

    public GameObject deathEffect;
    private Rigidbody rb;

    public float accelerationSpeed = 1f;
    public float timeBeforeAccelerating = 4f;

    public float predictionTime = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<CharacterController>();

        currentLifespan = bulletLifespan;

        AddRandomSpin();
    }


    private void FixedUpdate()
    {
        currentLifespan -= Time.fixedDeltaTime;
        transform.forward = rb.velocity;

        if (currentLifespan <= 0)
        {
            Die();
            return;
        }

        if (bulletLifespan - currentLifespan > timeBeforeAccelerating)
        {
            Vector3 randomDeviation = new Vector3(
                Random.Range(-0.2f, 0.2f),
                Random.Range(-0.2f, 0.2f),
                Random.Range(-0.21f, 0.2f)
            );

            Vector3 predictedPlayerOffset = playerController.velocity * predictionTime;
            Vector3 predictedPlayerPosition = player.position + predictedPlayerOffset + randomDeviation;
            Vector3 directionToTarget = predictedPlayerPosition - transform.position;

            rb.AddForce(directionToTarget * accelerationSpeed);
        }
    }





    private void AddRandomSpin()
    {
        // Generate a random direction for the bullet to spin
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Apply initial spin to the bullet
        rb.angularVelocity = randomDirection * 10f;
    }



    public virtual void OnCollisionEnter(Collision other)
    {
        //Destroy(gameObject);
        //Instantiate(deathEffect, transform.position, transform.rotation);
        //FindObjectOfType<AudioManager>().Play("PlayerHurt");

        Die();

        if (other.gameObject.tag == "Player")
        {
           
            if (!collidedWithPlayer)
            {
                collidedWithPlayer = true;

                AudioManager.Instance.Play("WraithSmash");
                var playerStats = other.gameObject.GetComponent<PlayerStats>();
                playerStats.ApplyDamage(damage);
            }
        }

    }

    private void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }


}
