using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private bool collidedWithPlayer = false;
    public float damage;
    public float bulletLifespan = 2f;
    private float currentLifespan;


    private void Start()
    {
        currentLifespan = bulletLifespan;

        // Generate a random direction for the bullet to spin
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Apply initial spin to the bullet
        GetComponent<Rigidbody>().angularVelocity = randomDirection * 10f;
    }

    private void FixedUpdate()
    {
        currentLifespan -= Time.deltaTime;

        if (currentLifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnCollisionEnter(Collision other)
    {
        //Destroy(gameObject);
        //Instantiate(deathEffect, transform.position, transform.rotation);
        //FindObjectOfType<AudioManager>().Play("PlayerHurt");

        Destroy(gameObject);

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
}
