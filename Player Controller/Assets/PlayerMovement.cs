using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{

    /* --------------------------------- Object --------------------------------- */
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float groundDistance;
    [SerializeField] private CapsuleCollider2D capCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private SpriteRenderer spriteRender;

    /* ------------------------------- horizontal ------------------------------- */
    [SerializeField] private float moveSpeed = 8f;
    private float horizontal;
    private bool facingRight = true;


    /* ------------------------------ Jump and Fall ----------------------------- */
    [SerializeField] private float terminalFall = 8f;
    [SerializeField] private float jumpHeight = 5f;
    private bool jumpPressed;
    private float jumpForce;

    /* ---------------------------------- Coyote --------------------------------- */
    [SerializeField] private float coyoteTime = 0.2f;
    public float coyoteCounter = 0;

    /* ------------------------------- Jump Buffer ------------------------------ */

    [SerializeField] private float jumpBufferTime = 0.2f;

    public float jumpBufferCoutner = 0f;

    /* -------------------------------- MultiJump ------------------------------- */

    public int jumpCounter = 0;
    [SerializeField] private int maxJumps = 1;


    /* ---------------------------------- Dash ---------------------------------- */
    private bool isDashing;
    private bool canDash = true;
    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;


    /* --------------------------------- Origize -------------------------------- */
    private Vector2 frameVelocity;



    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;



        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);


        if (horizontal > 0 && !facingRight)
        {
            Flip();
        }
        if (horizontal < 0 && facingRight)
        {
            Flip();
        }


        /////////
        if (IsGounded())
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
        else if (IsGounded() && coyoteCounter > 0f)
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

        // rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (rb.velocity.y < 0f) // if falling
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -terminalFall)); // limit falling speed
        }


        // TODO Orginize vars in to a scriptable object 
        // TODO Set up tool hints and orginize for unity editor 
        // * intigrate Jumpbuffer [Ask Other for opinions]
        // TODO ? Grapple ?
        // TODO Pick apart update function in to diffrent function && move some into fixed update 
        // TODO Think about how you want the dash to work or leave as is (ask a friend for input)




    }

    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }



    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !IsGounded())
        {
            jumpBufferCoutner = jumpBufferTime;
        }

        if (context.performed && (jumpCounter > 0 || coyoteCounter > 0f))
        {
            ExcuteJump();
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            jumpPressed = false;
            coyoteCounter = 0f;
            jumpCounter--;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

    }

    private void ExcuteJump()
    {
        jumpPressed = true;
        jumpForce = Mathf.Sqrt(jumpHeight * capCollider.size.y * -2 * (Physics2D.gravity.y * rb.gravityScale));
        // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpForce);

    }




    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    void OnDrawGizmos()
    {
        if (IsGounded())
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(transform.position - transform.up * groundDistance, boxSize);
    }


    private bool IsGounded()
    {
        return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, groundDistance, groundLayer);

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

}
