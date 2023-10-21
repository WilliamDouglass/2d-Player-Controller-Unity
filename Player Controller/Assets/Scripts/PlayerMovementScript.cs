using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    #region Vars 
    /* -------------------------------- OnEnable -------------------------------- */
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D objectCollider;

    /* ------------------------------ Ground Check ------------------------------ */
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float groundDistance;
    [SerializeField] private bool isGrounded;

    /* ------------------------------- horizontal ------------------------------- */
    [Range(1, 50)] //the max range need to be set to the max run speed
    [SerializeField] private float runAcceloration = 8f;
    [Range(1, 100)]
    [SerializeField] private float runDeceloration = 10f;
    [SerializeField] private float runMaxSpeed = 10f;
    [SerializeField] private float runMinSpeed = 1f;
    [SerializeField] private float horizontalInput;

    /* --------------------------------- Gravity -------------------------------- */
    [SerializeField] private float groundGravity = 5f;
    [SerializeField] private float fallingGravity = 10f;

    /* ------------------------------ Jump and Fall ----------------------------- */
    [SerializeField] private float terminalFall = 8f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpAcceloration = 10f;
    [SerializeField] private bool canJump = false;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool jumpInputPressed = false;
    [SerializeField] private bool jumpEnded = true;

    /* ---------------------------------- Coyote --------------------------------- */
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteCounter = 0f;

    /* ------------------------------- Jump Buffer ------------------------------ */
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpBufferCoutner = 0f;

    /* -------------------------------------------------------------------------- */
    [SerializeField] private Vector2 frameVelocity;

    #endregion



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
    }


    private void FixedUpdate()
    {
        InitFrameValues();
        HandleGravity();
        HandleCoyoteAndBuffer();
        HandleMove();
        HandleJump();
        HandleFalling();

        ApplyMotion();
    }


    #region Update Values 

    #region Input Methods 
    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Debug.Log("Pressed");
            jumpInputPressed = true;
        }

        else if (context.canceled)
        {
            // Debug.Log("Not Pressed");
            jumpInputPressed = false;
            // jumpEnded = true;
        }
    }
    #endregion
    #region Frame Init & Apply 
    private void InitFrameValues()
    {
        //update ground check
        // update framVelocity
        frameVelocity = rb.velocity;
        UpdateGrounded();
    }
    private void ApplyMotion()
    {
        rb.velocity = frameVelocity;
    }
    #endregion
    #region Ground Check ()
    private void UpdateGrounded()
    {
        isGrounded = Physics2D.BoxCast(transform.position, groundCheckSize, 0, -transform.up, groundDistance, groundLayer);
    }

    void OnDrawGizmos() // Displays the ground check box
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(transform.position - transform.up * groundDistance, groundCheckSize);
    }
    #endregion

    #endregion
    #region Gravity and Falling
    private void HandleGravity()
    {
        if (isGrounded)
        {
            rb.gravityScale = groundGravity;
        }
        else if (!isGrounded && frameVelocity.y < 0) //Falling
        {
            rb.gravityScale = fallingGravity;
        }
    }

    private void HandleFalling()
    {
        //Terminal Velocity
        frameVelocity.y = Mathf.Clamp(frameVelocity.y, -terminalFall, terminalFall);
    }

    #endregion
    #region Move Horizontal 
    private Vector2 velocity = Vector2.zero;
    private void HandleMove()
    {
        if (horizontalInput != 0)
        {
            frameVelocity.x = Mathf.SmoothDamp(frameVelocity.x, runMaxSpeed * horizontalInput, ref velocity.x, (runMaxSpeed - runAcceloration) * Time.deltaTime);
        }
        else
        {
            frameVelocity.x = Mathf.SmoothDamp(frameVelocity.x, 0, ref velocity.x, (100 - runDeceloration) * Time.deltaTime);
        }
        if (Mathf.Abs(frameVelocity.x) < runMinSpeed)
        {
            frameVelocity.x = 0;
        }

        ///Terminal Speed
        frameVelocity.x = Mathf.Clamp(frameVelocity.x, -runMaxSpeed, runMaxSpeed);

    }
    #endregion
    #region Jumping 
    private void HandleJump()
    {
        jumpEnded = (jumpInputPressed) ? false : true;
        if (canJump)
        {
            // Debug.Log(canJump);
            // Debug.Log("Attempt Jump");
            ExcuteJump();
        }
        if (!jumpInputPressed && frameVelocity.y > 0f)
        {   //Variable Jump hight 
            coyoteCounter = -1f;
            // jumpCounter--;
            frameVelocity = new Vector2(frameVelocity.x, frameVelocity.y * 0.5f);
        }
    }
    private void ExcuteJump()
    {
        // Debug.Log("Jump");
        canJump = false;
        jumpEnded = false;
        coyoteCounter = -1f;
        jumpBufferCoutner = -1f;
        rb.gravityScale *= jumpAcceloration;
        jumpForce = Mathf.Sqrt(jumpHeight * objectCollider.bounds.size.y * -2 * (Physics2D.gravity.y * rb.gravityScale));
        frameVelocity = new Vector2(frameVelocity.x, frameVelocity.y + jumpForce);
    }


    private void HandleCoyoteAndBuffer()
    {
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else if (coyoteCounter > 0f)
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpInputPressed && !isGrounded && jumpEnded)
        {
            jumpBufferCoutner = jumpBufferTime;
        }
        else if (jumpBufferCoutner > 0f)
        {
            jumpBufferCoutner -= Time.deltaTime;
        }

        if ((coyoteCounter > 0f && jumpInputPressed && jumpEnded) || (jumpBufferCoutner > 0f && isGrounded))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

    }

    #endregion

}
