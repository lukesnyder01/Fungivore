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

    private string currentText = "";




    void Start()
    {
        player = GameObject.Find("Player");

        textMesh = player.GetComponent<PlayerUI>().npcTextUI;

    }

    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            string testString = "I am the Sophont of Istrachill. Grovel before me you filthy worm, you Morld-Cologi depraved.";

            StartCoroutine(ReadString(testString));
        }
    }


    IEnumerator ReadString(string text)
    {
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
    }

    void AddLetterToDisplayText(char letter)
    {
        currentText = currentText + letter.ToString();
        textMesh.text = currentText;
    }



}
