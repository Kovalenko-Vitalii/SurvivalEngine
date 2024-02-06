using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float gravityForce;
    public float staminaSpeed;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float speedUpSmoothness;
    public float speedDownSmoothness;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float crouchCenter;
    private float startYScale;
    private float crouchStartCenter;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [Header("Boring nerd animator stuff")]
    public Animator anim;
    public float animationInterpolation;

    [Header("===Links===")]
    public Transform orientation;
    public Transform topHead;
    public CharacterController controller;

    [Header("===MOVING STATE===")]
    public MovementState state;
    public PlayerCondition playerCondition;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 playerVelocity;
    private bool isCrouching = false;
    public static bool canMove;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        controller = transform.GetComponent<CharacterController>();
        startYScale = transform.GetComponent<CharacterController>().height;
        crouchStartCenter = transform.GetComponent<CharacterController>().center.y;

        readyToJump = true;
        canMove = true;

        moveSpeed = walkSpeed;
        playerHeight = startYScale;

        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        grounded = controller.isGrounded;

        StateHandler();
        MovePlayer();
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            transform.GetComponent<CharacterController>().height = crouchYScale;
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey) && playerCondition.playerStamina > 0)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            HeadBobScript.Smooth = 70;
            
            playerCondition.playerStamina -= playerCondition.staminaRunCost * Time.deltaTime;
            if (playerCondition.playerStamina <= 0) playerCondition.playerStamina = 0;

            if (playerCondition.recharge != null) StopCoroutine(playerCondition.recharge);
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            HeadBobScript.Smooth = 28;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        if (Input.GetKeyUp(sprintKey))
        {
            if (playerCondition.recharge != null) StopCoroutine(playerCondition.recharge);
            playerCondition.recharge = StartCoroutine(playerCondition.rechargeStamina());
        }
    }

    private void MovePlayer()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded & playerCondition.playerStamina > playerCondition.staminaJumpCost)
        {
            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.GetComponent<CharacterController>().height = startYScale;
        }

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized * moveSpeed;

        // Apply gravity to simulate falling
        if (!controller.isGrounded)
        {
            playerVelocity.y -= gravityForce * Time.deltaTime;
        }

        moveDirection += playerVelocity; // Apply the player's vertical velocity (jump force and gravity).

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void Jump()
    {
        playerVelocity.y = jumpForce;
        playerCondition.playerStamina -= playerCondition.staminaJumpCost;
        if (playerCondition.playerStamina <= 0) playerCondition.playerStamina = 0;
        if (playerCondition.recharge != null) StopCoroutine(playerCondition.recharge);
        playerCondition.recharge = StartCoroutine(playerCondition.rechargeStamina());
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    bool checkHeight()
    {
        if (Physics.Raycast(topHead.position, Vector3.up, 1f))
        {
            return false;
        }
        else { return true; }
    }

    IEnumerator jumpCorutine()
    {
        playerVelocity.y = jumpForce;
        anim.SetTrigger("Jump");
        readyToJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        readyToJump = true;
    }
}
