using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualManager : MonoBehaviour
{
    private int currentSeed;

    public GameObject player;

    public Transform spawnLocationsParent;

    public Transform[] spawnLocations;


    void Awake()
    {
        player = GameObject.Find("Player");
    }


    public void AddToCurrentSeed(int value)
    {
        currentSeed += Mathf.Abs(value);
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
