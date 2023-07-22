using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseLine : MonoBehaviour
{
    [SerializeField]private bool isWinLine;
    private TheGameManager gameManager;

    private void Start() {
        gameManager = TheGameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            if(isWinLine){
                gameManager.FinishGame();
            }
            else{
                gameManager.DeadState();
            }
        }
    }
}
