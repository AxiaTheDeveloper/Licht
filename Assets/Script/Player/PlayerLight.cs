using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Light2D lightFire;
    [SerializeField] private float lightFireStartSize;
    private float lightFireSize;
    [SerializeField] private float lightFireMinus;

    private Transform playerPosNow;
    private Vector3 playerPositionLast;
    
    private void Awake() {
        playerPosNow = GetComponent<Transform>();
        playerPositionLast = playerPosNow.position;
    }

    private void Start() {
        lightFireSize = lightFireStartSize;
        changeFireSize();
        //gamemanager
        
    }

    private void changeFireSize(){
        lightFire.pointLightOuterRadius = lightFireSize;
    }

    private void Update() {
        if(playerPosNow.position != playerPositionLast){
            Debug.Log("berkurang");
            playerPositionLast = playerPosNow.position;
            if(lightFireSize > 0){
                lightFireSize -= (lightFireMinus * Time.deltaTime);
                changeFireSize();
            }
            
        }
    }

    

    
}
