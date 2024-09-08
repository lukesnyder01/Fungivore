using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class MemoryStone : MonoBehaviour, IInteractable
{



    string death = "d̶̖̏ḙ̴͋ă̴͙t̷̘̚h̶͇͝";

    string life = "l̸͎̈́i̶̲̇f̵̺͝ẽ̸̢";

    string fungus = "f̴̛̞u̴͉̽n̵̳̄g̷͚̓û̴̖s̸̜͘";



    string[] memoryStrings = new string[] {

        


        "d̶̖̏ḙ̴͋ă̴͙t̷̘̚h̶͇͝",

        "l̸͎̈́i̶̲̇f̵̺͝ẽ̸̢",

        "f̴̛̞u̴͉̽n̵̳̄g̷͚̓û̴̖s̸̜͘",

        "שคק๏гฬคשє",

        "г",
        "¤",

        "■",

        "▄",

        "▀",

        "█",

        "■██",


        " ▓ ",


        "▒",


        "░",


        "▓▓",

        "┤├",

        "╣",

        "╗",

        "╝",

        "‗",

        "╬",

        "╦",






    };


    string selectedString;



    [SerializeField]
    private TextMeshPro textMeshPro = null;

    public GameObject hitEffect;

    private int damageToDeal;

    private GameObject player;
    private PlayerStats playerStats;

    public GameObject floatingTextPrefab;


    public int minStringLength = 5;
    public int maxStringLength = 20;


    public string PromptText { get; set; } = "E";

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerStats>();

        BuildRandomString();
        SetTextToString();


    }

    void BuildRandomString()
    {
        Random.InitState(transform.position.GetHashCode() % 100000);

        int stringLength = Random.Range(minStringLength, maxStringLength);

        for (int i = 0; i < stringLength; i++)
        {
            selectedString = selectedString + memoryStrings[Random.Range(0, memoryStrings.Length)] + "  ";

        }


        


        //Debug.Log(selectedString);
    }

    void SetTextToString()
    {
        textMeshPro.text = selectedString;
    }


    /*
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PlayerBullet")
        {
            AddStats();

            Destroy(gameObject);
            Instantiate(hitEffect, transform.position, transform.rotation);
            FindObjectOfType<AudioManager>().Play("StoneBreak");
        }

    }
    */

    public void StartFocus()
    {

    }

    public void LoseFocus()
    {

    }

    public void Interact()
    {
        AddStats();

        Destroy(gameObject);
        Instantiate(hitEffect, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("StoneBreak");
    }




    void AddStats()
    {
        string textToPopup = null;


        //count death words and deal damage based on how many there are

        int deathWords = Regex.Matches(selectedString, death).Count;

        if (deathWords > 0)
        {
            playerStats.ApplyDamage(50 * deathWords);
            textToPopup = textToPopup + "Y̵͔̆ö̶̫́ǔ̵̞ ̷̬̀t̶̠̆o̶̻͌o̵̺͛ḳ̵̽  " + (50 * deathWords) + " d̷͕̐ḁ̷̿m̵̩͠a̸̭͑g̵̗͘ë̶͙ \n";
        }



        //count life words and increase suffering tolerance based on how many there are

        int lifeWords = Regex.Matches(selectedString, life).Count;

        for (int i = 0; i < lifeWords; i++)
        {
            //PlayerStats.sufferingTolerance.AddLevel();
        }

        if (lifeWords > 0)
        {
            textToPopup = textToPopup + "+ " + lifeWords + "  s̵̜͂u̵̗̓f̷̼͠f̵̩̔ȅ̸͓ŕ̷̫i̶̙͝n̵̖̈́g̸͚̿ ̶͚͛t̴͕͌o̷͍̅l̵͓̒ẹ̷̐ř̴̠ä̵̗ñ̴̯c̶̣̏e̶̦͒ \n";
        }

        

        //count fungus words and increase incubator efficiency based on how many there are

        int fungusWords = Regex.Matches(selectedString, fungus).Count;

        for (int i = 0; i < fungusWords; i++)
        {
            //PlayerStats.incubatorEfficiency.AddLevel();
        }

        if (fungusWords > 0)
        {
            textToPopup = textToPopup + "+ " + fungusWords + "  i̶̡̾ñ̸͖ċ̴̝u̷͠ͅb̴̘̋ā̵͍t̴̢̍ȏ̵͖r̸͈̅ ̷̨̈e̶̩͠f̸̥̕f̶͖̃i̵̗͗ć̸̺i̶͈̚ê̶̩n̴̰̈́c̷͔͠y̵͓̔ \n";
        }



        //handle null popup text
        if (textToPopup == null)
        {
            textToPopup = "Y̷̹͌ó̵̟ŭ̶͔ ̶̱͑h̵̼̽ä̵͖́ṽ̶͕e̴͈͑ ̶̼̇l̴̰͐ẻ̴̱a̴͖̿r̷͖̆n̸̟͆é̶̱d̷͙͑ ̴͉̀n̴̜̋o̶̮̚ẗ̶̘h̷͎̃i̸̟͋n̷̗͌g̷̻͒";
        }


        SpawnPopupText(textToPopup);
        
    }

    void SpawnPopupText(string popupString)
    {
        GameObject popup = Instantiate(floatingTextPrefab, transform.position, transform.rotation);
        popup.GetComponentInChildren<TextMeshPro>().text = popupString;


    }

    /*
     * 
             tooltipTrigger.content =

                playerStats.skills[skillID].GetDescription() + 
                "\nCurrent: " + playerStats.skills[skillID].GetValue() + 
                "\nNext Level: " + playerStats.skills[skillID].GetNextLevelValue() + 
                "\nCost: " + playerStats.skills[skillID].GetUpgradeCost();
     
     
     */



}
