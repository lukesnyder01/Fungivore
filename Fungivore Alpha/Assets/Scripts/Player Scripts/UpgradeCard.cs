using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade Card", menuName = "Upgrade Card/New Upgrade Card")]
public class UpgradeCard : ScriptableObject
{
    public string upgradeName;

    public int rarity;

    public List<KeyValuePair<string, float>> statModifiers;

    public List<UpgradeCard> unlocks = new List<UpgradeCard>();

}
