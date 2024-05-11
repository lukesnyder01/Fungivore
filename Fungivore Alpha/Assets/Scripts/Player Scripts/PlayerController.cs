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
    private float currentGravity;
    public float lateralSprintSpeedPenalty = 0.75f;



    [Header("References")]

    //public AudioSource audioSource;
    public AudioClip stepSound;
    public Transform headCheckPosition;
    public Transform groundCheckPosition;

    private CharacterController characterController;
    private Transform cameraTransform;
    private ScreenDamageIndicator screenDamage;
    private PlayerStats playerStats;
    private Recoil recoilScript;

    private PlayerInput playerInput;

    public GameObject beamRings;

    [Header("Other")]

    public float groundCheckRadius;
    public float headCheckRadius;
    public LayerMask groundMask;


    //------------------------------------------------------------------------------
    //Private variables
    //------------------------------------------------------------------------------
    private ConveyorBeam currentBeam;
    private Vector3 currentBeamPos;

    private float maxBeamSpeed = 20f;
    private float beamAcceleration = 1.04f;
    private float playerDeceleration = 0.95f;

    private bool playerInConveyorBeam;
    private Vector3 beamPushForce;
    private Vector3 beamDirection;

    private bool playerIsDashing;

    private Vector3 dashDirection;

    private float currentDashSpeed = 0;
    private float dashAcceleration = 1.2f;
    private float maxDashSpeed = 10f;

    private float dashTimeLength = 0.1f;
    private float dashTimer;

    private float dashCooldown = 0.5f;
    private float dashCooldownTimer;




    private Vector3 beamOrientation;
    private Vector3 targetRingPos;


    private Vector3 moveDirection;

    private float moveSpeed;
    private float timeUntilNextFootstep = 0f;
    private bool isGrounded;
    private int doubleJumpCount = 0;
    private bool hitHead;
    private Vector3 velocity;
    private float currentVerticalSpeed;

    private float jumpRecoilAmount = 15;
    private float landRecoilAmount = -10;
    //private float strafeRecoilAmount = 3;


    private float[] fallSpeedBuffer = new float[5];
    private int fallSpeedBufferIndex = 0;
    private float measuredVerticalSpeed;


    void Awake()
    {
        screenDamage = GetComponent<ScreenDamageIndicator>();
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();
        playerInput = GetComponent<PlayerInput>();

        cameraTransform = Camera.main.transform;
        recoilScript = transform.GetComponent<Recoil>();
        currentGravity = gravity;
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

            if (!playerInConveyorBeam) 
            {
                PlayFootsteps();
            }

            //reset double jumps
            doubleJumpCount = 0;

            //allows player to climb stairs
            characterController.stepOffset = 0.2f;          
        }

        moveDirection = transform.right * playerInput.xInput * moveSpeed * lateralSprintSpeedPenalty + transform.forward * playerInput.zInput * moveSpeed;
        
        HandleGravity();

        HandleDashing();

        HandleConveyorBeams();

        characterController.Move(moveDirection * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerStats.KillPlayer();
        }

    }


    void HandleGravity()
    {
        //keep a buffer of the player's fall speed, find the max value, and use that for fall damage
        fallSpeedBuffer[fallSpeedBufferIndex] = characterController.velocity.y;
        fallSpeedBufferIndex = (fallSpeedBufferIndex + 1) % fallSpeedBuffer.Length;
        measuredVerticalSpeed = Mathf.Min(fallSpeedBuffer);

        

        if (hitHead && measuredVerticalSpeed > 0)
        {
            currentVerticalSpeed = -0.1f;
        }

        if (!isGrounded)
        {
            currentVerticalSpeed += currentGravity * Time.deltaTime;
            timeUntilNextFootstep = footStepDelay;
            characterController.stepOffset = 0.0001f; // prevents player from catching on edges when jumping up near them
        }

        if (isGrounded)
        {
            //currentVerticalSpeed = -0.2f; // slowly push player down so they keep in contact with the ground
        }

        HandleJumping();

        moveDirection.y += currentVerticalSpeed;
    }


    void HandleJumping()
    {
        if (playerInput.jumpInput && isGrounded)
        {
            currentVerticalSpeed += jumpForce;
            AudioManager.Instance.Play("PlayerJump");
            doubleJumpCount = 0;
            playerStats.IncreaseHungerFromJumping();
            recoilScript.RecoilJump(jumpRecoilAmount);
        }
        else if (playerInput.jumpInput && doubleJumpCount < playerStats.GetStatValue("Double Jumps"))
        {
            currentVerticalSpeed += jumpForce;
            AudioManager.Instance.Play("PlayerDoubleJump");
            doubleJumpCount++;
            playerStats.IncreaseHungerFromJumping();
            recoilScript.RecoilJump(jumpRecoilAmount);
        }

        if (currentVerticalSpeed > jumpForce && !playerInConveyorBeam)
        {
            currentVerticalSpeed = jumpForce;
        }
    }


    void HandleDashing()
    {
        dashCooldownTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            playerIsDashing = false;
            dashTimer = 0;
        }


        if (!playerIsDashing)
        {
            currentDashSpeed *= playerDeceleration;

            if (dashCooldownTimer <= 0 && playerInput.dashInput)
            {
                dashDirection = transform.right * playerInput.xInput + transform.forward * playerInput.zInput;

                //defaults the dash to forward if the player isn't touching wasd
                if (dashDirection.magnitude < 0.2)
                {
                    dashDirection = transform.forward;
                }

                currentDashSpeed = 1;
                playerIsDashing = true;

                
                dashTimer = dashTimeLength;
                dashCooldownTimer = dashCooldown;
            }
        }


        if (playerIsDashing)
        {
            if (currentDashSpeed <= maxDashSpeed)
            {
                currentDashSpeed *= dashAcceleration;
            }
        }

        moveDirection += dashDirection * currentDashSpeed;
    }


    void GetPlayerStats()
    {
        walkSpeed = playerStats.GetStatValue("Move Speed");

        runSpeed = walkSpeed * playerStats.GetStatValue("Run Multiplier");


        minSafeFallSpeed = playerStats.GetStatValue("Safe Fall Speed");

        jumpForce = playerStats.GetStatValue("Jump Force");

    }


    void PlayerHitsGround()
    {
        if (currentVerticalSpeed < -1f)
        {
            Debug.Log("Measured fall speed - " + measuredVerticalSpeed + " Current vertical speed - " + currentVerticalSpeed);

            float playbackVolume = Mathf.Clamp(-measuredVerticalSpeed / 10 + 0.3f, 0.5f, 1f);
            AudioManager.Instance.PlayAtVolume("PlayerLanding", playbackVolume);
            recoilScript.RecoilJump(landRecoilAmount);

            if (!playerInConveyorBeam)
            {
                beamPushForce = Vector3.zero;
            }

            currentVerticalSpeed = -0.2f;

            //fall damage calculation
            if (measuredVerticalSpeed < -minSafeFallSpeed && !playerInConveyorBeam)
            {
                float excessFallSpeed = Mathf.Abs(measuredVerticalSpeed) - minSafeFallSpeed;

                float fallDamage = Mathf.Ceil(excessFallSpeed * excessFallSpeed);

                playerStats.ApplyDamage(fallDamage);

                Debug.Log("Fall Damage = " + fallDamage);
            }
        }

        //slowly push player down so they keep in contact with the ground
        if (currentVerticalSpeed <= 0.01f)
        {
            currentVerticalSpeed = -0.2f;
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

        Vector3 moveInput = new Vector3(playerInput.xInput, 0, playerInput.zInput);

        if (moveInput != new Vector3(0f, 0f, 0f) && Mathf.Abs(characterController.velocity.y) < 0.52)
        {
            timeUntilNextFootstep -= Time.deltaTime;

            if (timeUntilNextFootstep <= 0f)
            {
                AudioManager.Instance.Play("PlayerStep");
                timeUntilNextFootstep = footStepDelay + 0.25f / moveSpeed;
            }
        }
    }

    void HandleConveyorBeams()
    {
        if (playerInConveyorBeam)
        {
            if (beamPushForce.magnitude <= maxBeamSpeed)
            {
                beamPushForce *= beamAcceleration;
            }

            HandleConveyorBeamRing();

            // Modify currentVerticalSpeed
            if (beamOrientation.y > 0.5f)
            {
                currentVerticalSpeed = -0.5f;
            }

        }
        else
        {
            beamPushForce *= playerDeceleration;
        }


        // Apply the beam push force to moveDirection
        moveDirection += new Vector3(beamPushForce.x, beamPushForce.y, beamPushForce.z);
    }


    void HandleConveyorBeamRing()
    {
        // Check if the beam is oriented vertically
        if (beamOrientation.y >= 0.5f || beamOrientation.y <= -0.5f)
        {
            targetRingPos = new Vector3(currentBeamPos.x, transform.position.y, currentBeamPos.z);
        }
        else
        {
            // Beam is oriented horizontally
            if (beamOrientation.z >= 0.5f)
            {
                targetRingPos = new Vector3(currentBeamPos.x, beamRings.transform.position.y, transform.position.z);
            }
            else
            {
                targetRingPos = new Vector3(transform.position.x, beamRings.transform.position.y, currentBeamPos.z);
            }
        }

        beamRings.transform.forward = beamOrientation;
        beamRings.transform.position = targetRingPos;

    }


    void SetBeamDirection(Vector3 beamOrientation)
    {
        // Get the forward direction of the transform (assumed to be the player's orientation)
        Vector3 playerDirection = cameraTransform.forward.normalized;

        // Calculate the beam direction based on player's orientation and beamOrientation
        Vector3 beamDirection = new Vector3(
            Mathf.Sign(playerDirection.x) * Mathf.Abs(beamOrientation.x),
            Mathf.Sign(playerDirection.y + 0.8f) * Mathf.Abs(beamOrientation.y),
            Mathf.Sign(playerDirection.z) * Mathf.Abs(beamOrientation.z)
        );

        // Normalize beamDirection to ensure consistent magnitude
        beamDirection = beamDirection.normalized;

        // Assign beamDirection to some beam variable (assuming 'beamPushForce' is a class-level variable)
        beamPushForce = beamDirection;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConveyorBeam"))
        {
            playerInConveyorBeam = true;

            currentBeam = other.GetComponent<ConveyorBeam>();
            currentBeamPos = currentBeam.transform.position;

            maxBeamSpeed = currentBeam.beamSpeed;

            var beamPitch = 0.5f + (maxBeamSpeed / 200);
            AudioManager.Instance.StartLoop("Conveyor", beamPitch);

            SetBeamDirection(currentBeam.beamOrientation);
            beamOrientation = currentBeam.beamOrientation;

            if (beamOrientation.y > 0.5f)
            {
                currentGravity = 0f;
            }

            HandleConveyorBeamRing();
            beamRings.transform.position = targetRingPos;
            beamRings.transform.forward = beamOrientation;
            beamRings.SetActive(true);
        }
        else if (other.CompareTag("KillPlayer"))
        {
            PlayerStats.KillPlayer();
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ConveyorBeam"))
        {
            beamRings.SetActive(false);
            AudioManager.Instance.FadeOut("Conveyor", 0.2f);

            currentGravity = gravity;

            playerInConveyorBeam = false;
        }
    }


}
