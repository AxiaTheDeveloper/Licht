using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFadeBG : MonoBehaviour
{
    [SerializeField]private RectTransform fadeBG;
    private void Awake() {
        fadeBG = GetComponent<RectTransform>();
        HideUI();
    }

    private void HideUI(){
        LeanTween.alpha(fadeBG, 0f, 0.8f);
    }
    public void ShowUIDead(){
        LeanTween.alpha(fadeBG, 1f, 0.3f).setOnComplete(
            ()=> SceneManager.LoadScene("normal") // ini ganti ke nama first scene entar
        );
    }

    public void ShowUINextScene(){
        LeanTween.alpha(fadeBG, 1f, 0.5f).setOnComplete(
            ()=> ChooseNextScene()
        );
    }
    private void ChooseNextScene(){
        TheGameManager gameManager = TheGameManager.Instance;
        if(gameManager.GetEnvironment() == TheGameManager.gameEnvironment.normal){
            SceneManager.LoadScene("windy");
        }
    }
    public void ShowUIExit(){
        LeanTween.alpha(fadeBG, 1f, 0.3f).setOnComplete(
            ()=> Application.Quit()
        );
    }

}
