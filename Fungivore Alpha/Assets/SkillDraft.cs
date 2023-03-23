using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDraft : MonoBehaviour
{
    public List<string> skillDeck = new List<string>();

    public List<string> currentPool = new List<string>();

    private int skillsPerDraft = 3;

    void Awake()
    {
        skillDeck.Add("max HP +10");
        skillDeck.Add("max HP +10");
        skillDeck.Add("max HP +10");
        skillDeck.Add("max HP +15");
        skillDeck.Add("spine damage +1");
        skillDeck.Add("spine damage +1");
        skillDeck.Add("spine damage +2");
        skillDeck.Add("armor +1");
        skillDeck.Add("armor +1");
        skillDeck.Add("armor +2");



 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {

            StartNewDraft();
        }
    }


    void StartNewDraft()
    {
        Debug.Log("Starting new draft, shuffling...");

        ShuffleSkillList();

        Debug.Log("Shuffle completed, adding skills to pool...");

        for(int i = 0; i < skillsPerDraft; i++)
        {
            AddSkillToDraftPool();
        }

    }

    void AddSkillToDraftPool()
    {
        int index = skillDeck.Count - 1;
        currentPool.Add(skillDeck[index]);
        Debug.Log("[[ " + skillDeck[index] + " ]] was added to draft pool.");
        skillDeck.RemoveAt(index);
    }


    void ShuffleSkillList()
    {
        var ts = skillDeck;
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }


}
