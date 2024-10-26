using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCharacter : MonoBehaviour, IInteractable
{

    private GameObject player;
    private TextToSpeech textToSpeech;

    private float turnSpeed = 7.0f;
    private float hoverFrequency = 3f;
    private float hoverAmplitude = 0.1f;

    public string[] dialogueText;

    private bool turningTowardsPlayer;
    private Vector3 startingPosition;

    private int currentTextIndex = 0;


    public string PromptText { get; set; } = "[E] Talk";


    void Start()
    {
        player = GameObject.Find("Player");
        textToSpeech = player.GetComponent<TextToSpeech>();
        startingPosition = transform.position;
    }


    public void Interact()
    {
        if (textToSpeech.currentlySpeaking == true)
        {
            textToSpeech.CancelReadingAndDisplayFullText();
        }
        else
        {
            textToSpeech.StartSpeech(dialogueText[currentTextIndex], 0);
            GoToNextDialogueText();
        }
    }


    public void LoseFocus()
    {
        turningTowardsPlayer = false;
    }

    public void StartFocus()
    {
        turningTowardsPlayer = true;
    }


    void GoToNextDialogueText()
    {
        currentTextIndex++;

        if (currentTextIndex >= dialogueText.Length)
        {
            currentTextIndex = 0;
        }
    }


    public void PointAtPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }


    public void Hover()
    {
        float newY = startingPosition.y + Mathf.Cos(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        Hover();

        if (turningTowardsPlayer)
        {
            PointAtPlayer();
        }
    }
}
