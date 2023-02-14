using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualGate : MonoBehaviour
{
    public GameObject bloodSpatter;
    public GameObject gateTrigger;

    private GameObject player;
    private RitualManager ritualManager;

    private int positionHash;



    void Awake()
    {
        player = GameObject.Find("Player");
        ritualManager = GameObject.Find("Ritual Manager").GetComponent<RitualManager>();

        positionHash = transform.position.GetHashCode() % 100000;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<AudioManager>().Play("DoorSlam");

            var playerStats = other.gameObject.GetComponent<PlayerStats>();
            playerStats.ApplyDamage(10);

            var camTransform = Camera.main.transform;

            var particleSpatter = Instantiate(bloodSpatter, camTransform.position, camTransform.rotation);
            particleSpatter.transform.parent = transform.root;

            ritualManager.AddToCurrentSeed(positionHash);

            gateTrigger.SetActive(false);
        }
    }
}
