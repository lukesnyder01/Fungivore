using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade Card", menuName = "Upgrade Card/New Upgrade Card")]
public class UpgradeCard : ScriptableObject
{
    public int rarity;

    public List<Modifier> statModifiers = new List<Modifier>();

    public List<UpgradeCard> unlocks = new List<UpgradeCard>();
}

[System.Serializable]
public class Modifier
{
    public string name;
    public float value;
}