using System;
using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    #region Vars 
    /* -------------------------------- OnEnable -------------------------------- */
    [Tooltip("Gets assigned On Awake")]
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("Gets assigned On Awake")]
    [SerializeField] private Collider2D objectCollider;

    /* ------------------------------ Ground Check ------------------------------ */
    [Tooltip("Layer the Player Can jump on")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Size of the box cast for ground check")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1.51f, 0.03f);
    [Tooltip("Y offset of ground check box")]
    [SerializeField] private float groundDistance = 1.01f;
    [SerializeField] private bool isGrounded;


    /* ------------------------------- horizontal ------------------------------- */
    [Range(1, 100)]
    [SerializeField] private float runAcceloration = 45f;
    [Range(1, 100)]
    [SerializeField] private float runDeceloration = 95f;
    [SerializeField] private float runMaxSpeed = 20f;
    [Tooltip("If the Player velocity is lower than this then velocity is snaped to 0")]
    [SerializeField] private float runMinSpeed = 1f; //Probably keep this at 1
    [SerializeField] private float horizontalInput;

    /* --------------------------------- Gravity -------------------------------- */
    [SerializeField] private float groundGravity = 5f;
    [SerializeField] private float fallingGravity = 10f;

    /* ------------------------------ Jump and Fall ----------------------------- */
    [Tooltip("Max fall speed")]
    [SerializeField] private float terminalFall = 100f;
    [Tooltip("How high the player can jump in terms of player units")]
    [SerializeField] private float jumpHeight = 5f;
    [Range(1, 100)]
    [Tooltip("How fast the player reaches apex of the jump")]
    [SerializeField] private float jumpAcceloration = 5f;
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
    /* --------------------------------- Physics -------------------------------- */
    [Tooltip("The velocity that will be applied to rb per frame")]
    [SerializeField] private Vector2 frameVelocity;
    /* -------------------------------- Platforms ------------------------------- */
    [Tooltip("GameObject of the oen way platform player is touching")]
    [SerializeField] private GameObject currentOneWayPlatform;
    [SerializeField] private bool downInput;
    [SerializeField] private bool byPassNormalGravityUpdate = false;

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
        HandlePlatforms();
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
            jumpInputPressed = true;
        }

        else if (context.canceled)
        {
            jumpInputPressed = false;
        }
    }

    public void JumpDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            downInput = true;
        }

        else if (context.canceled)
        {
            downInput = false;
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
        if (byPassNormalGravityUpdate) return;
        if (isGrounded && jumpEnded)
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
    #region Platforms 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }

    }

    private void HandlePlatforms()
    {
        if (currentOneWayPlatform != null && downInput && frameVelocity.y <= 0f)
        {
            StartCoroutine(DisableCollision());
        }
    }


    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(objectCollider, platformCollider);
        byPassNormalGravityUpdate = true;
        rb.gravityScale = fallingGravity;
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(objectCollider, platformCollider, false);
        byPassNormalGravityUpdate = false;
    }


    #endregion



}
