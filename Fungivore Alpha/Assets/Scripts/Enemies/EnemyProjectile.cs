using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private bool collidedWithPlayer = false;
    public float damage;

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
