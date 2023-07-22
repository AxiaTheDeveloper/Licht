using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    public static PlayerLight Instance{get; private set;}
    public enum Environment{
        normal, windy
    }
    [Header("Environment Mode")]
    [SerializeField]private Environment enviNow;


    [Header("Light")]
    [SerializeField] private Light2D lightFire;
    [SerializeField] private float lightFireStartSize;
    private float lightFireSize;
    private float dyingTime_Idle, dyingTime_Move;

    [Header("DyingTime - Normal")]
    [SerializeField] private float dyingTime_IdleNormal;
    [SerializeField] private float dyingTime_MoveNormal;

    [Header("DyingTime - Windy")]
    [SerializeField] private float dyingTime_IdleWindy;
    [SerializeField] private float dyingTime_MoveWindy;

    [Header("Light Collider")]
    [SerializeField] private CircleCollider2D colliderLight;

    //bikin kek public enum gitu buat cek lg apa ada di daerah apa etc biar dying time ya berubah gitu

    private Transform playerPosNow;
    private Vector3 playerPositionLast;

    private TheGameManager gameManager;
    private PlayerMovement playerMovement;

    [SerializeField]private float LightSizeDifferenceWithCollider;

    private void Awake() {
        Instance = this;
        playerMovement = GetComponent<PlayerMovement>();
        playerPosNow = GetComponent<Transform>();
        playerPositionLast = playerPosNow.position;
    }

    private void Start() {
        gameManager = TheGameManager.Instance;

        restartFireSize(lightFireStartSize);
        if(gameManager.GetEnvironment() == TheGameManager.gameEnvironment.normal){
            ChangeEnvironmentMode(Environment.normal);
        }
        else if(gameManager.GetEnvironment() == TheGameManager.gameEnvironment.windy){
            ChangeEnvironmentMode(Environment.windy);
        }

        
    }

    public float GetFireSize(){
        return lightFireSize;
    }
    
    public void changeFireSize(float change){
        lightFireSize += change;

        changeFireSizeRadius();
        changeLightColliderSize();
    }

    public void restartFireSize(float restartSize){
        lightFireSize = restartSize;

        changeFireSizeRadius();
        changeLightColliderSize();
    }

    private void changeFireSizeRadius(){
        lightFire.pointLightOuterRadius = lightFireSize;
    }

    public void changeLightColliderSize(){
        float collideSize = 0;
        if(lightFireSize - LightSizeDifferenceWithCollider > 0){
            collideSize = lightFireSize - LightSizeDifferenceWithCollider;
        }
        else{
            collideSize = 0;
        }
        colliderLight.radius = collideSize;
    }

    public void ChangeEnvironmentMode(Environment enviChange){
        enviNow = enviChange;
        SetDyingTIme();
    }

    public void SetDyingTIme(){
        if(enviNow == Environment.normal)
        {
            dyingTime_Idle = dyingTime_IdleNormal/2;
            dyingTime_Move = dyingTime_MoveNormal/2;
        }
        else if(enviNow == Environment.windy)
        {
            dyingTime_Idle = dyingTime_IdleWindy;
            dyingTime_Move = dyingTime_MoveWindy;
        }
    }

    private void Update() {
        if(gameManager.IsIngame()){
            //tny ini jdnya mo pastiinnya gmn mo intinya berubah posisi trmasuk gerak ato tidak etc etc
            float dyingTimeChange;
            if(playerPosNow.position == playerPositionLast)
            {
                // Debug.Log("berkurang diam");
                if(lightFireSize > 0)
                {
                    dyingTimeChange = -dyingTime_Idle * Time.fixedDeltaTime;
                    
                    changeFireSize(dyingTimeChange);
                }
                
            }
            else if(playerPosNow.position != playerPositionLast)
            {
                // Debug.Log("berkurang gerak");
                
                if(lightFireSize > 0)
                {
                    if(playerPositionLast.y == playerPosNow.position.y && playerPositionLast.x != playerPosNow.position.x)
                    {
                        if(playerMovement.GetDirX() == 0)
                        {
                            dyingTimeChange = -dyingTime_Idle * Time.fixedDeltaTime;
                        }
                        else{
                            dyingTimeChange = -dyingTime_Move * Time.fixedDeltaTime;
                            
                        }
                        changeFireSize(dyingTimeChange);
                        
                    }
                    else if(playerPositionLast.y != playerPosNow.position.y && playerPositionLast.x == playerPosNow.position.x)
                    {
                        dyingTimeChange = -dyingTime_Move * Time.fixedDeltaTime;
                        changeFireSize(dyingTimeChange);
                    }
                    else if(playerPositionLast.y != playerPosNow.position.y && playerPositionLast.x != playerPosNow.position.x)
                    {
                        dyingTimeChange = -dyingTime_Move * Time.fixedDeltaTime;
                        changeFireSize(dyingTimeChange);
                    }
                }
                playerPositionLast = playerPosNow.position;   
            }
            

            if(lightFireSize < 0)
            {
                restartFireSize(0f);
                gameManager.DeadState();
            }
        }
        
    }

    public void LightsOutExit(){
        while(lightFireSize >= 0){
            changeFireSize(-0.1f*Time.fixedDeltaTime);
        }
    }

}
