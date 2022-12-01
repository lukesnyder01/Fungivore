using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualGate : MonoBehaviour
{
    private GameObject player;
    private RitualManager ritualManager;

    public GameObject gatesToDestroy;
    public GameObject bloodSpatter;

    public bool isXGate = false;
    public bool isYGate = false;
    public bool isZGate = false;

    void Start()
    {
        player = GameObject.Find("Player");
        ritualManager = GameObject.Find("Ritual Manager").GetComponent<RitualManager>();
        

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            FindObjectOfType<AudioManager>().Play("DoorSlam");

            var playerStats = other.gameObject.GetComponent<PlayerStats>();
            playerStats.ApplyDamage(10);

            var particleSpatter = Instantiate(bloodSpatter, other.transform.position, transform.rotation);
            particleSpatter.transform.parent = transform.root;

            if (isXGate)
            {
                ritualManager.SetXShift(transform.position.x);
            }

            if (isYGate)
            {
                ritualManager.SetYShift(transform.position.x);
            }

            if (isZGate)
            {
                ritualManager.SetZShift(transform.position.x);
            }

            Destroy(gatesToDestroy);

        }




    }

}
