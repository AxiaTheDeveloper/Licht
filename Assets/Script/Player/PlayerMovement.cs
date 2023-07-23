using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance {get; private set;}

    [Header("== References Object ==")]
    private Rigidbody2D playerRb;
    private CapsuleCollider2D capsuleCollider;
    private MoveCamera moveCamera;
    private DustParticleSpawner dustPSpawner;
    [Tooltip("Optional, if there are any, it will spawn here")]
    [SerializeField] private GameObject playerSpawner;


    [Header("== Variables ==")]

    [Tooltip("Movement Direction"), SerializeField] 
    private Vector2 dir;
    private bool dustIsRunning;


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
    [SerializeField] private bool isJumpCut, isOnGround, wasOnJump;
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

    [Header("Player Rotation")]
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Transform LightVisual;
    [SerializeField]private Vector3 left_lightVisualPos, right_lightVisualPos;

    [Header("Player Animator")]
    [SerializeField]private Animator playerAnimator;


    private TheGameManager gameManager;
    private bool isStart;

    private Vector2 SaveVelocityPause;
    private SFXManager sfxManager;
    
    

    void Awake()
    {
        Instance = this;

        isStart = true;
        playerAnimator = gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        playerRb = gameObject.GetComponent<Rigidbody2D>();
        isOnSlope = false;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        colliderSize = capsuleCollider.size;

        moveCamera = GameObject.Find("CM vcam1").GetComponent<MoveCamera>();

        wasOnJump = true;
        isJumping = false;
        isOnGround = false;
        playerAnimator.SetBool("IsOnGround", false);

        SetGravityScale(defaultGravScale);
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;


    }

    private void Start() {
        gameManager = TheGameManager.Instance;
        int lastChild = transform.childCount - 1;
        dustPSpawner = transform.GetChild(lastChild).GetComponent<DustParticleSpawner>();
        sfxManager = SFXManager.Instance;
        playerSpawner = GameObject.Find("Player Spawn");
    }

    private void OnEnable()
    {
        if (playerSpawner != null)
        {
            // Debug.Log("Run");
            transform.position = playerSpawner.transform.position;
        }
    }

    private void Update()
    {
        float x;

        if(gameManager.IsIngame()){
            x = GetHorizontalInput();
            CheckEdgeCamera();
            PlayDustEffect();
        }
        else{
            x = 0;
        }

        dir = new Vector2(x, 0);
        if(isOnGround){
            if(dir.x != 0){
                if(!sfxManager.isPlayedSFX_PlayerWalk()){
                    sfxManager.PlaySFX_PlayerWalk();
                }
                
            }
            else{
                sfxManager.StopSFX_PlayerWalk();
            }
        }
        else{
            sfxManager.StopSFX_PlayerWalk();
        }
        PlayerJump();
        PlayerRotation();
        CheckWindyEnvironment();
    }

    void FixedUpdate()
    {
        SlopeCheck();
        MovePlayer();
    }

    #region Methods

    public float GetDirX(){
        return dir.x;
    }

    public void PlayerRotation(){
        if(dir.x == 1){
            float playerScaleX = Math.Abs(playerVisual.localScale.x);
            playerVisual.localScale = new Vector2(playerScaleX, playerVisual.localScale.y);
            LightVisual.transform.localPosition = right_lightVisualPos;
        }
        else if(dir.x == -1){
            float playerScaleX = Math.Abs(playerVisual.localScale.x) * -1;
            playerVisual.localScale = new Vector2(playerScaleX, playerVisual.localScale.y);
            LightVisual.transform.localPosition = left_lightVisualPos;
        }
    }

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
        
        if(!isOnGround){
            playerAnimator.SetFloat("playerRb",playerRb.velocity.y);
        }

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
            if (Physics2D.OverlapBox(transform.position - new Vector3(0f, 0.5f), new Vector2(0.1f, transform.localScale.y + 0.8f), 0, layerGround) && !isJumping) //checks if set box overlaps with ground
            {
                if(isStart){
                    isStart = false;
                    StartCoroutine(StartCinematicToInGame_Counter());
                }
                SetGravityScale(defaultGravScale);
                lastOnGroundTime = coyoteTime;
                isOnGround = true;
                playerAnimator.SetBool("IsOnGround", true);
            }
            else{
                isOnGround = false;
                playerAnimator.SetBool("IsOnGround", false);
            }
        }
        //lastongroundtime > 0 - landing

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
            sfxManager.PlaySFX_PlayerJump();
            playerRb.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);


            isJumping = true;
            isJumpCut = false;
            isOnGround = false;
            playerAnimator.SetBool("IsOnGround", false);
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
        
        if(dir.x == 0){
            playerAnimator.SetBool("IsWalk", false);
        }
        else{
            playerAnimator.SetBool("IsWalk", true);
        }

        if(!isOnSlope && isOnGround)
        {
            if(gameManager.IsIngame()){
                playerRb.constraints = RigidbodyConstraints2D.None;
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            targetSpeed = dir.x * moveSpeed;
            speedDif = targetSpeed - playerRb.velocity.x;
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelerate : decelerate;
            movement = speedDif * accelRate;

            playerRb.AddForce(movement * Vector2.right);
            
        }
        else if(isOnSlope && isOnGround)
        {
            
            if(gameManager.IsIngame()){
                if(dir.x == 0){
                    playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
                else{
                    playerRb.constraints = RigidbodyConstraints2D.None;
                    playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
            Vector2 newVelocitySlope = new Vector2(-dir.x * moveSpeed * slopeNormalPerpendicular.x, moveSpeed * slopeNormalPerpendicular.y * -dir.x);
  
            playerRb.velocity = newVelocitySlope;

            
        }
        else if(!isOnGround)
        {
            if(gameManager.IsIngame()){
                playerRb.constraints = RigidbodyConstraints2D.None;
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
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
            // SlopeCheckHorizontal(checkPos_Middle);
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
            // Debug.Log(hitObjectFront.transform + "object front");
        }
        else if(hitObjectBack)
        {
            isOnSlope = true;
            wasOnJump = false;
            slopeSideAngle = Vector2.Angle(hitObjectBack.normal, Vector2.up);
            // Debug.DrawRay(hitObjectBack.point, hitObjectBack.normal, Color.blue);
            // Debug.Log(hitObjectBack.transform + "object beck");
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
                if(slopeDownAngle == 0){
                    isOnSlope = false;
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 0f);
                }
                else{
                    isOnSlope = true;
                
                }
                wasOnJump = false;
                // Debug.Log (slopeDownAngle + " angelnya" + slopeDownAngleLast);
            }
            
            slopeDownAngleLast = slopeDownAngle;

            

            Debug.DrawRay(hitObject.point, hitObject.normal, Color.green);
            Debug.DrawRay(hitObject.point, slopeNormalPerpendicular, Color.blue);
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

    public void PausePlayer(bool isPause){
        if(isPause){
            SaveVelocityPause = playerRb.velocity;
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else{
            if(isOnSlope){
                if(dir.x == 0){
                    playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
                else{
                    playerRb.constraints = RigidbodyConstraints2D.None;
                    playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
            else{
                playerRb.constraints = RigidbodyConstraints2D.None;
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            playerRb.velocity = SaveVelocityPause;
        }
    }
    private void PlayDustEffect()
    {
        if (playerRb.velocity.x > 0 && GetHorizontalInput() != 0 && !dustIsRunning && isOnGround)
        {
// /           Debug.Log("Running");
            dustIsRunning = true;
            dustPSpawner.SummonDustTrail(playerRb.transform.position, GetHorizontalInput());
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            dustIsRunning = false;
        }
    }

    private void CheckWindyEnvironment()
    {
        LayerMask windyArea = LayerMask.GetMask("WindyArea");
        if (Physics2D.OverlapBox(transform.position, new Vector2(1f, 1f), 0f, windyArea)) gameManager.SetEnvironment("windy");
        else gameManager.SetEnvironment("normal");
            
    }

    #endregion
}
