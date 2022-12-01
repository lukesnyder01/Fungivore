using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAudioSources : MonoBehaviour
{

    public GameObject mushroomAudio;
    public GameObject jumpMushroomAudio;
    public GameObject wraithAudio;

    void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.CompareTag ("Mushroom"))
        {
            GameObject newAudio = Instantiate(mushroomAudio, target.transform.position, target.transform.rotation);
            newAudio.transform.parent = target.transform;
        }

        if (target.gameObject.CompareTag("Jump Mushroom"))
        {
            GameObject newAudio = Instantiate(jumpMushroomAudio, target.transform.position, target.transform.rotation);
            newAudio.transform.parent = target.transform;
        }

        if (target.gameObject.CompareTag("Wraith"))
        {
            GameObject newAudio = Instantiate(wraithAudio, target.transform.position, target.transform.rotation);
            newAudio.transform.parent = target.transform;
        }



    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("AudioSource"))
        {
            Destroy(other.gameObject);
        }
    }

}
