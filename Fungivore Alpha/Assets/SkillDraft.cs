using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDraft : MonoBehaviour
{
    public List<string> skillDeck = new List<string>();

    public List<string> currentPool = new List<string>();


    public int skillsPerDraft = 3;



    private List<int> skillValuePool = new List<int>();



    private int legendaryCount = 1;
    private int rareCount = 8;
    private int uncommonCount = 32;
    private int commonCount = 128;



    void Awake()
    {

        InitializeSkillValuePool();

        InitializeSkillDeck();

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {

            StartNewDraft();
        }
    }


    void InitializeSkillValuePool()
    {
        for (int i = 0; i < legendaryCount; i++)
        {
            skillValuePool.Add(8);
        }

        for (int i = 0; i < rareCount; i++)
        {
            skillValuePool.Add(4);
        }

        for (int i = 0; i < uncommonCount; i++)
        {
            skillValuePool.Add(2);
        }

        for (int i = 0; i < commonCount; i++)
        {
            skillValuePool.Add(1);
        }
    }


    void InitializeSkillDeck()
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
        Debug.Log("Added skill worth " + NewSkillValue() + " points.");

        /*
        int index = skillDeck.Count - 1;
        currentPool.Add(skillDeck[index]);
        Debug.Log("[[ " + skillDeck[index] + " ]] was added to draft pool.");
        skillDeck.RemoveAt(index);

        */
    }


    int NewSkillValue()
    {
        return skillValuePool[Random.Range(0, skillValuePool.Count)];
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
