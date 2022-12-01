using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    [SerializeField]
    public string skillName;

    [SerializeField]
    public string skillDescription;


    public int level = 0;


    [SerializeField]
    public float baseValue = 0;


    [SerializeField]
    public float valueChangePerLevel = 0;


    [SerializeField]
    public int baseUpgradeCost = 1;

    [SerializeField]
    public int maxLevel = 10;

    static int totalUpgradesPurchased = 0;


    public string GetName()
    {
        return skillName;
    }

    public string GetDescription()
    {
        return skillDescription;
    }


    public int GetLevel()
    {
        return level;
    }


    public float GetValue()
    {
        return baseValue + (valueChangePerLevel * level);
    }


    public float GetNextLevelValue()
    {
        if (level == maxLevel)
        {
            return 0f;
        }
        else
        {
            return baseValue + valueChangePerLevel * (level + 1);
        }

    }


    public void AddLevel()
    {
        if (level < maxLevel)
        {
            level++;
            totalUpgradesPurchased++;
        }
    }


    public int GetUpgradeCost()
    {
        if (level + 1 == maxLevel)
        {
            return 99999999;
        }
        else
        {
            return (baseUpgradeCost) * (level + 1) + totalUpgradesPurchased;
        }

        

        /*
        return baseUpgradeCost + (level * baseUpgradeCost) + ((baseUpgradeCost + level) * (baseUpgradeCost + level));
        */
    }

}