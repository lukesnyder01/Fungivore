using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject target;
    public GameObject player;
    public Vector3 displacement;

    public GameObject activationTarget;
    public GameObject deactivationTarget;


    void Start() 
    {
        displacement = target.transform.position - transform.position;
        player = GameObject.Find("Player");
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.GetComponent<CharacterController>().enabled = false;

            player.transform.position += displacement;
            //player.transform.position = target.transform.position;
            player.GetComponent<CharacterController>().enabled = true;

            FindObjectOfType<AudioManager>().Play("Teleport");

            if (activationTarget != null)
            {
                activationTarget.SetActive(true);
            }

            if (deactivationTarget != null)
            {
                deactivationTarget.SetActive(false);
            }

        }
    }

}
