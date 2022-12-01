using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Mushroom : MonoBehaviour
{
    private GameObject player;

    public GameObject mushroomDeathEffect;
    public GameObject jumpMushroomDeathEffect;

    private GameObject audioPrefab;
    private AudioSource audioSource;
    public float audioFadeTime = 2f;

    private float playerDistance;
    public float pickupDistance = 0.2f;
    
    public bool isJumpMushroom = false;

    private bool hasBeenPickedUp = false;

    void Start() 
    {
        player = GameObject.Find("Player");
    }


    void OnTriggerStay(Collider other)
    {
        if (Vector3.Distance(other.transform.position, transform.position) < pickupDistance)
        {

            if (other.gameObject.tag == "Player" && hasBeenPickedUp == false)
            {
                hasBeenPickedUp = true;
                DisableRendererAndCollider();
                StartCoroutine("FadeOutAndDestroy");
                AddStats();
                DestroyEffects(other);
            }
            else if (other.gameObject.tag == "Wraith")
            {
                DisableRendererAndCollider();
                StartCoroutine("FadeOutAndDestroy");
                DestroyEffects(other);
            }
            else if (other.gameObject.tag == "Mushroom" || other.gameObject.tag == "Jump Mushroom")
            {
                Destroy(gameObject);
            }
        }
    }


    public void DisableRendererAndCollider()
    {
        GetComponent<Collider>().enabled = false;

        Component[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.enabled = false;
        }
    }

    public void AddStats()
    {
        if (isJumpMushroom)
        {
            PlayerStats.maxDoubleJumpValue++;
        }
        else
        {
            PlayerStats.sporesInventory++;
        }
    }


    public void DestroyEffects(Collider other)
    {
        if (isJumpMushroom)
        {
            FindObjectOfType<AudioManager>().Play("JumpMushroomScream");

            Instantiate(jumpMushroomDeathEffect, transform.position - new Vector3(0f, 0.3f, 0f), other.transform.rotation);
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("MushroomScream");

            Instantiate(mushroomDeathEffect, transform.position - new Vector3(0f, 0.3f, 0f), other.transform.rotation);
        }
    }


    public IEnumerator FadeOutAndDestroy()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "AudioSource")
            {
                audioPrefab = child.gameObject;
            }
        }


        if (audioPrefab != null)
        {
            audioPrefab.transform.parent = null;

            audioSource = audioPrefab.GetComponent<AudioSource>();


            float startVolume = audioSource.volume;

            //this quickly fades the volume out of the audio source before playing the clip, to avoid popping by ensuring new clips always start from 0
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / audioFadeTime;
                yield return null;
            }

            Destroy(audioPrefab);
        }

        Destroy(gameObject);
    }



}
