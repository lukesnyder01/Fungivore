using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSpawnFancy : MonoBehaviour
{
    public GameObject spawnPrefab;
    public GameObject spawnParticleEffect;
    public string spawnSound;

    private GameObject player;

    public bool mustBeClicked;

    private float playerDistance;
    public float minPlayerDistance = 5;
    public float timeBetweenSteps = 0.1f;

    public float destroyDistance = 200;

    public bool destroyWhenFarAway;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        StartCoroutine(CheckDistance());

    }


    private IEnumerator CheckDistance()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenSteps);

        while (true) {

            playerDistance = ((transform.position - player.transform.position).sqrMagnitude); //find the square of the distance from player

            if (playerDistance < (minPlayerDistance * minPlayerDistance))     //square the minPlayerDistance because we're comparing square magnitudes
            {
                Instantiate(spawnPrefab, transform.position, transform.rotation);

                if (spawnParticleEffect != null)
                {
                    Instantiate(spawnParticleEffect, transform.position, transform.rotation);
                }


                if (spawnSound != null)
                {
                    AudioManager.Instance.Play(spawnSound);
                }


                Destroy(this.gameObject);
            }

            if (destroyWhenFarAway)
            {
                if (playerDistance > (destroyDistance * destroyDistance))
                {
                    Destroy(this.gameObject);
                }
            }

            yield return wait;
        }

    }

}

