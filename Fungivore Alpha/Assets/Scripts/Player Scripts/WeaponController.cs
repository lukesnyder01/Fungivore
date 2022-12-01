using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletSpawn;
    public Rigidbody bulletPrefab;

    public GameObject shotParticleEffect;

    CharacterController playerChar;

    private float spread;
    private float shotTimer;
    private float timeUntilNextShot;

    //public PlayerStats playerStats;
    private Recoil recoilScript;


    void Start()
    {
        playerChar = GameObject.Find("Player").GetComponent<CharacterController>();
        //playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        recoilScript = transform.Find("CameraRotation/CameraRecoil").GetComponent<Recoil>();

    }


    void Update()
    {
        if (!PauseMenu.gameIsPaused)
        {

            shotTimer -= Time.deltaTime;


            if (Input.GetMouseButton(0))
            {
                if (shotTimer < 0f)
                {
                    FireGun();

                    var baseTimeBetweenShots = 1 / PlayerStats.spineFireRate.GetValue();
                    var modifiedTimeBetweenShots = baseTimeBetweenShots * (1 + ((PlayerStats.spinesPerShot.GetValue() - 1) * 0.5f));

                    shotTimer = modifiedTimeBetweenShots;
                }
            }
        }
    }


    void FireGun()
    {
        var spinesPerShot = PlayerStats.spinesPerShot.GetValue();


        if (PlayerStats.currentSpines >= spinesPerShot)
        {
            FindObjectOfType<AudioManager>().Play("SpineSwish");

            for (int i = 0; i < spinesPerShot; i++)
            {
                ShootBullet();
                PlayerStats.currentSpines--;
                
            }

            FindObjectOfType<AudioManager>().Play("PlayerShoot");
        }
    }


    void ShootBullet()
    {
        recoilScript.RecoilFire(PlayerStats.verticalRecoil, PlayerStats.horizontalRecoil);

        Vector3 directionWithoutSpread = bulletSpawn.transform.forward;

        spread = PlayerStats.spineSpread + PlayerStats.spinesPerShot.GetValue() / 100;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, z);


        /*
        var particles = Instantiate(shotParticleEffect, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        particles.transform.parent = transform;
        */


        var bullet = ObjectPooler.current.GetPooledObject(0);

        bullet.SetActive(true);

        bullet.transform.rotation = bulletSpawn.transform.rotation;
        bullet.transform.position = bulletSpawn.transform.position;

        var rb = bullet.GetComponent<Rigidbody>();

        rb.velocity = (playerChar.velocity + directionWithSpread * PlayerStats.spineSpeed.GetValue());

        //bullet.GetComponent<Rigidbody>().velocity = playerVelocity;

        //rb.AddForce(directionWithSpread * playerStats.spineSpeed.GetValue(), ForceMode.Impulse);

        



    }


}
