using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CreditUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI timerText;
    private TimerPlayer timer;
    public void ShowUI(){
        timer = GameObject.FindWithTag("Timer").GetComponent<TimerPlayer>();
        timerText.text = TimeSpan.FromSeconds(timer.GetTime()).ToString("mm:ss");
    }
}
