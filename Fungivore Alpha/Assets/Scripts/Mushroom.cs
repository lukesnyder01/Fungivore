using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mushroom : MonoBehaviour, IInteractable
{

    public GameObject mushroomDeathEffect;

    private GameObject audioPrefab;
    private AudioSource audioSource;

    public string PromptText { get; set; } = "E";

    public void StartFocus()
    {

    }

    public void LoseFocus()
    {

    }

    public void Interact() 
    {
        PlayerStats.sporesInventory++;

        AudioManager.Instance.Play("MushroomScream");
        Instantiate(mushroomDeathEffect, transform.position - new Vector3(0f, 0.3f, 0f), Quaternion.identity);

        Destroy(gameObject);
    }



}
