using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TestingMovement : MonoBehaviour
{
    #region Vars
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

    #endregion

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

    private void FixedUpdate()
    {
        // 
    }

    //Move



    // Input

    public void Move()


/*
    Handle
        Jump
        Fall
        Move
        Dash
    Update
        groundCheck

*/  