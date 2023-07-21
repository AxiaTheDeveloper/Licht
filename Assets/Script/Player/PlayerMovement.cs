using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("== References Object ==")]
    private Rigidbody2D playerRb;


    [Header("== Variables ==")]

    [Tooltip("Movement Direction"), SerializeField] 
    private Vector2 dir;


    [Header("Horizontal Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float accelerate;
    [SerializeField] private float decelerate;

    [Header("Vertical Variables")]
    [SerializeField] private float verticalForce;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float lastPressedJumpTime;
    [SerializeField] private float lastOnGroundTime;
    [SerializeField] private bool isJumping;
    [SerializeField] private float defaultGravScale;
    [SerializeField] private float gravScaleMultOnJumpCut;
    [SerializeField] private float jumpInputBuffer;
    private bool isJumpCut;
    [SerializeField, Tooltip("Time for how long player still can jump on the ledge")] 
    private float coyoteTime;

    void Awake()
    {
        playerRb = gameObject.GetComponent<Rigidbody2D>();
        SetGravityScale(defaultGravScale);
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        isJumping = false;
    }

    private void Update()
    {
        float x = GetHorizontalInput();
        dir = new Vector2(x, 0);
        PlayerJump();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    #region Methods

    private float GetHorizontalInput()
    {
        if (Input.GetKey(KeyCode.D)) return 1f;
        if (Input.GetKey(KeyCode.A)) return -1f;

        return 0f;
    }
    
    private bool GetVerticalInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            lastPressedJumpTime = jumpInputBuffer;
            return lastOnGroundTime > 0 && !isJumping;
        }

        return false;
    }

    void PlayerJump()
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        

        if (isJumpCut)
        {
            SetGravityScale(defaultGravScale * gravScaleMultOnJumpCut);
            playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Max(playerRb.velocity.y, -maxFallSpeed));
        }
        else SetGravityScale(defaultGravScale);

        if (!isJumping) 
        {
            if (Physics2D.OverlapBox(transform.position, new Vector2(0.49f, transform.localScale.y + 0.1f), 0, groundLayer) && !isJumping) //checks if set box overlaps with ground
            {
                SetGravityScale(defaultGravScale);
                lastOnGroundTime = coyoteTime;
            }
        }

        if (isJumping && playerRb.velocity.y < 0) isJumping = false;
        
        CollissionCheck();
        //OnJumpInput
        if (GetVerticalInput() && lastPressedJumpTime > 0)
        {
            lastPressedJumpTime = 0;
            lastOnGroundTime = 0;

            Debug.Log("Running");
            float tempForce = verticalForce;
            if (playerRb.velocity.y < 0)
            {
                tempForce -= playerRb.velocity.y;
                playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Max(playerRb.velocity.y, -maxFallSpeed));
            }

            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
            playerRb.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);


            isJumping = true;
            isJumpCut = false;
        }
        //OnJumpOutput
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
        {
            if (CanJumpCut())
            {
                isJumpCut = true;
            }
        }
    }

    private bool CanJumpCut()
    {
        return isJumping && playerRb.velocity.y > 0;
    }

    private void SetGravityScale(float scale)
    {
        playerRb.gravityScale = scale;
    }

    private void MovePlayer()
    {
        float targetSpeed = dir.x * moveSpeed;
        float speedDif = targetSpeed - playerRb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelerate : decelerate;
        float movement = speedDif * accelRate;

        playerRb.AddForce(movement * Vector2.right);
    }

    private void CollissionCheck()
    {
        if (isJumping)
        {

        }
    }
    #endregion
}
