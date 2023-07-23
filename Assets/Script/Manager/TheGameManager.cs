using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TheGameManager : MonoBehaviour
{
    public static TheGameManager Instance{get; private set;}
    public enum gameState{
        cinematic, inGame, Pause, Finish, Dead, Credit
    }
    [SerializeField]private gameState state;
    
    public enum gameEnvironment{
        normal, windy
    }
    [SerializeField]private gameEnvironment environment;

    private bool isPause;

    [SerializeField]private UIFadeBG fadeBG;

    [SerializeField]private float inputCooldownMax;
    private float inputCooldown;
    [SerializeField]private GameObject pauseUI;

    [SerializeField]private float ExitCounterMax;
    private float exitCounter;
    private bool isInputPauseInGame;
    [SerializeField]private CreditUI credit;

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
    public gameEnvironment GetEnvironment(){
        return environment;
    }

    public void SetEnvironment(string weatherCondition)
    {
        if(weatherCondition != null)
        {
            if(weatherCondition == "normal")
            {
                environment = gameEnvironment.normal;
            }
            else if(weatherCondition == "windy")
            {
                environment = gameEnvironment.windy;
            }
        }
        
    }

    private void Update() {
        if(state == gameState.inGame){
            if(GetInputDownPause() && inputCooldown <= 0){
                isInputPauseInGame = true;
                inputCooldown = inputCooldownMax;
                
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
        if(isInputPauseInGame){
            if(exitCounter < ExitCounterMax){
                exitCounter += Time.deltaTime;
            }
            if(exitCounter > ExitCounterMax){
                state = gameState.cinematic;
                PlayerLight.Instance.LightsOutExit();
                fadeBG.ShowUIExit();
            }
        }

    }
    private bool GetInputDownPause(){
        return Input.GetKeyDown(KeyCode.Escape);
    }
    private bool GetInputUpPause(){
        return Input.GetKeyUp(KeyCode.Escape);
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
        
        if(SceneManager.GetActiveScene().name != "Level2"){ // intinya kalo bukan last scene
            fadeBG.ShowUINextScene();
            state = gameState.Finish;
        }
        else{
            state = gameState.cinematic;
            TimelineManager.Instance.Start_Ending();
        }
    }
    public void ChangeToCredit(){
        state = gameState.Credit;
    }

    public void DeadState(){
        state = gameState.Dead;
        SFXManager.Instance.PlaySFX_PlayerDead();
        fadeBG.ShowUIDead();
    }

    
    public bool IsIngame(){
        return state == gameState.inGame;
    }
}
