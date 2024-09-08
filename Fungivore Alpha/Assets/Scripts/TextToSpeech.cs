using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextToSpeech : MonoBehaviour
{

    public DialogueVoice[] voices;

    private DialogueVoice dialogueVoice;

    private float _speechSpeed;
    private float _letterSoundLength;
    private float _clipVolume;
    private float _shortPauseLength;
    private float _longPauseLength;
    private AudioClip[] _sounds;

    public GameObject player;

    public TextMeshPro textMesh;

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
            StartSpeech(dialogueText[currentTextIndex], 0);
        }

        HideTextAfterTime();

    }


    public void StartSpeech(string text, int voiceIndex)
    {
        LoadVoice(voiceIndex);


        if (currentlySpeaking == false)
        {
            currentlySpeaking = true;
            speechCoroutine = StartCoroutine(ReadString(text));
        }
        else
        {
            if (speechCoroutine != null)
            {
                StopCoroutine(speechCoroutine);
            }

            textMesh.text = text;
            currentlySpeaking = false;

            GoToNextDialogueText();
        }
    }

    void LoadVoice(int i)
    {
        _speechSpeed = voices[i].speechSpeed;
        _letterSoundLength = voices[i].letterSoundLength;
        _clipVolume = voices[i].clipVolume;
        _shortPauseLength = voices[i].shortPauseLength;
        _longPauseLength = voices[i].longPauseLength;
        _sounds = voices[i].sounds;
}



    IEnumerator ReadString(string text)
    {
        currentDisplayText = "";

        foreach (char c in text)
        {
            AddLetterToDisplayText(c);

            if (c == ' ' || c == ',')
            {
                yield return new WaitForSeconds(_shortPauseLength / _speechSpeed);
            }
            else if (c == '.')
            {
                yield return new WaitForSeconds(_longPauseLength / _speechSpeed);
            }
            else
            {
                int index = (int)char.ToLower(c) - 97; // get the index of the corresponding AudioClip
                index = index % _sounds.Length;


                float duration = 0.1f;

                if (index >= 0)
                {
                    duration = _sounds[index].length / _speechSpeed;

                    // play the AudioClip at the position of the GameObject
                    AudioSource.PlayClipAtPoint(_sounds[index], player.transform.position, _clipVolume);
                }

                yield return new WaitForSeconds(duration * _letterSoundLength);
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
