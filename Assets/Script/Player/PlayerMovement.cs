using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float LastPressedJumpTime = 0;
    [SerializeField] private float LastOnGroundTime = 0;
    [SerializeField] private bool isJumping;

    void Awake()
    {
        playerRb = gameObject.GetComponent<Rigidbody2D>();
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) return true;

        return false;
    }

    void PlayerJump()
    {
        //LastOnGroundTime -= Time.deltaTime;
        //LastPressedJumpTime -= Time.deltaTime;
        CollissionCheck();
        if (GetVerticalInput())
        {
            LastPressedJumpTime = 0;
            LastOnGroundTime = 0;
            float tempForce = verticalForce;
            if (playerRb.velocity.y < 0)
                tempForce -= playerRb.velocity.y;

            playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
            playerRb.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
        }

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
