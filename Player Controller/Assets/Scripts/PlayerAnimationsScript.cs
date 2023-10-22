using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsScript : MonoBehaviour
{
    private Animator animator;
    [SerializeField] Transform TargetTransform;
    [SerializeField] private PlayerMovementScript moveScript;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool prevGround = false;
    [SerializeField] private Vector2 prevVelocity;
    [SerializeField] private float horizontalInput;
    [SerializeField] private bool jumpExcuted;
    [SerializeField] private bool faceingRight = true;



    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateVars();

        if (velocity.x == 0)
        {
            animator.SetFloat("Velocity", 1);
        }
        else
        {
            animator.SetFloat("Velocity", (Mathf.Abs(velocity.x) / 10) + 1);
        }

        if (jumpExcuted)
        {
            animator.SetTrigger("Jump");
            moveScript.SetJumpExcuted(false);
        }

        if (isGrounded && !prevGround && (prevVelocity.y <= 0f && velocity.y <= 0.1f))
        {
            animator.SetTrigger("Land");
        }
        if (horizontalInput > 0f && !faceingRight)
        {
            Flip();
        }
        if (horizontalInput < 0f && faceingRight)
        {
            Flip();
        }



        prevGround = isGrounded;
        prevVelocity = velocity;
    }

    private void Flip()
    {
        Vector3 currentScale = TargetTransform.localScale;
        currentScale.x *= -1;
        TargetTransform.localScale = currentScale;
        faceingRight = !faceingRight;
    }

    private void UpdateVars()
    {
        velocity = moveScript.GetVelocity();
        isGrounded = moveScript.GetisGrounded();
        jumpExcuted = moveScript.GetJumpExcuted();
        horizontalInput = moveScript.GetHorizontalInput();
    }

}
