using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SkillButton : MonoBehaviour
{
    public int skillID = 0;
    public Text nameText;
    //public Text upgradeText;
    //public Text costText;

    public PlayerStats PlayerStats;

    public TooltipTrigger tooltipTrigger;

    private Button _button;

    private bool canAfford;

    private int upgradeCost = 0;




    void Start()
    {
        tooltipTrigger = this.GetComponent<TooltipTrigger>();
        PlayerStats = GameObject.Find("Player").GetComponent<PlayerStats>();

        nameText = this.transform.Find("Name Text").GetComponent<Text>();

        _button = this.GetComponent<Button>();
    }


    void Update()
    {
        UpdateTooltipValues();

        //upgradeCost = PlayerStats.skills[skillID].GetUpgradeCost();

        UpdateAffordability();

        UpdateLevelText();

    }

    public void UpdateLevelText()
    { 
    
    }


    public void UpgradeSkill()
    {
        if (canAfford)
        {
            PlayerStats.experiencePoints -= upgradeCost;
            //PlayerStats.skills[skillID].AddLevel();
        }
    }

    public void UpdateAffordability()
    {
        canAfford = upgradeCost <= PlayerStats.experiencePoints;

        _button.interactable = canAfford;

    }


    public void UpdateTooltipValues()
    {
        /*
        tooltipTrigger.header = PlayerStats.skills[skillID].GetName();

        nameText.text = PlayerStats.skills[skillID].GetLevel().ToString();

        tooltipTrigger.content =

                PlayerStats.skills[skillID].GetDescription() + 
                "\nCurrent: " + PlayerStats.skills[skillID].GetValue() + 
                "\nNext Level: " + PlayerStats.skills[skillID].GetNextLevelValue() + 
                "\nCost: " + PlayerStats.skills[skillID].GetUpgradeCost();
        */
    }

}
