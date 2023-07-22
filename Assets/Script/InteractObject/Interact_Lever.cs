using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Interact_Lever : MonoBehaviour
{
    [SerializeField]private bool leverOn;
    // [SerializeField]private Animator leverAnimator;
    [SerializeField]private Sprite leverOnSprite, leverOffSprite;
    [SerializeField]private SpriteRenderer spriteRenderer;
    public event EventHandler OnUsedLever;
    private void Awake() {
        leverOn = false;
        spriteRenderer = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
    }
    public void UseLever(){
        leverOn = !leverOn;
        if(leverOn){
            spriteRenderer.sprite = leverOnSprite;
        }
        else{
            spriteRenderer.sprite = leverOffSprite;
        }
        OnUsedLever?.Invoke(this,EventArgs.Empty);
        
    }

    public bool GetLeverOn(){
        return leverOn;
    }

    
}
