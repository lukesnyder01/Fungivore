using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualManager : MonoBehaviour
{
    private int currentSeed;

    public GameObject player;

    public Transform spawnLocationsParent;

    public Transform[] spawnLocations;


    private int spikesToCompleteRitual = 3;
    private int spikesActivated = 0;


    void Awake()
    {
        player = GameObject.Find("Player");
    }


    public void AddToCurrentSeed(int value)
    {
        currentSeed += Mathf.Abs(value);

        RandomUtility.SetGlobalSeed(currentSeed);

        spikesActivated++;

        if (spikesActivated >= spikesToCompleteRitual)
        {
            spikesActivated = 0;

            CompleteTheRitual();
        }
    }



    public void CompleteTheRitual()
    {
        FindObjectOfType<GameManager>().LoadMainScene();
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var selectedSpawn = currentSeed % (spawnLocations.Length);

            var targetPosition = spawnLocations[selectedSpawn].position;
            var targetForward = spawnLocations[selectedSpawn].forward;

            player.GetComponent<CharacterController>().enabled = false;

            player.transform.position = targetPosition;
            player.transform.rotation = Quaternion.Euler(targetForward);

            player.GetComponent<CharacterController>().enabled = true;
        }
    }



}
