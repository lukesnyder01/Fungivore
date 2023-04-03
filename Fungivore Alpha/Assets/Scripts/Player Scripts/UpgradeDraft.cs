using System.Collections.Generic;
using UnityEngine;

public class UpgradeDraft : MonoBehaviour
{
    private PlayerStats playerStats;

    int cardsPerDraft = 3;

    //list of rarities 
    //0 - common, 1 - uncommon, 2 - rare, 3 - legendary   
    private List<int> rarityDistribution = new List<int>();


    //how many times each rarity shows up in the rarity distribution
    private int legendaryCount = 1;
    private int rareCount = 8;
    private int uncommonCount = 32;
    private int commonCount = 64;


    // will hold all of the possible upgrades in the game, even ones that are currently locked or have already been selected
    private List<UpgradeCard> masterUpgradeList = new List<UpgradeCard>();

    //list for all the possible upgrades that are currently unlocked and could show up in a draft
    private List<UpgradeCard> currentUpgradePool = new List<UpgradeCard>();

    //temporary pool of upgrades in the draft pool, any of which could be selected by the player
    public List<UpgradeCard> draftPool = new List<UpgradeCard>();

    //temporary pool of upgrades to prevent upgrade cards from showing up twice in the same draft
    private List<UpgradeCard> tempUpgradePool = new List<UpgradeCard>();





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
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChooseCard(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChooseCard(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChooseCard(3);
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
        card.statModifiers.Add(new StatModifier("Max Health", 10f));
        card.unlocks.Add("Fortify Constitution II");
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Chitinous Shell";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Armor", 1f));
        card.statModifiers.Add(new StatModifier("Walk Speed", -0.2f));
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Quickness";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Walk Speed", 0.5f));
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Improve Energy Reserve";
        card.rarity = 0;
        card.statModifiers.Add(new StatModifier("Max Energy", 10f));
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Fortify Constitution II";
        card.rarity = 1;
        card.statModifiers.Add(new StatModifier("Max Health", 15f));
        card.statModifiers.Add(new StatModifier("Armor", 1f));
        masterUpgradeList.Add(card);


        card = new UpgradeCard();
        card.name = "Double Jump I";
        card.rarity = 3;
        card.statModifiers.Add(new StatModifier("Double Jumps", 1f));
        masterUpgradeList.Add(card);
    }



    void InitialzeStartingUpgradeList()
    {
        MoveCardToList("Fortify Constitution I", masterUpgradeList, currentUpgradePool);
        MoveCardToList("Chitinous Shell", masterUpgradeList, currentUpgradePool);
        MoveCardToList("Quickness", masterUpgradeList, currentUpgradePool);
        MoveCardToList("Improve Energy Reserve", masterUpgradeList, currentUpgradePool);
        MoveCardToList("Double Jump I", masterUpgradeList, currentUpgradePool);
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
                string modifierName = card.statModifiers[i].name;
                float currentValue = playerStats.GetStatValue(card.statModifiers[i].name);
                float upgradedValue = currentValue + card.statModifiers[i].amount;

                Debug.Log("│ " + modifierName + " " + currentValue + " > " + upgradedValue);

            }
            Debug.Log("└──────────────────────────");

        }
        else
        {
            Debug.Log("Not enough cards to draft!");
        }

    }


    void ChooseCard(int cardNumber)
    {
        var draftIndex = cardNumber - 1;
        if (draftPool[draftIndex] != null)
        {

            UpgradeCard selectedCard = draftPool[draftIndex];
            for (int i = 0; i < selectedCard.statModifiers.Count; i++)
            {
                string modifierName = selectedCard.statModifiers[i].name;

                float modifierAmount = selectedCard.statModifiers[i].amount;

                playerStats.ApplyModifier(modifierName, modifierAmount);

                float currentValue = playerStats.GetStatValue(selectedCard.statModifiers[i].name);
                float upgradedValue = currentValue + selectedCard.statModifiers[i].amount;
            }

            Debug.Log("Selected card " + cardNumber);

        }
        else
        {
            Debug.Log("No card in slot " + cardNumber);
        }
    }



    //returns value from 0-3 according to the rarityDistribution list, 0 being the most common
    int RandomRarity()
    {
        return rarityDistribution[Random.Range(0, rarityDistribution.Count)];
    }


}
