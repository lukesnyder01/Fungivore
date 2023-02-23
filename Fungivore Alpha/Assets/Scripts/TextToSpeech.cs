using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToSpeech : MonoBehaviour
{
    public float soundDelay;
    public float speedMultiplier;
    public float clipVolume;

    public AudioClip[] sound;


    void Start()
    {
        string testString = "I am the Sophont of Istrachill. Grovel before me you filthy worm, you Morld-Cologi depraved.";

        Debug.Log(testString);

        StartCoroutine(ReadString(testString));
    }


    IEnumerator ReadString(string text)
    {
        foreach (char c in text)
        {
            if (c == ' ')
            {
                yield return new WaitForSeconds(0.2f / speedMultiplier);
                Debug.Log("space");
            }
            else if (c == '.')
            {
                yield return new WaitForSeconds(0.3f / speedMultiplier);
                Debug.Log("period");
            }
            else
            {
                int index = (int)char.ToLower(c) - 97; // get the index of the corresponding AudioClip

                float duration = 0.1f;

                if (index >= 0 && index < sound.Length) // make sure the index is within bounds
                {
                    duration = sound[index].length / speedMultiplier;

                    Debug.Log(index);
                    // play the AudioClip at the position of the GameObject
                    AudioSource.PlayClipAtPoint(sound[index], transform.position, clipVolume); 
                }

                yield return new WaitForSeconds(duration * soundDelay);
            }
        }
    }

}
