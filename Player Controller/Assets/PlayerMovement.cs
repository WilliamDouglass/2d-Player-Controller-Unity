using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    /* -------------------------------- OnEnable -------------------------------- */
    private Rigidbody2D rb;
    private CapsuleCollider2D capCollider;
    private TrailRenderer trail;
    private SpriteRenderer spriteRender;

    /* ------------------------------ Ground Check ------------------------------ */
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float groundDistance;
    private bool isGrounded;

    /* ------------------------------- horizontal ------------------------------- */
    [SerializeField] private float moveSpeed = 8f;
    private float horizontal;
    private bool facingRight = true;


    /* ------------------------------ Jump and Fall ----------------------------- */
    [SerializeField] private float terminalFall = 8f;
    [SerializeField] private float jumpHeight = 5f;
    private float jumpForce;

    /* ---------------------------------- Coyote --------------------------------- */
    [SerializeField] private float coyoteTime = 0.2f;
    public float coyoteCounter = 0;

    /* ------------------------------- Jump Buffer ------------------------------ */
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCoutner = 0f;

    /* -------------------------------- MultiJump ------------------------------- */

    [SerializeField] private int maxJumps = 1;
    public int jumpCounter = 0;


    /* ---------------------------------- Dash ---------------------------------- */
    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;
    private bool canDash = true;
    private bool isDashing;


    /* --------------------------------- Origize -------------------------------- */
    private Vector2 frameVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        trail = GetComponent<TrailRenderer>();
        spriteRender = GetComponent<SpriteRenderer>();
    }










    // TODO Orginize vars in to a scriptable object 
    // TODO Set up tool hints and orginize for unity editor 
    // * intigrate Jumpbuffer [Ask Other for opinions]
    // TODO ? Grapple ?
    // TODO Pick apart update function in to diffrent function && move some into fixed update 
    // TODO Think about how you want the dash to work or leave as is (ask a friend for input)


    private void Update()
    {
        // Debug.Log(rb.velocity);
        // frameVelocity.x = (horizontal * moveSpeed);
    }


    private void FixedUpdate()
    {
        if (isDashing) return;
        frameVelocity = new Vector2(horizontal * moveSpeed, frameVelocity.y);
        UpdateGround();
        HandleFlip();
        ManageJumpAndFall();
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        rb.velocity = frameVelocity;
    }

    private void HandleFlip()
    {
        if (horizontal > 0 && !facingRight)
        {
            Flip();
        }
        if (horizontal < 0 && facingRight)
        {
            Flip();
        }
    }
    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }

    #region Movement 
    private void UpdateGround()
    {
        isGrounded = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, groundDistance, groundLayer);
    }

    #region Jump 
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !isGrounded)
        {
            jumpBufferCoutner = jumpBufferTime;
        }

        if (context.performed && (jumpCounter > 0 || coyoteCounter > 0f))
        {
            ExcuteJump();
        }

        if (context.canceled && frameVelocity.y > 0f)
        {
            coyoteCounter = 0f;
            jumpCounter--;
            frameVelocity = new Vector2(frameVelocity.x, frameVelocity.y * 0.5f);
        }

    }

    private void ExcuteJump()
    {
        Debug.Log("Jump");
        jumpForce = Mathf.Sqrt(jumpHeight * capCollider.size.y * -2 * (Physics2D.gravity.y * rb.gravityScale));
        // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        frameVelocity = new Vector2(frameVelocity.x, frameVelocity.y + jumpForce);

    }

    private void ManageJumpAndFall()
    {
        //Manage Jump
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpCounter = maxJumps;
            if (jumpBufferCoutner > 0 && jumpCounter > 0)
            {
                Debug.Log("JUMP BUFFER");
                jumpBufferCoutner = -1;
                ExcuteJump();
            }
        }
        else if (isGrounded && coyoteCounter > 0f)
        {
            coyoteCounter -= Time.deltaTime;
        }
        if (jumpBufferCoutner > 0f)
        {
            jumpBufferCoutner -= Time.deltaTime;
        }
        else
        {
            jumpBufferCoutner = -1;
        }

        //Manage Fall
        if (frameVelocity.y < 0f) // if falling
        {
            frameVelocity = new Vector2(frameVelocity.x, Mathf.Max(frameVelocity.y, -terminalFall)); // limit falling speed
        }
    }



    #endregion




    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    #region Dash 
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        spriteRender.color = Color.magenta;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.AddForce(Vector2.right * horizontal * dashPower, ForceMode2D.Impulse);
        trail.emitting = true;

        yield return new WaitForSeconds(dashTime);
        trail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        rb.gravityScale = originalGravity;
        canDash = true;
        spriteRender.color = Color.white;


    }
    #endregion
    #endregion



    /* ---------------------------------- Debug --------------------------------- */
    void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(transform.position - transform.up * groundDistance, boxSize);
    }
    /* ---------------------------------- Debug --------------------------------- */





}
