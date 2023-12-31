using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFadeBG : MonoBehaviour
{
    [SerializeField]private RectTransform fadeBG;
    [SerializeField]private CreditUI creditUI;
    [SerializeField]private float hideUISpeed = 2f;
    private void Awake() {
        fadeBG = GetComponent<RectTransform>();
        HideUI();
    }

    private void HideUI(){
        LeanTween.alpha(fadeBG, 0f, hideUISpeed);
    }
    public void ShowUIDead(){
        LeanTween.alpha(fadeBG, 1f, 2f).setOnComplete(
            ()=> SceneManager.LoadScene("Level1") // ini ganti ke nama first scene entar
        );
    }

    public void ShowUINextScene(){
        LeanTween.alpha(fadeBG, 1f, 2f).setOnComplete(
            ()=> ChooseNextScene()
        );
    }
    public void ShowUIRestart(TimerPlayer timer){
        LeanTween.alpha(fadeBG, 1f, 2f).setOnComplete(
            ()=> Restart(timer) // ini ganti ke nama first scene entar
        );
    }
    private void Restart(TimerPlayer timer){
        timer.RestartTimer();
        SceneManager.LoadScene("Level1");
    }

    private void ChooseNextScene(){
        TheGameManager gameManager = TheGameManager.Instance;
        if(SceneManager.GetActiveScene().name == "Level1"){
            SceneManager.LoadScene("Level2");
        }
    }
    public void ShowUIExit(){
        LeanTween.alpha(fadeBG, 1f, 0.3f).setOnComplete(
            ()=> Application.Quit()
        );
    }

}