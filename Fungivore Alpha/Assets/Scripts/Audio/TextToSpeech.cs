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

    private string currentFullDialogueText;

    private string currentDisplayText = "";
    public bool textIsHidden = true;

    public bool currentlySpeaking = false;

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
        HideTextAfterTime();
    }


    public void StartSpeech(string text, int voiceIndex)
    {
        currentlySpeaking = true;
        LoadVoice(voiceIndex);
        speechCoroutine = StartCoroutine(ReadString(text));
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


    public void CancelReadingAndDisplayFullText()
    {
        if (speechCoroutine != null)
        {
            StopCoroutine(speechCoroutine);
        }

        textMesh.text = currentFullDialogueText;
        currentlySpeaking = false;
    }


    IEnumerator ReadString(string text)
    {
        currentFullDialogueText = text;
        textIsHidden = false;

        currentDisplayText = "";

        foreach (char c in currentFullDialogueText)
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

        currentlySpeaking = false;
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
                textIsHidden = true;
            }
        }
        else
        {
            hideTextTimer = 0;
        }
    }


}
