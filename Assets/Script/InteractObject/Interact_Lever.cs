using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Interact_Lever : MonoBehaviour
{
    [SerializeField]private bool leverOn;
    [SerializeField]private Animator leverAnimator;
    public event EventHandler OnUsedLever;
    private void Awake() {
        leverOn = false;
    }
    public void UseLever(){
        leverOn = !leverOn;
        leverAnimator.SetBool("isLeverOn", leverOn);
        //nyalain animasi lever trus ntr br jalanin functionnya
        
    }
    public void AnimationFinish(){
        OnUsedLever?.Invoke(this,EventArgs.Empty);
    }
    public bool GetLeverOn(){
        return leverOn;
    }

    
}
