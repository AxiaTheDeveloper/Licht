using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("== References Object ==")]
    private Rigidbody2D playerRb;
    private CapsuleCollider2D capsuleCollider;
    private MoveCamera moveCamera;


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
    [SerializeField] private float lastPressedJumpTime = 0;
    [SerializeField] private float lastOnGroundTime = 0;
    [SerializeField] private bool isJumping;
    [SerializeField] private float defaultGravScale;
    [SerializeField] private float gravScaleMultOnJumpCut;
    [SerializeField] private float jumpInputBuffer;
    private bool isJumpCut, isOnGround, wasOnJump;
    [SerializeField, Tooltip("Time for how long player still can jump on the ledge")] 
    private float coyoteTime;


    [Header("Slope Variables")]
    [SerializeField] private float slopeCheckDistanceNormal;
    [SerializeField] private float slopeCheckDistanceJump;
    private float slopeCheckDistance;
    [SerializeField] private LayerMask layerGround;
    private Vector2 colliderSize;
    private float slopeDownAngle, slopeDownAngleLast, slopeSideAngle;
    private Vector2 slopeNormalPerpendicular;
    [SerializeField] private bool isOnSlope;



    private TheGameManager gameManager;
    private bool isStart;
    
    

    void Awake()
    {
        
        isStart = true;

        playerRb = gameObject.GetComponent<Rigidbody2D>();
        isOnSlope = false;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        colliderSize = capsuleCollider.size;

        moveCamera = GameObject.Find("CM vcam1").GetComponent<MoveCamera>();

        wasOnJump = true;
        isJumping = false;
        isOnGround = false;
        SetGravityScale(defaultGravScale);
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
    }

    private void Start() {
        gameManager = TheGameManager.Instance;
    }

    private void Update()
    {
        float x;

        if(gameManager.IsIngame()){
            x = GetHorizontalInput();
        }
        else{
            x = 0;
        }

        dir = new Vector2(x, 0);
        PlayerJump();
        CheckEdgeCamera();
    }

    public float GetDirX(){
        return dir.x;
    }

    void FixedUpdate()
    {
        SlopeCheck();
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
        else if (playerRb.velocity.y <= 0)
        {
            SetGravityScale(defaultGravScale * gravScaleMultOnJumpCut);
        }
        else SetGravityScale(defaultGravScale);

        if (!isJumping) 
        {
            if (Physics2D.OverlapBox(transform.position, new Vector2(0.1f, transform.localScale.y + 0.1f), 0, layerGround) && !isJumping) //checks if set box overlaps with ground
            {
                if(isStart){
                    isStart = false;
                    StartCoroutine(StartCinematicToInGame_Counter());
                }
                SetGravityScale(defaultGravScale);
                lastOnGroundTime = coyoteTime;
                isOnGround = true;
            }
            else{
                isOnGround = false;
            }
        }
        

        if (isJumping && playerRb.velocity.y < 0) isJumping = false;
        
        //OnJumpInput
        if (GetVerticalInput() && lastPressedJumpTime > 0 && gameManager.IsIngame())
        {
            lastPressedJumpTime = 0;
            lastOnGroundTime = 0;

            // Debug.Log("Running");
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
            isOnGround = false;
            wasOnJump = true;
            
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

    private IEnumerator StartCinematicToInGame_Counter(){
        yield return new WaitForSeconds(0.2f);
        gameManager.ChangeToInGame();
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
        float targetSpeed;
        float speedDif;
        float accelRate;
        float movement;
        

        if(!isOnSlope && isOnGround)
        {
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            targetSpeed = dir.x * moveSpeed;
            speedDif = targetSpeed - playerRb.velocity.x;
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelerate : decelerate;
            movement = speedDif * accelRate;

            playerRb.AddForce(movement * Vector2.right);
            
        }
        else if(isOnSlope && isOnGround)
        {
            if(dir.x == 0){
                playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            else{
                playerRb.constraints = RigidbodyConstraints2D.None;
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            Vector2 newVelocitySlope = new Vector2(-dir.x * moveSpeed * slopeNormalPerpendicular.x, moveSpeed * slopeNormalPerpendicular.y * -dir.x);
  
            playerRb.velocity = newVelocitySlope;
            
        }
        else if(!isOnGround)
        {
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            targetSpeed = dir.x * moveSpeed;
            speedDif = targetSpeed - playerRb.velocity.x;
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelerate : decelerate;
            movement = speedDif * accelRate;

            playerRb.AddForce(movement * Vector2.right);
        }

        // Debug.Log(playerRb.velocity);
    }

    private void SlopeCheck()
    {
        Vector2 checkPos_Middle = transform.position - new Vector3(0f, colliderSize.y / 2);
        if(wasOnJump)
        {
            slopeCheckDistance = slopeCheckDistanceJump;
        }
        else
        {
            slopeCheckDistance = slopeCheckDistanceNormal;
        }
        //kalo isjumping true, si slope dijadiin false dan ini gabisa dijalanin, kalo isjumping false, bawah baru bisa dijalanin
        if(!isOnGround && isOnSlope)
        {
            isOnSlope = false;
        }
        else if(isOnGround){
            SlopeCheckHorizontal(checkPos_Middle);
            SlopeCheckVertical(checkPos_Middle);
        }
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D hitObjectFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, layerGround);
        RaycastHit2D hitObjectBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, layerGround);

        if(hitObjectFront)
        {
            isOnSlope = true;
            wasOnJump = false;
            slopeSideAngle = Vector2.Angle(hitObjectFront.normal, Vector2.up);
            // Debug.DrawRay(hitObjectFront.point, hitObjectFront.normal, Color.red);
        }
        else if(hitObjectBack)
        {
            isOnSlope = true;
            wasOnJump = false;
            slopeSideAngle = Vector2.Angle(hitObjectBack.normal, Vector2.up);
            // Debug.DrawRay(hitObjectBack.point, hitObjectBack.normal, Color.blue);
        }
        else
        {
            slopeSideAngle = 0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hitObject = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, layerGround);

        if(hitObject)
        {
            slopeNormalPerpendicular = Vector2.Perpendicular(hitObject.normal).normalized;
            slopeDownAngle = Vector2.Angle(hitObject.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleLast)
            {
                isOnSlope = true;
                wasOnJump = false;
            }
            slopeDownAngleLast = slopeDownAngle;

            Debug.DrawRay(hitObject.point, hitObject.normal, Color.green);
            Debug.DrawRay(hitObject.point, slopeNormalPerpendicular, Color.green);
        }
    }
    private void CheckEdgeCamera()
    {
        
        Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x > 1.02f)
            moveCamera.Right();
        if (pos.x < -0.02f)
            moveCamera.Left();
        if (pos.y > 1.02f)
            moveCamera.Up();
        if (pos.y < -0.02f)
            moveCamera.Down();

    }
    #endregion
}
