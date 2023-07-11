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
    public float accelerationSpeed = 1f;

    public float timeBeforeAccelerating = 4f;

    public GameObject deathEffect;


    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindWithTag("Player").transform;

        currentLifespan = bulletLifespan;

        AddRandomSpin();
    }


    private void Update()
    {
        currentLifespan -= Time.deltaTime;

        transform.forward = rb.velocity;

        if (currentLifespan <= 0)
        {
            Die();

        }

        if (bulletLifespan - currentLifespan > timeBeforeAccelerating)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.Normalize();

            //accelerate towards player
            rb.AddForce(directionToPlayer * accelerationSpeed);
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

                FindObjectOfType<AudioManager>().Play("WraithSmash");
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
