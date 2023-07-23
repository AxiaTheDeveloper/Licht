using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CreditUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI timerText;
    [SerializeField]private RectTransform BG;
    [SerializeField]private CanvasGroup creditText;
    [SerializeField]private UIFadeBG fade;
    private TimerPlayer timer;
    private void Awake() {
        HideUI();
    }
    public void HideUI(){
        gameObject.SetActive(false);
        creditText.alpha = 0f;
        LeanTween.alpha(BG, 0f, 0.2f);
    }
    public void ShowUI(){
        gameObject.SetActive(true);
        timer = GameObject.FindWithTag("Timer").GetComponent<TimerPlayer>();
        var ts = TimeSpan.FromSeconds(timer.GetTime());
        timerText.text = string.Format("{0:00}:{1:00}", ts.TotalMinutes, ts.Seconds);
        LeanTween.alpha(BG, 1f, 0.8f).setOnComplete(
            ()=>creditText.LeanAlpha(1f, 1f).setOnComplete(
                () => TheGameManager.Instance.ChangeToCredit()
            )
        );
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            fade.ShowUIExit();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            fade.ShowUIRestart(timer);
        }
    }
}