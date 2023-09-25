using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextToSpeech : MonoBehaviour
{
    public float speechSpeed;

    public float letterSoundLength;

    public float clipVolume;

    public float shortPauseLength;
    public float longPauseLength;

    public TextMeshPro textMesh;

    public AudioClip[] sound;

    public GameObject player;


    public string[] dialogueText;
    private int currentTextIndex = 0;


    private string currentDisplayText = "";
    private bool currentlySpeaking = false;

    private Coroutine speechCoroutine;

    public float textTimeout = 3;
    private float hideTextTimer = 0;


    void Start()
    {
        player = GameObject.Find("Player");

        textMesh = player.GetComponent<PlayerUI>().npcTextUI;

        textMesh.text = "";

    }


    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            if (currentlySpeaking == false)
            {
                currentlySpeaking = true;
                speechCoroutine = StartCoroutine(ReadString(dialogueText[currentTextIndex]));
            }
            else
            {
                if (speechCoroutine != null)
                {
                    StopCoroutine(speechCoroutine);
                }    

                textMesh.text = dialogueText[currentTextIndex];
                currentlySpeaking = false;

                GoToNextDialogueText();
            }
        }

        HideTextAfterTime();

    }


    IEnumerator ReadString(string text)
    {
        currentDisplayText = "";

        foreach (char c in text)
        {
            AddLetterToDisplayText(c);

            if (c == ' ' || c == ',')
            {
                yield return new WaitForSeconds(shortPauseLength / speechSpeed);
            }
            else if (c == '.')
            {
                yield return new WaitForSeconds(longPauseLength / speechSpeed);
            }
            else
            {
                int index = (int)char.ToLower(c) - 97; // get the index of the corresponding AudioClip

                float duration = 0.1f;

                if (index >= 0 && index < sound.Length) // make sure the index is within bounds
                {
                    duration = sound[index].length / speechSpeed;

                    // play the AudioClip at the position of the GameObject
                    AudioSource.PlayClipAtPoint(sound[index], player.transform.position, clipVolume);
                }

                yield return new WaitForSeconds(duration * letterSoundLength);
            }
        }

        GoToNextDialogueText();
        currentlySpeaking = false;
    }


    void GoToNextDialogueText()
    {
        currentTextIndex++;

        if (currentTextIndex >= dialogueText.Length)
        {
            currentTextIndex = 0;
        }
    }



    void AddLetterToDisplayText(char letter)
    {
        currentDisplayText = currentDisplayText + letter.ToString();
        textMesh.text = currentDisplayText;
    }


    void HideTextAfterTime()
    {
        if (currentlySpeaking == false)
        {
            hideTextTimer += Time.deltaTime;

            if (hideTextTimer >= textTimeout)
            {
                textMesh.text = "";
            }
        }
        else
        {
            hideTextTimer = 0;
        }
    }


}
