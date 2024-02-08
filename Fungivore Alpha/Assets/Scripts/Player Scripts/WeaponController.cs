using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletSpawn;
    public Rigidbody bulletPrefab;

    public GameObject shotParticleEffect;

    Rigidbody playerRigidbody;

    private float spread;
    private float shotTimer;
    private float timeUntilNextShot;

    private Recoil recoilScript;
    private PlayerInput playerInput;
    private PlayerStats playerStats;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        recoilScript = transform.GetComponent<Recoil>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        //recoilScript = transform.Find("CameraRotation/CameraRecoil").GetComponent<Recoil>();
    }


    void Update()
    {
        shotTimer -= Time.deltaTime;

        if (playerInput.shootInput)
        {
            if (shotTimer < 0f)
            {
                FireGun();

                var baseTimeBetweenShots = 1 / playerStats.GetStatValue("Fire Rate");
                var modifiedTimeBetweenShots = baseTimeBetweenShots * (1 + ((playerStats.GetStatValue("Spines Per Shot") - 1) * 0.5f));

                shotTimer = modifiedTimeBetweenShots;
            }
        }
    }


    void FireGun()
    {
        var spinesPerShot = playerStats.GetStatValue("Spines Per Shot");


        if (PlayerStats.currentSpines >= spinesPerShot)
        {
            AudioManager.Instance.Play("SpineSwish");

            for (int i = 0; i < spinesPerShot; i++)
            {
                ShootBullet();
                PlayerStats.currentSpines--;
            }

            AudioManager.Instance.Play("PlayerShoot");
        }
    }


    void ShootBullet()
    {
        //recoilScript.RecoilFire(PlayerStats.verticalRecoil, PlayerStats.horizontalRecoil);
        recoilScript.TranslationRecoil();

        Vector3 directionWithoutSpread = bulletSpawn.transform.forward;

        spread = PlayerStats.spineSpread + playerStats.GetStatValue("Spines Per Shot") / 100;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, z);

        var bullet = ObjectPooler.current.GetPooledObject(0);

        bullet.SetActive(true);
        bullet.transform.rotation = bulletSpawn.transform.rotation;
        bullet.transform.position = bulletSpawn.transform.position;

        var rb = bullet.GetComponent<Rigidbody>();
        bullet.GetComponent<BulletController>().hasCollided = false;

        rb.velocity = (playerRigidbody.velocity + directionWithSpread * playerStats.GetStatValue("Spine Speed"));

    }


}
