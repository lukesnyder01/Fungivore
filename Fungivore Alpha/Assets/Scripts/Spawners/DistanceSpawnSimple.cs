using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSpawnSimple : MonoBehaviour
{
    public GameObject spawnPrefab;
    private GameObject player;

    public bool mustBeClicked;

    private float playerDistance;
    public float minPlayerDistance = 30;
    public float timeBetweenSteps = 1f;


    public bool destroyWhenFarAway;
    public float destroyDistance = 200;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if (!mustBeClicked) {
            StartCoroutine(CheckDistance());
        }
    }


    private IEnumerator CheckDistance()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenSteps);

        while (true) {

            playerDistance = ((transform.position - player.transform.position).sqrMagnitude); //find the square of the distance from player

            if (playerDistance < (minPlayerDistance * minPlayerDistance))     //square the minPlayerDistance because we're comparing square magnitudes
            {
                Instantiate(spawnPrefab, transform.position, transform.rotation);
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

