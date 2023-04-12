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

    //list for all the possible upgrades that are currently unlocked and could show up in a draft
    public List<UpgradeCard> currentUpgradePool = new List<UpgradeCard>();

    //temporary pool of upgrades in the draft pool, any of which could be selected by the player
    public List<UpgradeCard> draftPool = new List<UpgradeCard>();

    //temporary pool of upgrades to prevent upgrade cards from showing up twice in the same draft
    private List<UpgradeCard> tempUpgradePool = new List<UpgradeCard>();




    void Awake()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();

        GenerateRarityDistribution();

        StartNewDraft();
    }


    void Update()
    {
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


    void RemoveCardFromPool(UpgradeCard card)
    {
        for (int i = 0; i < currentUpgradePool.Count; i++)
        {
            if (currentUpgradePool[i].name == card.name)
            {
                currentUpgradePool.RemoveAt(i);
                return;
            }
        }

        Debug.Log("Didn't find the card [" + card.name + "] to remove");
    }


    void Unlock(UpgradeCard card)
    {
        currentUpgradePool.Add(card);
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
        Debug.Log("Starting new draft, clearing pool");

        draftPool.Clear();

        tempUpgradePool.Clear();

        if (currentUpgradePool.Count > 0)
        {
            foreach (UpgradeCard card in currentUpgradePool)
            {
                tempUpgradePool.Add(card);
            }

            for (int i = 0; i < cardsPerDraft; i++)
            {
                AddRandomCardToCurrentDraft();
            }
        }
        else
        {
            Debug.Log("Somehow, you already drafted every single upgrade");
        }


    }


    void AddRandomCardToCurrentDraft()
    {
        if (tempUpgradePool.Count > 0)
        {
            int chosenRarity = RandomRarity();

            int cardIndex = IndexOfRandomCardOfRarity(chosenRarity);

            UpgradeCard card = tempUpgradePool[cardIndex];

            draftPool.Add(card);

            tempUpgradePool.RemoveAt(cardIndex);




            Debug.Log("┌──────────────────────────");
            Debug.Log("│   ** " + card.name + " **");
            Debug.Log("|  " + card.rarity);
            for (int i = 0; i < card.statModifiers.Count; i++)
            {
                string modifierName = card.statModifiers[i].name;
                float currentValue = playerStats.GetStatValue(card.statModifiers[i].name);
                float upgradedValue = currentValue + card.statModifiers[i].value;
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

            //apply modifiers to the player's stats
            for (int i = 0; i < selectedCard.statModifiers.Count; i++)
            {
                string modifierName = selectedCard.statModifiers[i].name;
                float modifierAmount = selectedCard.statModifiers[i].value;
                playerStats.ApplyModifier(modifierName, modifierAmount);
                float currentValue = playerStats.GetStatValue(selectedCard.statModifiers[i].name);
                float upgradedValue = currentValue + selectedCard.statModifiers[i].value;
            }

            //unlock any additional upgrades
            for (int i = 0; i < selectedCard.unlocks.Count; i++)
            {
                Unlock(selectedCard.unlocks[i]);
            }

            //remove from the draft pool
            RemoveCardFromPool(selectedCard);

            Debug.Log("Selected card " + cardNumber);

            //end current draft
            EndDraft();
        }
        else
        {
            Debug.Log("No card in slot " + cardNumber);
        }
    }


    void EndDraft()
    {
        tempUpgradePool.Clear();
        draftPool.Clear();

        for (int i = 0; i < 10; i++)
        {
            Debug.Log(" ");
        }

        StartNewDraft();

    }




    //returns value from 0-3 according to the rarityDistribution list, 0 being the most common
    int RandomRarity()
    {
        return rarityDistribution[Random.Range(0, rarityDistribution.Count)];
    }


    int IndexOfRandomCardOfRarity(int rarity)
    {
        //list of indices of cards in temp draft pool of the chosen rarity
        List<int> indexOfTempPoolCard = new List<int>();


        for (int i = 0; i < tempUpgradePool.Count; i++)
        {
            if (tempUpgradePool[i].rarity == rarity)
            {
                indexOfTempPoolCard.Add(i);
            }
        }

        if (indexOfTempPoolCard.Count > 0)
        {
            int selectedIndex = indexOfTempPoolCard[Random.Range(0, indexOfTempPoolCard.Count)];
            return selectedIndex;
        }
        else if (rarity > 0)
        {
            return IndexOfRandomCardOfRarity(rarity - 1);
        }
        else
        {
            Debug.Log("No remaining cards of rarity " + rarity + " or lower in pool");
            return 0;
        }
        
    }


}
