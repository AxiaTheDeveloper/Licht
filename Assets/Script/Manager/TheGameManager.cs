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

    [SerializeField]private UIFadeBG fadeBG;

    [SerializeField]private float inputCooldownMax;
    private float inputCooldown;
    [SerializeField]private GameObject pauseUI;

    [SerializeField]private float ExitCounterMax;
    private float exitCounter;
    private bool isInputPauseInGame;

    private void Awake() {
        Instance = this;
        isPause = false;
        isInputPauseInGame = false;
        pauseUI.SetActive(false);
    }

    private void Start() {
        inputCooldown = 0;
        exitCounter = 0;
        state = gameState.cinematic;
    }

    private void Update() {
        if(state == gameState.inGame){
            if(GetInputDownPause() && inputCooldown <= 0){
                isInputPauseInGame = true;
                inputCooldown = inputCooldownMax;
                if(exitCounter < ExitCounterMax){
                    exitCounter += Time.deltaTime;
                }
                if(exitCounter > ExitCounterMax){
                    Debug.Log("Exit");
                }
            }
            if(GetInputUpPause() && exitCounter < ExitCounterMax){
                isInputPauseInGame = false;
                inputCooldown = inputCooldownMax;
                ChangeToPause();
                exitCounter = 0;
            }
        }
        else if(state == gameState.Pause){
            
            if(GetInputUpPause()  && inputCooldown <= 0){
                inputCooldown = inputCooldownMax;
                ChangeToPause();
            }
        }
        if(inputCooldown > 0){
            inputCooldown -= Time.deltaTime;
        }
        if(isi)

    }
    private bool GetInputDownPause(){
        return Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);
    }
    private bool GetInputUpPause(){
        return Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape);
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
        pauseUI.SetActive(isPause);
        PlayerMovement.Instance.PausePlayer(isPause);
    }

    public void FinishGame(){
        state = gameState.Finish;
    }

    public void DeadState(){
        state = gameState.Dead;
        fadeBG.ShowUI();
    }

    
    public bool IsIngame(){
        return state == gameState.inGame;
    }
}
