using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum State { 
        Idle,
        HuntForPlayer,
        FloatNearPlayer,
        ChargeAtPlayer,
    }


    public float thrust;
    public float turnSpeed = 1f;

    public float followDistance;

    public float detectionRange;
    public float detectionRadius = 0.7f;

    public float raycastDistToFindEmpty = 4f;


    public float despawnRange = 100f;

    public GameObject deathEffect;
    public GameObject hitEffect;

    public float wanderSpeed = 2f;
    public float chargeSpeed = 3f;
    public float rapidEvasionSpeed = 5f;

    public int maxHitPoints;
    public int currentHitPoints;

    public float collisionDamageDealt = 30f;
    private bool collidedWithPlayer = false;

    private GameObject playerCamera;
    private Transform cameraTransform;
   
    private GameObject player;
    private Transform playerTransform;
    private Vector3 playerPosition;

    private float distanceFromPlayer;
    private Vector3 directionToPlayer;
    private bool canSeePlayer;


    [HideInInspector]
    public Rigidbody rb;


    [HideInInspector]
    public List<Vector3> openDirection = new List<Vector3>();


    public float shotTimer;

    public float timeBeforeChargeAttack = 30f;

    public bool continuousRotation;
    public float rotationSpeed;

    private State state;





    void Start()
    {
        currentHitPoints = maxHitPoints;

        rb = GetComponent<Rigidbody>();

        playerCamera = GameObject.Find("Player Camera");
        cameraTransform = playerCamera.transform;

        player = GameObject.Find("Player");
        playerTransform = player.transform;

        state = State.Idle;

    }


    public virtual void FixedUpdate()
    {
        shotTimer -= Time.deltaTime;


        if (currentHitPoints <= 0)
        {
            HitPointsAtZero();
        }


        if (currentHitPoints < maxHitPoints)
        {
            timeBeforeChargeAttack = 0f;
        }


        if (timeBeforeChargeAttack <= 0f)
        {
            followDistance = 0f;
        }


        playerPosition = playerTransform.position;

        distanceFromPlayer = Vector3.Distance(transform.position, playerPosition);

        directionToPlayer = (playerPosition - transform.position).normalized;


        if (distanceFromPlayer > despawnRange)
        {
            Destroy(gameObject);
        }

        if (continuousRotation)
        {
            Spin();
        }

        PointAtPlayer();

        switch (state)
        {
            default:

            case State.Idle:

                Hover();
                FindOpenDirection();
                EvasiveMove(wanderSpeed);

                if (distanceFromPlayer < detectionRange)
                {
                    state = State.HuntForPlayer;
                }
                
                break;

            case State.HuntForPlayer:

                if (distanceFromPlayer > detectionRange)
                {
                    state = State.Idle;
                }

                if (CanSeePlayer())
                {
                    state = State.FloatNearPlayer;
                }
                else
                {
                    FindOpenDirection();
                    EvasiveMove(rapidEvasionSpeed);

                    if (distanceFromPlayer > followDistance)
                    {
                        MoveTowardsPlayer(wanderSpeed);
                    }
                    if (distanceFromPlayer < followDistance)
                    {
                        timeBeforeChargeAttack -= Time.deltaTime;
                        MoveAwayFromPlayer();
                    }

                }

                break;

            case State.FloatNearPlayer:

                if (currentHitPoints < maxHitPoints || timeBeforeChargeAttack <= 0f)
                {
                    state = State.ChargeAtPlayer;
                }

                if (distanceFromPlayer < followDistance)
                {
                    timeBeforeChargeAttack -= Time.deltaTime;
                    MoveAwayFromPlayer();
                }
                else
                {
                    MoveTowardsPlayer(wanderSpeed);
                }

                if (!CanSeePlayer())
                {
                    state = State.HuntForPlayer;
                }

                break;

            case State.ChargeAtPlayer:

                if (CanSeePlayer())
                {
                    MoveTowardsPlayer(chargeSpeed + (distanceFromPlayer / 3));
                }
                else
                {
                    state = State.HuntForPlayer;
                }
                break;
        }

    }


    void PointAtPlayer()
    {
        transform.forward = Vector3.Slerp(transform.forward, directionToPlayer, Time.deltaTime * turnSpeed);

        //transform.LookAt(playerTransform, transform.up);
    }


    public virtual void ShootAtPlayer()
    { 
        //override later
    }


    bool CanSeePlayer()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, detectionRadius, directionToPlayer, out hit, detectionRange))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    void MoveForward(float thrustMultiplier)
    {
        rb.AddForce(transform.forward * thrust * thrustMultiplier);
    }


    void FindOpenDirection()
    {
        RaycastHit hit;

        openDirection.Clear();

        if (!(Physics.Raycast(transform.position, transform.right, out hit, raycastDistToFindEmpty)))
        {
            openDirection.Add(transform.right);
        }
        if (!(Physics.Raycast(transform.position, -transform.right, out hit, raycastDistToFindEmpty)))
        {
            openDirection.Add(-transform.right);
        }
        if (!(Physics.Raycast(transform.position, transform.up, out hit, raycastDistToFindEmpty)))
        {
            openDirection.Add(transform.up);
        }
        if (!(Physics.Raycast(transform.position, -transform.up, out hit, raycastDistToFindEmpty)))
        {
            openDirection.Add(-transform.up);
        }
    }


    void EvasiveMove(float thrustMultiplier)
    {
        if (openDirection.Count != 0)
        { 
            var selectedDirection = Random.Range(0, openDirection.Count);

            rb.AddForce(openDirection[selectedDirection] * thrust * thrustMultiplier);

        }
    }


    public void MoveTowardsPlayer(float thrustMultiplier)
    {
        rb.AddForce(directionToPlayer * thrust * thrustMultiplier);
    }


    void MoveAwayFromPlayer()
    {
        rb.AddForce(Vector3.up * thrust / 2.0f);
        rb.AddForce(-directionToPlayer * (rapidEvasionSpeed * thrust - distanceFromPlayer));
    }


    public virtual void Hover()
    {
        rb.AddForce(Vector3.up * Mathf.Cos(Time.time) * thrust / 20.0f);
    }


    public void Spin()
    {
        rb.AddTorque(transform.forward * Random.Range(-rotationSpeed, rotationSpeed));
    }


    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (collidedWithPlayer == false)
            {
                collidedWithPlayer = true;
                FindObjectOfType<AudioManager>().Play("WraithSmash");

                HitPointsAtZero();

                //Destroy(gameObject);
                //Instantiate(deathEffect, transform.position, other.transform.rotation);

                var playerStats = other.gameObject.GetComponent<PlayerStats>();
                playerStats.ApplyDamage(collisionDamageDealt);
            }
        }
        else if (other.gameObject.tag == "PlayerBullet")
        {
            //Destroy(other.gameObject);

            currentHitPoints--;

            Instantiate(hitEffect, transform.position, other.transform.rotation);

            FindObjectOfType<AudioManager>().Play("SpineHit");

        }
    }


    public virtual void HitPointsAtZero()
    {
        Destroy(gameObject);

        Instantiate(deathEffect, transform.position, transform.rotation);

        FindObjectOfType<AudioManager>().Play("PlayerHurt");

    }



}
