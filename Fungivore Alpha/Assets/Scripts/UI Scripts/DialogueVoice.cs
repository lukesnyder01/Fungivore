using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Voice", menuName = "Dialogue Voice/New Dialogue Voice")]
public class DialogueVoice : ScriptableObject
{
    public float speechSpeed;
    public float letterSoundLength;
    public float clipVolume;
    public float shortPauseLength;
    public float longPauseLength;
    public AudioClip[] sounds;
}