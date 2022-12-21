using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public Skill[] skills = new Skill[100];

    [HideInInspector] public static Skill spinesPerShot;
    [HideInInspector] public static Skill spineRegen;
    [HideInInspector] public static Skill maxSpines;
    [HideInInspector] public static Skill spineFireRate;
    [HideInInspector] public static Skill spineSpeed;
    [HideInInspector] public static Skill incubatorEfficiency;
    [HideInInspector] public static Skill sufferingTolerance;
    [HideInInspector] public static Skill doubleJumps;
    [HideInInspector] public static Skill baseWalkSpeed;
    [HideInInspector] public static Skill baseHungerDecay;

    public static float verticalRecoil = -6;
    public static float horizontalRecoil = 2;
    public static float spineSpread = 0.01f;

    [HideInInspector] public static float currentSpines;
    public static float spinesPercent;

    private float timeUntilNextSpine;
    private float spineTimer;


    [Header("Inventory")]

    public static int experiencePoints = 0;
    public static int sporesInventory = 0;
    public static int sporeIncubatorContents = 0;
    private static float sporeAccumulator = 0f;
    private static float timeBetweenIncubationSteps = 10f;


    [Header("Legs")]

    //public static float baseWalkSpeed = 2f;
    public static float baseRunMultiplier = 2f;

    public static float baseJumpForce = 5f;

    public static float baseSafeFallSpeed = 7f;

    public static int maxDoubleJumpValue = 0;


    [Header("Thorax")]

    public static float maxHealth = 100f;
    public static float healthRegen = 2f;
    public static float currentHealth { get; private set; }
    public static float healthPercent;

    public static float maxHunger = 100f;
    //public static float baseHungerDecay = 0.1f;
    public static float currentHungerDecay;
    public static float runHungerMultiplier = 3f;
    public static float jumpHungerCost = 0.5f;
    public static float currentHunger;
    private static float hungerPercent;


    [Header("References")]

    AudioSource audioSource;
    AudioManager audioManager;

    private ScreenDamageIndicator screenDamage;
    private PlayerController playerController;

    public Image healthBar;
    public Image hungerBar;
    public Image spinesBar;
    public Text sporesText;
    public Text sporesIncubatorText;
    public Text doubleJumpText;
    public Text experiencePointsText;

    //private Recoil recoilScript;



    void Start()
    {
        //recoilScript = transform.Find("CameraRotation/CameraRecoil").GetComponent<Recoil>();
        screenDamage = GetComponent<ScreenDamageIndicator>();
        audioSource = GetComponent<AudioSource>();
        audioManager = FindObjectOfType<AudioManager>();

        playerController = GetComponent<PlayerController>();


        DefineSkill(_index: 0, _skillName: "Spines Per Shot",
            _skillDescription: "Increases the number of spines per shot, but makes fire rate and accuracy worse.",
            _baseValue: 1, _valueChangePerLevel: 1, _baseUpgradeCost: 10, _maxLevel: 5);

        DefineSkill(_index: 1, _skillName: "Spine Regen",
            _skillDescription: "",
            _baseValue: 2, _valueChangePerLevel: 1, _baseUpgradeCost: 5, _maxLevel: 10);

        DefineSkill(_index: 2, _skillName: "Max Spines",
            _skillDescription: "",
            _baseValue: 10, _valueChangePerLevel: 5, _baseUpgradeCost: 1, _maxLevel: 10);

        DefineSkill(_index: 3, _skillName: "Fire Rate",
            _skillDescription: "",
            _baseValue: 3, _valueChangePerLevel: 0.5f, _baseUpgradeCost: 5, _maxLevel: 10);

        DefineSkill(_index: 4, _skillName: "Spine Speed",
            _skillDescription: "",
            _baseValue: 10, _valueChangePerLevel: 2f, _baseUpgradeCost: 2, _maxLevel: 5);

        DefineSkill(_index: 5, _skillName: "Incubator Efficiency",
            _skillDescription: "Your spore incubation gland can perform more efficiently, yielding greater mutagen harvests.",
            _baseValue: 1, _valueChangePerLevel: 0.2f, _baseUpgradeCost: 1, _maxLevel: 10);

        DefineSkill(_index: 6, _skillName: "Suffering Tolerance",
            _skillDescription: "",
            _baseValue: 100, _valueChangePerLevel: 10, _baseUpgradeCost: 1, _maxLevel: 10);

        DefineSkill(_index: 7, _skillName: "Double Jumps",
            _skillDescription: "",
            _baseValue: 0, _valueChangePerLevel: 1, _baseUpgradeCost: 50, _maxLevel: 4);

        DefineSkill(_index: 8, _skillName: "Movement Speed",
            _skillDescription: "",
            _baseValue: 2, _valueChangePerLevel: 0.2f, _baseUpgradeCost: 1, _maxLevel: 10);

        DefineSkill(_index: 9, _skillName: "Base Metabolism",
            _skillDescription: "",
            _baseValue: 0.1f, _valueChangePerLevel: -0.01f, _baseUpgradeCost: 10, _maxLevel: 5);


        spinesPerShot = skills[0];
        spineRegen = skills[1];
        maxSpines = skills[2];
        spineFireRate = skills[3];
        spineSpeed = skills[4];
        incubatorEfficiency = skills[5];
        sufferingTolerance = skills[6];
        doubleJumps = skills[7];
        baseWalkSpeed = skills[8];
        baseHungerDecay = skills[9];





        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentSpines = maxSpines.GetValue();

    }


    void Update()
    {
        maxDoubleJumpValue = (int)doubleJumps.GetValue();

        if (Input.GetKeyDown("1") && sporesInventory > 0)
        {
            EatSpore();
        }

        IncubateSpores();

        UpdateSpines();

        UpdatePlayerHunger();

        UpdatePlayerHealth();

        UpdateUI();

    }


    void UpdateUI()
    {
        hungerPercent = currentHunger / maxHunger;
        hungerBar.fillAmount = hungerPercent;

        spinesPercent = currentSpines / maxSpines.GetValue();
        spinesBar.fillAmount = spinesPercent;

        sporesIncubatorText.text = "(1) Incubator: " + sporeIncubatorContents.ToString();
        sporesText.text = "Spores: " + sporesInventory.ToString();
        doubleJumpText.text = "Double Jumps: " + maxDoubleJumpValue.ToString();
        experiencePointsText.text = "XP: " + experiencePoints.ToString();
    }


    void EatSpore()
    {
        sporesInventory--;
        sporeIncubatorContents++;
        PlayerStats.currentHunger += 30f;
    }


    public void IncubateSpores()
    {
       
        sporeAccumulator += sporeIncubatorContents * Time.deltaTime / (timeBetweenIncubationSteps / incubatorEfficiency.GetValue());

        if (sporeAccumulator > 1.0f)
        {
            sporeAccumulator -= 1.0f;
            experiencePoints++;
        }

    }


    void UpdateSpines()
    {

        timeUntilNextSpine = 1 / spineRegen.GetValue();

        spineTimer -= Time.deltaTime;

        if (currentSpines < maxSpines.GetValue() && spineTimer < 0f)
        {
            currentSpines++;
            spineTimer = timeUntilNextSpine;
        }


    }


    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;

        //recoilScript.RecoilJump(damage / 2);

        float bloodAmount = damage / 50 + 0.2f;

        screenDamage.MakeScreenBloody(bloodAmount);

        float hurtSoundVolume = Mathf.Clamp(bloodAmount, 0.2f, 0.6f);

        audioManager.PlayAtVolume("PlayerHurt", bloodAmount);



        if (currentHealth <= 0f)
        {
            KillPlayer();
        }
    }


    public static void KillPlayer()
    {
        Application.Quit();
    }


    void UpdatePlayerHunger()
    {

        if (playerController.isRunning == true)
        {
            currentHungerDecay = baseHungerDecay.GetValue() * runHungerMultiplier;
        }
        else
        {
            currentHungerDecay = baseHungerDecay.GetValue();
        }


        currentHunger -= Time.deltaTime * currentHungerDecay;


        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }


        if (currentHunger < 0)
        {
            currentHunger = 0;
        }


    }



    public void IncreaseHungerFromJumping()
    {
        currentHunger -= jumpHungerCost;

        if (currentHunger < 0)
        {
            currentHunger = 0;
        }
    }



    void UpdatePlayerHealth()
    {
        maxHealth = sufferingTolerance.GetValue();

        if (hungerPercent > 0.5f)
        {
            currentHealth += Time.deltaTime * (healthRegen * hungerPercent * hungerPercent);
        }
        else if (hungerPercent < 0.1f)
        {
            currentHealth -= Time.deltaTime * (healthRegen * (1f - hungerPercent) * (1f - hungerPercent));
        }


        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }


        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        healthPercent = currentHealth / maxHealth;

        healthBar.fillAmount = healthPercent;

    }


    void DefineSkill(
        int _index, 
        string _skillName, 
        string _skillDescription, 
        float _baseValue, 
        float _valueChangePerLevel, 
        int _baseUpgradeCost,
        int _maxLevel
        )
    {
        skills[_index].skillName = _skillName;
        skills[_index].skillDescription = _skillDescription;
        skills[_index].baseValue = _baseValue;
        skills[_index].valueChangePerLevel = _valueChangePerLevel;
        skills[_index].baseUpgradeCost = _baseUpgradeCost;
        skills[_index].maxLevel = _maxLevel;
    }




}
