using System.Collections.Generic;
using UnityEngine;

public class SkillDraft : MonoBehaviour
{

    public List<UpgradeCard> currentPool = new List<UpgradeCard>();


    int cardsPerDraft = 100;


    private List<int> upgradeValues = new List<int>();


    // will hold all of the possible stat modifiers in the game, even ones that are currently locked or have already been maxed out
    private List<StatModifier> masterStatModList = new List<StatModifier>();


    // will hold all of the stat mods that could currently show up on an upgrade card
    // when a new stat mod is unlocked it gets added to this list and can be selected whenever an upgrade card is created
    private List<StatModifier> statModPool = new List<StatModifier>();




    private List<UpgradeCard> commonUpgradePool = new List<UpgradeCard>();



    public class StatModifier
    {
        public readonly string statName;
        public readonly int pointValue;
        public readonly float changePerLevel;

        public StatModifier(string _statName, int _pointValue, float _changePerLevel)
        {
            statName = _statName;
            pointValue = _pointValue;
            changePerLevel = _changePerLevel;
        }
    }


    public class UpgradeCard
    {
        public int totalPointValue;

        public List<StatModifier> statModifiers = new List<StatModifier>();
    }








    void Awake()
    {
        statModPool.Add(new StatModifier("Max Health", 1, 5f));
        statModPool.Add(new StatModifier("Move Speed", 1, 0.2f));
        statModPool.Add(new StatModifier("Spine Damage", 1, 1f));


        statModPool.Add(new StatModifier("Armor", 2, 1f));

        statModPool.Add(new StatModifier("Double Jumps", 4, 1f));

        InititalizeUpgradeValues();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {

            StartNewDraft();
        }
    }


    void InititalizeUpgradeValues()
    {
         int legendaryCount = 2;
         int rareCount = 8;
         int uncommonCount = 32;
         int commonCount = 64;


        for (int i = 0; i < legendaryCount; i++)
        {
            upgradeValues.Add(8);
        }

        for (int i = 0; i < rareCount; i++)
        {
            upgradeValues.Add(4);
        }

        for (int i = 0; i < uncommonCount; i++)
        {
            upgradeValues.Add(2);
        }

        for (int i = 0; i < commonCount; i++)
        {
            upgradeValues.Add(1);
        }
    }



    void StartNewDraft()
    {
        Debug.Log("Starting new draft...");


        for(int i = 0; i < cardsPerDraft; i++)
        {
            CreateNewUpgradeCard();
        }

    }


    void CreateNewUpgradeCard()
    {
        //create a new card
        UpgradeCard newCard = new UpgradeCard();

        //give it a point value from the upgrade values list
        newCard.totalPointValue = NewUpgradeValue();

        var currentPointValue = 0;

        var currentTries = 0;

        //randomly select a stat mod from the pool and add it if it doesn't make the upgrade card go over its point value
        while (currentPointValue < newCard.totalPointValue)
        {
            //make sure we don't get stuck in an infinte loop if no remaining stat mod fits
            currentTries++;

            if (currentTries > 100)
            {
                return;
            }

            //pick a random mod from the mod pool
            var potentialMod = statModPool[Random.Range(0, statModPool.Count)];

            //if the mod doesn't cost too many points, add it to the upgrade card
            if (potentialMod.pointValue + currentPointValue <= newCard.totalPointValue)
            {
                newCard.statModifiers.Add(potentialMod);
                currentPointValue += potentialMod.pointValue;
            }
        }

        currentPool.Add(newCard);

        Debug.Log("Card value = " + newCard.totalPointValue);

        Debug.Log("┌──────────────────────────");

        for (int i = 0; i < newCard.statModifiers.Count; i++)
        {
            Debug.Log("│ " + newCard.statModifiers[i].statName + " +" + newCard.statModifiers[i].changePerLevel);
        }

        Debug.Log("└──────────────────────────");


    }


    int NewUpgradeValue()
    {
        return upgradeValues[Random.Range(0, upgradeValues.Count)];
    }


}
