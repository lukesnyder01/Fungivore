using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SporeShrine : MonoBehaviour, IDamageable
{
    [SerializeField] private float _health = 5f;

    public float Health { get; set; }

    public GameObject spawnPrefab;
    public GameObject spawnParticleEffect;
    public string spawnSound;


    void Start()
    {
        Health = _health;
        Debug.Log(Health);
    }


    public void Damage(float damage)
    {
        Health -= damage;

        Debug.Log(Health);

        if (Health <= 0)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation);
            Instantiate(spawnParticleEffect, transform.position, transform.rotation);
            FindObjectOfType<AudioManager>().Play(spawnSound);

            Destroy(this.gameObject);
        }
    }
}

