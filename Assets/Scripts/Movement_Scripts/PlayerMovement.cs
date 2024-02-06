using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public PlayerCondition playerCondition;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float speedUpSmoothness;
    public float speedDownSmoothness;
    public float groundDrag;
    float moveSpeed;
    float horizontalInput;
    float verticalInput;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float crouchCenter;
    float startYScale;
    float crouchStartCenter;
    

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [Header("Animation settings")]
    public Animator anim;
    public float animationInterpolation;

    [Header("Links")]
    public Transform orientation;
    public Transform topHead;
    CharacterController controller;

    [Header("Other information")]
    public float gravityForce;
    public MovementState state;
    Vector3 playerVelocity;
    bool isCrouching = false;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool isMoving;
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
        moveSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;      
    }

    private void Update()
    {
        MovePlayer();
        StateHandler(); 
    }

    private void StateHandler()
    {
        // Toggle crouch
        if (Input.GetKeyDown(crouchKey)) isCrouching = !isCrouching;

        // stop crouch
        if (!isCrouching && checkHeight())
        {
            anim.SetBool("Crouching", false);
            transform.GetComponent<CharacterController>().height = startYScale;
            transform.GetComponent<CharacterController>().center = new Vector3(transform.GetComponent<CharacterController>().center.x, crouchStartCenter, transform.GetComponent<CharacterController>().center.z);
        }
        else  isCrouching = true; 

        // Mode - Crouching
        if (isCrouching)
        {
            anim.SetBool("Crouching", true);
            transform.GetComponent<CharacterController>().height = crouchYScale;
            transform.GetComponent<CharacterController>().center = new Vector3(transform.GetComponent<CharacterController>().center.x, crouchCenter, transform.GetComponent<CharacterController>().center.z);

            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
            
            anim.SetFloat("xCrouching", Mathf.Lerp(anim.GetFloat("xCrouching"), Input.GetAxis("Horizontal"), animationInterpolation * Time.deltaTime));
            anim.SetFloat("yCrouching", Mathf.Lerp(anim.GetFloat("yCrouching"), Input.GetAxis("Vertical"), animationInterpolation * Time.deltaTime)); 
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey) && playerCondition.playerStamina > 0)
        {
            if (Input.GetKey(jumpKey) && grounded && readyToJump && playerCondition.playerStamina > playerCondition.staminaJumpCost) StartCoroutine(jumpCorutine());      

            state = MovementState.sprinting;
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, Time.deltaTime * speedUpSmoothness);
            playerCondition.playerStamina -= playerCondition.staminaRunCost * Time.deltaTime;

            if (playerCondition.playerStamina <= 0) playerCondition.playerStamina = 0;
            if (playerCondition.recharge != null) StopCoroutine(playerCondition.recharge);
          
            anim.SetFloat("x", Mathf.Lerp(anim.GetFloat("x"), Input.GetAxis("Horizontal") * 1.5f, animationInterpolation * Time.deltaTime));
            anim.SetFloat("y", Mathf.Lerp(anim.GetFloat("y"), Input.GetAxis("Vertical") * 1.5f, animationInterpolation * Time.deltaTime));
        }

        // Mode - Walking
        else if (grounded)
        {
            if (Input.GetKey(jumpKey) && grounded && readyToJump && playerCondition.playerStamina > playerCondition.staminaJumpCost) StartCoroutine(jumpCorutine());

            state = MovementState.walking;
            moveSpeed = walkSpeed;
            
            anim.SetFloat("x", Mathf.Lerp(anim.GetFloat("x"), Input.GetAxis("Horizontal"), animationInterpolation * Time.deltaTime));
            anim.SetFloat("y", Mathf.Lerp(anim.GetFloat("y"), Input.GetAxis("Vertical"), animationInterpolation * Time.deltaTime));
        }

        // Mode - Air
        else state = MovementState.air;
        
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
        grounded = controller.isGrounded;

        Vector3 cameraForward = orientation.forward;
        cameraForward.y = 0;

        // Calculate the move direction by combining the projected camera forward and right vectors
        moveDirection = cameraForward.normalized * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized * moveSpeed;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, orientation.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        if (!controller.isGrounded) playerVelocity.y -= gravityForce * Time.deltaTime;// Apply gravity to simulate falling

        moveDirection += playerVelocity; // Apply the player's vertical velocity (jump force and gravity).
        controller.Move(moveDirection * Time.deltaTime);
    }

    bool checkHeight()
    {     
        if(Physics.Raycast(topHead.position, Vector3.up, 1f)) return false;
        else return true; 
    }

    IEnumerator jumpCorutine()
    {
        playerVelocity.y = jumpForce;
        anim.SetTrigger("Jump");
        readyToJump = false;

        playerCondition.playerStamina -= playerCondition.staminaJumpCost;
        if (playerCondition.playerStamina <= 0) playerCondition.playerStamina = 0;
        if (playerCondition.recharge != null) StopCoroutine(playerCondition.recharge);
        playerCondition.recharge = StartCoroutine(playerCondition.rechargeStamina());
        yield return new WaitForSeconds(jumpCooldown);
        readyToJump = true;
    }
}
