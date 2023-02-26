using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] public float maxHealth = 1f;

    public float Health { get; set; }

    [HideInInspector]
    public enum State { 
        Idle,
        Hunt,
        MaintainDistance,
        Charge,
    }


    public float thrust = 2f;
    public float turnSpeed = 2f;


    public float minimumDistance = 1f;

    public float detectionRange = 40f;
    public float detectionRadius = 0.01f;

    public float raycastDistToFindEmpty = 4f;

    public float despawnRange = 100f;

    public GameObject deathEffect;

    public float wanderSpeed = 1.5f;
    public float chargeSpeed = 2f;
    public float rapidEvasionSpeed = 5f;

    public float collisionDamageDealt = 10f;

    public float rotationSpeed = 0.2f;


    [HideInInspector]
    public GameObject playerCamera;
    [HideInInspector]
    public Transform cameraTransform;


    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Transform playerTransform;
    [HideInInspector]
    public Vector3 playerPosition;

    [HideInInspector]
    public float distanceFromPlayer;
    [HideInInspector]
    public Vector3 directionToPlayer;
    [HideInInspector]
    public bool canSeePlayer;

    private bool collidedWithPlayer = false;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public List<Vector3> openDirection = new List<Vector3>();

    [HideInInspector]
    public State state;


    void Awake()
    {
        Health = maxHealth;

        rb = GetComponent<Rigidbody>();

        playerCamera = GameObject.Find("Player Camera");
        cameraTransform = playerCamera.transform;

        player = GameObject.Find("Player");
        playerTransform = player.transform;

        state = State.Idle;

    }


    public virtual void FixedUpdate()
    {
        playerPosition = playerTransform.position;

        distanceFromPlayer = Vector3.Distance(transform.position, playerPosition);

        directionToPlayer = (playerPosition - transform.position).normalized;

        if (distanceFromPlayer > despawnRange)
        {
            Destroy(gameObject);
        }

        UpdateStateMachine();

    }

    public virtual void UpdateStateMachine()
    {
        if (rotationSpeed > 0f)
        {
            Spin(Random.Range(-rotationSpeed, rotationSpeed));
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
                    state = State.Hunt;
                }

                break;

            case State.Hunt:

                if (distanceFromPlayer > detectionRange)
                {
                    state = State.Idle;
                }

                if (CanSeePlayer())
                {
                    state = State.MaintainDistance;
                }
                else
                {
                    FindOpenDirection();
                    EvasiveMove(rapidEvasionSpeed);

                    if (distanceFromPlayer > minimumDistance)
                    {
                        MoveTowardsPlayer(wanderSpeed);
                    }
                    if (distanceFromPlayer < minimumDistance)
                    {
                        EvadePlayer();
                    }
                }

                break;

            case State.MaintainDistance:

                if (Health < maxHealth)
                {
                    state = State.Charge;
                }

                if (distanceFromPlayer < minimumDistance)
                {
                    EvadePlayer();
                }
                else
                {
                    MoveTowardsPlayer(wanderSpeed);
                }

                if (!CanSeePlayer())
                {
                    state = State.Hunt;
                }

                break;

            case State.Charge:

                if (CanSeePlayer())
                {
                    MoveTowardsPlayer(chargeSpeed + (distanceFromPlayer / 3));
                }
                else
                {
                    state = State.Hunt;
                }
                break;
        }
    }



    public void PointAtPlayer()
    {

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, transform.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }


    public bool CanSeePlayer()
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


    public void MoveForward(float thrustMultiplier)
    {
        rb.AddForce(transform.forward * thrust * thrustMultiplier);
    }


    public void FindOpenDirection()
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


    public void EvasiveMove(float thrustMultiplier)
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


    public void EvadePlayer()
    {
        rb.AddForce(Vector3.up * thrust / 2.0f);
        rb.AddForce(-directionToPlayer * rapidEvasionSpeed * rapidEvasionSpeed);
    }


    public virtual void Hover()
    {
        rb.AddForce(Vector3.up * Mathf.Cos(Time.time) * thrust / 20.0f);
    }


    public void Spin(float speed)
    {
        rb.AddTorque(transform.forward * speed);
    }


    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!collidedWithPlayer)
            {
                collidedWithPlayer = true;

                Die();

                FindObjectOfType<AudioManager>().Play("WraithSmash");
                var playerStats = other.gameObject.GetComponent<PlayerStats>();
                playerStats.ApplyDamage(collisionDamageDealt);
            }
        }
    }


    public void Damage(float damage)
    {
        Health -= damage;

        FindObjectOfType<AudioManager>().Play("SpineHit");

        if (Health <= 0)
        {
            Die();
        }
    }


    public virtual void Die()
    {
        Destroy(gameObject);
        Instantiate(deathEffect, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("PlayerHurt");
    }



}
