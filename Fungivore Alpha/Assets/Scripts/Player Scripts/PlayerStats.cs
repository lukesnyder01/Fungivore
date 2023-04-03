using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private Dictionary<string, float> stats = new Dictionary<string, float>();



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


    [Header("Thorax")]

    public static float healthRegen = 2f;
    public static float currentHealth { get; private set; }
    public static float healthPercent;


    public static float runHungerMultiplier = 3f;
    public static float jumpHungerCost = 0.5f;

    public static float currentEnergy;
    private static float energyPercent;


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



    void Awake()
    {
        InitializeStatsList();


        //recoilScript = transform.Find("CameraRotation/CameraRecoil").GetComponent<Recoil>();
        screenDamage = GetComponent<ScreenDamageIndicator>();
        audioSource = GetComponent<AudioSource>();
        audioManager = FindObjectOfType<AudioManager>();

        playerController = GetComponent<PlayerController>();

        currentHealth = GetStatValue("Max Health");
        currentEnergy = GetStatValue("Max Energy");
        currentSpines = GetStatValue("Max Spines");
    }


    private void InitializeStatsList()
    {
        stats.Add("Max Health", 100f);
        stats.Add("Max Energy", 100f);
        stats.Add("Armor", 0f);


        stats.Add("Walk Speed", 2f);
        stats.Add("Run Multiplier", 2f);

        stats.Add("Spines Per Shot", 1f);
        stats.Add("Fire Rate", 1f);
        stats.Add("Spine Regen", 2f);
        stats.Add("Max Spines", 10f);
        stats.Add("Spine Speed", 10f);

        stats.Add("Double Jumps", 0f);
        stats.Add("Safe Fall Speed", 7f);
        stats.Add("Jump Force", 5f);

        stats.Add("Base Metabolism", 0.1f);
        stats.Add("Incubator Efficiency", 1f);
    }


    public void ApplyModifier(string name, float value)
    {
        if (stats.ContainsKey(name))
        {
            stats[name] += value;
        }
        {
            Debug.Log("Stat " + name + " not found, no modifier applied.");
        }
    }


    public float GetStatValue(string name)
    {
        if (stats.ContainsKey(name))
        {
            return stats[name];
        }
        else
        {
            Debug.Log("Stat " + name + " not found, returning value of 0.");
            return 0f;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown("1") && sporesInventory > 0)
        {
            EatSpore();
        }

        IncubateSpores();

        UpdateSpines();

        UpdatePlayerEnergy();

        UpdatePlayerHealth();

        //UpdateUI();

    }


    void UpdateUI()
    {
        energyPercent = currentEnergy / GetStatValue("Max Energy");
        hungerBar.fillAmount = energyPercent;

        healthPercent = currentHealth / GetStatValue("Max Health");
        healthBar.fillAmount = healthPercent;

        spinesPercent = currentSpines / GetStatValue("Max Spines");

        spinesBar.fillAmount = spinesPercent;

        sporesIncubatorText.text = "(1) Incubator: " + sporeIncubatorContents.ToString();
        sporesText.text = "Spores: " + sporesInventory.ToString();
        doubleJumpText.text = "Double Jumps: " + GetStatValue("Double Jumps").ToString();
        experiencePointsText.text = "XP: " + experiencePoints.ToString();
    }


    void EatSpore()
    {
        sporesInventory--;
        sporeIncubatorContents++;
        PlayerStats.currentEnergy += 30f;
    }


    public void IncubateSpores()
    {
        sporeAccumulator += sporeIncubatorContents * Time.deltaTime / (timeBetweenIncubationSteps / GetStatValue("Incubator Efficiency"));

        if (sporeAccumulator > 1.0f)
        {
            sporeAccumulator -= 1.0f;
            experiencePoints++;
        }
    }


    void UpdateSpines()
    {
        timeUntilNextSpine = 1 / GetStatValue("Spine Regen");

        spineTimer -= Time.deltaTime;

        if (currentSpines < GetStatValue("Max Spines") && spineTimer < 0f)
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


    void UpdatePlayerEnergy()
    {
        float currentEnergyDecay;

        if (playerController.isRunning == true)
        {
            currentEnergyDecay = GetStatValue("Base Metabolism") * runHungerMultiplier;
        }
        else
        {
            currentEnergyDecay = GetStatValue("Base Metabolism");
        }

        currentEnergy -= Time.deltaTime * currentEnergyDecay;


        if (currentEnergy > GetStatValue("Max Energy"))
        {
            currentEnergy = GetStatValue("Max Energy");
        }


        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }

    }



    public void IncreaseHungerFromJumping()
    {
        currentEnergy -= jumpHungerCost;

        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }
    }



    void UpdatePlayerHealth()
    {
        if (energyPercent > 0.5f)
        {
            currentHealth += Time.deltaTime * (healthRegen * energyPercent * energyPercent);
        }
        else if (energyPercent < 0.1f)
        {
            currentHealth -= Time.deltaTime * (healthRegen * (1f - energyPercent) * (1f - energyPercent));
        }


        if (currentHealth > GetStatValue("Max Health"))
        {
            currentHealth = GetStatValue("Max Health");
        }


        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

    }





}
