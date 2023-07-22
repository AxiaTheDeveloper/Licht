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
    public void ShowUI(){
        LeanTween.alpha(fadeBG, 1f, 0.3f).setOnComplete(
            ()=> SceneManager.LoadScene(SceneManager.GetActiveScene().name)
        );
    }

}
