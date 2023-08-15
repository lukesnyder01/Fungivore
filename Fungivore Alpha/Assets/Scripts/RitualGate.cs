using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualGate : MonoBehaviour
{
    public GameObject bloodSpatter;

    private GameObject player;
    private RitualManager ritualManager;

    private int positionHash;

    private bool playerTriggered = false;

    private Vector3 initialPosition;

    public float moveDistance = 2.0f;
    public float moveDuration = 1f;


    void Awake()
    {
        initialPosition = transform.localPosition;

        player = GameObject.Find("Player");
        ritualManager = GameObject.Find("Ritual Manager").GetComponent<RitualManager>();

        positionHash = transform.position.GetHashCode() % 100000;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && playerTriggered == false)
        {
            playerTriggered = true;
            StartCoroutine(ActivateRitualSpike());
        }
    }


    private IEnumerator ActivateRitualSpike()
    {
        //moves the spike upwards quickly, impaling the player


        float elapsedTime = 0;

        Vector3 targetPosition = initialPosition + transform.up * moveDistance;

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;

        ritualManager.AddToCurrentSeed(positionHash);

        SlicePlayer();

        gameObject.SetActive(false);

    }


    private void SlicePlayer()
    {
        FindObjectOfType<AudioManager>().Play("DoorSlam");

        var playerStats = player.GetComponent<PlayerStats>();
        playerStats.ApplyDamage(10);

        var camTransform = Camera.main.transform;

        var particleSpatter = Instantiate(bloodSpatter, camTransform.position, camTransform.rotation);
        particleSpatter.transform.parent = transform.root;
    }


}
