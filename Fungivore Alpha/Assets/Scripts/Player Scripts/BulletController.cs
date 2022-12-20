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

    [HideInInspector]
    public bool hasCollided = false;

    public GameObject dummySpine;

    private Transform dummySpineParent;

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
        if (!hasCollided) //make sure the bullet can't double collide
        {
            var hitPos = other.contacts[0].point;

            dummySpineParent = other.gameObject.transform;

            hasCollided = true;

            if (other.gameObject.TryGetComponent(out IDamageable hit))
            {
                hit.Damage(bulletDamage);
            }

            var newDummySpine = Instantiate(dummySpine, hitPos, transform.rotation);
            if (other.collider.attachedRigidbody)
            {
                newDummySpine.transform.SetParent(dummySpineParent, true);
            }
    

            Instantiate(bulletImpact, hitPos, Quaternion.identity);

            Deactivate();
        }
    }




    void Deactivate()
    {
        currentLifespan = bulletLifespan;
        this.gameObject.SetActive(false);
    }



}
