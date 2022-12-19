using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeShrine : InteractableObject, IDamageable
{
    [SerializeField] private float _health = 5f;

    public float Health { get; set; }

    public GameObject spawnPrefab;
    public GameObject spawnParticleEffect;
    public string spawnSound;


    void Start()
    {
        Health = _health;
    }

    public override void Interact()
    {
        Damage(1f);
    }


    public void Damage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation);
            Instantiate(spawnParticleEffect, transform.position, transform.rotation);
            FindObjectOfType<AudioManager>().Play(spawnSound);

            Destroy(this.gameObject);
        }
    }
}

