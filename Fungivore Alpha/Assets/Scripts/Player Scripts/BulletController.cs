using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float gravity;
    private Rigidbody rb;
    public GameObject bulletImpact;
    float shotTimer;
    public float bulletLifespan = 2f;
    float currentLifespan;
    public PlayerStats playerStats;
    public float bulletDamage = 1f;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentLifespan = bulletLifespan;
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }


    void FixedUpdate()
    {
        currentLifespan -= Time.deltaTime;

        if (currentLifespan <= 0)
        {
            Deactivate();
        }

        shotTimer += Time.deltaTime;

        rb.AddForce(-Vector3.up * gravity);
        transform.forward = rb.velocity;

    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable hit))
        {
            hit.Damage(bulletDamage);
        }

        
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "NearDistanceTrigger")
        {
            Instantiate(bulletImpact, transform.position, Quaternion.identity);

            Deactivate();
        }

    }


    void Deactivate()
    {
        currentLifespan = bulletLifespan;
        this.gameObject.SetActive(false);
    }



}
