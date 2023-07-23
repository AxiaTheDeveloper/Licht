using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPlayer : MonoBehaviour
{
    public static TimerPlayer Instance{get; private set;}

    private float timer;
    private void Awake() {
        if(!Instance){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else{
            Destroy(gameObject);
        }
        
    }

    private void Update() {
        if(TheGameManager.Instance.IsIngame()){
            timer += Time.fixedDeltaTime;
        }
    }

    public void RestartTimer(){
        timer = 0f;
    }
    public float GetTime(){
        return timer;
    }
}
