using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWhenShot : MonoBehaviour
{
    public GameObject spawnPrefab;
    public GameObject spawnParticleEffect;
    public string spawnSound;

    private GameObject player;

    private bool hasBeenHit = false;


    void Start()
    {


    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PlayerBullet")
        {
            if (!hasBeenHit)
            {
                hasBeenHit = true;
                Instantiate(spawnPrefab, transform.position, transform.rotation);
                Instantiate(spawnParticleEffect, transform.position, transform.rotation);
                FindObjectOfType<AudioManager>().Play(spawnSound);
            }

            Destroy(this.gameObject);

        }
    }

}

