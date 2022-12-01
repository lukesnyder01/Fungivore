using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Enemy
{
    public float timeBetweenShots;

    public GameObject projectile;


    public override void ShootAtPlayer()
    {

        if (shotTimer <= 0)
        {
            var newProjectile = Instantiate(projectile, transform.position + (-transform.forward * 2), transform.rotation);
            shotTimer = timeBetweenShots;
        }


        
    }


}
