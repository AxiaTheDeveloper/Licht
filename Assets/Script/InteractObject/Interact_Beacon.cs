using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class Interact_Beacon : MonoBehaviour
{
    [SerializeField] private Light2D beaconLight;
    [Tooltip("MaxFireSize = 12"), SerializeField] private float powerNeededPower;
    private bool isLitUp;

    public event EventHandler OnBeaconLitUp; // lempar ke obstaclePuzzle
    [SerializeField]private Sprite beaconOnSprite;
    [SerializeField]private SpriteRenderer spriteRenderer;

    private void Awake() {
        isLitUp = false;
        spriteRenderer = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    public void LightUpBeacon(PlayerLight playerLight){
        if(playerLight.GetFireSize() >= powerNeededPower){
            playerLight.changeFireSize(-powerNeededPower);
            isLitUp = true;

            spriteRenderer.sprite = beaconOnSprite;

            beaconLight.intensity = 1;
            OnBeaconLitUp?.Invoke(this,EventArgs.Empty);
        }
        
    }

    public bool IsLitUp(){
        return isLitUp;
    }
}
