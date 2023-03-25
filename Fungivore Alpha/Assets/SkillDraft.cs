using System.Collections.Generic;
using UnityEngine;

public class SkillDraft : MonoBehaviour
{
 

    int cardsPerDraft = 3;

    //list of rarities 
    //0 - common, 1 - uncommon, 2 - rare, 3 - legendary   
    private List<int> rarityDistribution = new List<int>();

    //how many times each rarity shows up in the rarity distribution
    private int legendaryCount = 1;
    private int rareCount = 8;
    private int uncommonCount = 32;
    private int commonCount = 64;


    // will hold all of the stat mods that can show up on a card
    private List<StatModifier> statModList = new List<StatModifier>();


    // will hold all of the possible stat modifiers in the game, even ones that are currently locked or have already been maxed out
    private List<UpgradeCard> masterUpgradeList = new List<UpgradeCard>();

    //list for all the possible upgrades that are currently unlocked
    private List<UpgradeCard> currentUpgradePool = new List<UpgradeCard>();

    //temporary pool for upgrade cards, to prevent upgrade cards from showing up twice in the same draft
    private List<UpgradeCard> tempUpgradePool = new List<UpgradeCard>();

    //temporary list for cards selected in each draft
    public List<UpgradeCard> draftPool = new List<UpgradeCard>();


    public class StatModifier
    {
        public readonly string name;
        public readonly float amount;

        public StatModifier(string _name, float _amount)
        {
            name = _name;
            amount = _amount;
        }
    }


    public class UpgradeCard
    {
        public string name;
        public int rarity;
        public List<StatModifier> statModifiers = new List<StatModifier>();
        public List<string> unlocks = new List<string>();
    }




    void Awake()
    {
        GenerateMasterUpgradeList();

        InitialzeStartingUpgradeList();

        GenerateRarityDistribution();
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartNewDraft();
        }
    }



    void GenerateRarityDistribution()
    {
        rarityDistribution.Clear();

        for (int i = 0; i < legendaryCount; i++)
        {
            rarityDistribution.Add(3);
        }

        for (int i = 0; i < rareCount; i++)
        {
            rarityDistribution.Add(2);
        }

        for (int i = 0; i < uncommonCount; i++)
        {
            rarityDistribution.Add(1);
        }

        for (int i = 0; i < commonCount; i++)
        {
            rarityDistribution.Add(0);
        }
    }

    void GenerateMasterUpgradeList()
    {
        UpgradeCard card;

        card = new UpgradeCard();
        card.name = "Fortify Constitution I";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Max Health", 5f));
        card.unlocks.Add("Fortify Constitution II");
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Chitinous Shell";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Armor", 1f));
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Quickness";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Movement Speed", 1f));
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Fortify Constitution II";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Max Health", 10f));
        masterUpgradeList.Add(card);
    }


    void InitialzeStartingUpgradeList()
    {
        MoveCardToList("Fortify Constitution I", masterUpgradeList, currentUpgradePool);
        MoveCardToList("Chitinous Shell", masterUpgradeList, currentUpgradePool);
        MoveCardToList("Quickness", masterUpgradeList, currentUpgradePool);
    }


    void MoveCardToList(string card, List<UpgradeCard> fromList, List<UpgradeCard> toList)
    {
        for (int i = 0; i < fromList.Count; i++)
        {
            if (fromList[i].name == card)
            {
                toList.Add(fromList[i]);
                fromList.RemoveAt(i);
                return;
            }
        }

        Debug.Log("Didn't find the card [" + card + "] to move");
    }




    void GenerateModList()
    {
        statModList.Add(new StatModifier("Max Health", 5f));
        statModList.Add(new StatModifier("Move Speed", 0.2f));
        statModList.Add(new StatModifier("Spine Damage", 1f));
        statModList.Add(new StatModifier("Armor", 1f));
        statModList.Add(new StatModifier("Double Jumps", 1f));
    }



    void StartNewDraft()
    {
        tempUpgradePool.Clear();

        foreach (UpgradeCard card in currentUpgradePool)
        {
            tempUpgradePool.Add(card);
        }

        Debug.Log("Starting new draft, clearing pool");
        draftPool.Clear();

        for(int i = 0; i < cardsPerDraft; i++)
        {
            AddRandomCardToCurrentDraft();
        }

    }


    void AddRandomCardToCurrentDraft()
    {
        if (tempUpgradePool.Count > 0)
        {
            var cardIndex = Random.Range(0, tempUpgradePool.Count);
            UpgradeCard card = tempUpgradePool[cardIndex];
            draftPool.Add(card);
            tempUpgradePool.RemoveAt(cardIndex);


            Debug.Log("┌──────────────────────────");
            Debug.Log("│   ** " + card.name + " **");
            for (int i = 0; i < card.statModifiers.Count; i++)
            {
                Debug.Log("│ " + card.statModifiers[i].name + " +" + card.statModifiers[i].amount);
            }
            Debug.Log("└──────────────────────────");

        }
        else
        {
            Debug.Log("Not enough cards to draft!");
        }

    }



    /*
    void CreateNewUpgradeCard()
    {
        //create a new card
        UpgradeCard newCard = new UpgradeCard();

        //give it a point value from the upgrade values list
        newCard.totalPointValue = RandomRarity();

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
    */



    //returns value from 0-3 according to the rarityDistribution list, 0 being the most common
    int RandomRarity()
    {
        return rarityDistribution[Random.Range(0, rarityDistribution.Count)];
    }


}
