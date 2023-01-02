using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    float walkSpeed;
    float runSpeed;
    public bool isRunning;

    float minSafeFallSpeed;
    float jumpForce;

    public float footStepDelay = 0.2f;
    public float gravity;
    public float lateralSprintSpeedPenalty = 0.75f;


    [Header("References")]

    public AudioSource audioSource;
    public AudioClip stepSound;
    public Transform headCheckPosition;
    public Transform groundCheckPosition;
    public AudioManager audioManager;


    private CharacterController characterController;
    private Transform cameraTransform;
    private ScreenDamageIndicator screenDamage;
    private PlayerStats playerStats;
    private Recoil recoilScript;

    private PlayerInput playerInput;


    [Header("Other")]

    public float groundCheckRadius;
    public float headCheckRadius;
    public LayerMask groundMask;


    //Private variables


    private Vector3 moveDirection;

    private float moveSpeed;
    private float timeUntilNextFootstep = 0f;
    private bool isGrounded;
    private int doubleJumpCount = 0;
    private bool hitHead;
    private Vector3 velocity;

    private float jumpRecoilAmount = 15;
    private float landRecoilAmount = -10;
    //private float strafeRecoilAmount = 3;


    void Awake()
    {
        screenDamage = GetComponent<ScreenDamageIndicator>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        playerInput = GetComponent<PlayerInput>();
        audioManager = FindObjectOfType<AudioManager>();
        cameraTransform = Camera.main.transform;
        recoilScript = transform.GetComponent<Recoil>();
    }


    void Update()
    {
        GetPlayerStats();

        KillPlayerBelowWorldLimit();

        //recoilScript.RecoilStrafe(strafeRecoilAmount * -playerInput.xInput);


        SetPlayerMoveSpeed();

        isGrounded = Physics.CheckSphere(groundCheckPosition.position, groundCheckRadius, groundMask);
        hitHead = Physics.CheckSphere(headCheckPosition.position, headCheckRadius, groundMask);


        if (isGrounded)
        {
            PlayerHitsGround();

            PlayFootsteps();

            doubleJumpCount = 0;                            //reset double jumps
            characterController.stepOffset = 0.2f;          //allows player to climb stairs
        }
        
        moveDirection = transform.right * playerInput.xInput * moveSpeed * lateralSprintSpeedPenalty + transform.forward * playerInput.zInput * moveSpeed;
        

        if (hitHead && velocity.y > 0)
        {
            velocity.y = -0.1f;
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;

            timeUntilNextFootstep = footStepDelay;
            characterController.stepOffset = 0.0001f;       //prevents player from catching on edges when jumping up near them
        }


        if (playerInput.jumpInput && isGrounded)
        {
            velocity.y += jumpForce;
            audioManager.Play("PlayerJump");
            doubleJumpCount = 0;
            playerStats.IncreaseHungerFromJumping();
            recoilScript.RecoilJump(jumpRecoilAmount);


        }
        else if (playerInput.jumpInput && doubleJumpCount < PlayerStats.doubleJumps.GetValue())
        {
            velocity.y += jumpForce;
            audioManager.Play("PlayerDoubleJump");
            doubleJumpCount ++;
            playerStats.IncreaseHungerFromJumping();
            recoilScript.RecoilJump(jumpRecoilAmount);
        }

        //clamp maximum vertical speed from jumping
        if (velocity.y > jumpForce)
        {
            velocity.y = jumpForce;
        }

        characterController.Move(moveDirection * Time.deltaTime + velocity * Time.deltaTime);

    }


    void GetPlayerStats()
    {
        walkSpeed = PlayerStats.baseWalkSpeed.GetValue();
        runSpeed = walkSpeed * PlayerStats.baseRunMultiplier;
        minSafeFallSpeed = PlayerStats.baseSafeFallSpeed;
        jumpForce = PlayerStats.baseJumpForce;
    }


    void PlayerHitsGround()
    {
        if (velocity.y < -1f)
        {
            float playbackVolume = Mathf.Clamp(-velocity.y / 10 + 0.3f, 0.5f, 1f);
            audioManager.PlayAtVolume("PlayerLanding", playbackVolume);
            recoilScript.RecoilJump(landRecoilAmount);
        }

        //fall damage calculation
        if (velocity.y < -minSafeFallSpeed)                
        {
            float fallDamage = Mathf.Ceil((-velocity.y - minSafeFallSpeed) * (-velocity.y - minSafeFallSpeed));

            playerStats.ApplyDamage(2 * fallDamage);
        }

        //slowly push player down so they keep in contact with the ground
        if (velocity.y < 0f)
        {
            velocity.y = -0.5f;         
        }
    }


    void KillPlayerBelowWorldLimit()
    {
        if (transform.position.y < -400)
        {
            PlayerStats.KillPlayer();
        }
    }


    void SetPlayerMoveSpeed()
    {
        if (playerInput.sprintInput && playerInput.zInput == 1)
        {
            moveSpeed = runSpeed;
            isRunning = true;
        }
        else
        {
            moveSpeed = walkSpeed;
            isRunning = false;
        }
    }


    void PlayFootsteps()
    {
        if (moveDirection != new Vector3(0f, 0f, 0f) && Mathf.Abs(velocity.y) < 0.52)
        {
            timeUntilNextFootstep -= Time.deltaTime;

            if (timeUntilNextFootstep <= 0f)
            {
                audioManager.Play("PlayerStep");
                timeUntilNextFootstep = footStepDelay + 0.25f / moveSpeed;
            }
        }
    }

}
