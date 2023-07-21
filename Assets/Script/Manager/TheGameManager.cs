using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGameManager : MonoBehaviour
{
    public static TheGameManager Instance{get; private set;}
    public enum gameState{
        cinematic, inGame, Pause, Finish, Dead
    }
    [SerializeField]private gameState state;

    private bool isPause;

    private void Awake() {
        Instance = this;
        isPause = false;
    }

    private void Start() {
        state = gameState.cinematic;
    }

    public void ChangeToInGame(){
        state = gameState.inGame;
    }
    
    public void ChangeToPause(){
        isPause = !isPause;
        if(isPause){
            state = gameState.Pause;
        }
        else{
            state = gameState.inGame;
        }
    }

    public void FinishGame(){
        state = gameState.Finish;
    }

    public void DeadState(){
        state = gameState.Dead;
    }

    
    public bool IsIngame(){
        return state == gameState.inGame;
    }
}
