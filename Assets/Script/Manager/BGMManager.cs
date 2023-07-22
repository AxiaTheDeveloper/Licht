using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField]private AudioSource BGM;
    private float volume;
    private const string PLAYER_PREF_BGM_VOLUME = "BGM_Volume";
    public static BGMManager Instance{get; private set;}

    private void Awake() {
        if(!Instance){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start() {
        volume = PlayerPrefs.GetFloat(PLAYER_PREF_BGM_VOLUME, 0.3f);
        // BGM.volume = volume;
    }
    private void Update() {
        InputChangeVolume();
        
    }
    private void InputChangeVolume(){
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            UpdateBGM_Volume(-0.1f);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)){
            UpdateBGM_Volume(0.1f);
        }

        
    }

    public void UpdateBGM_Volume(float upVolume){
        if(volume < 1 && upVolume == 0.1f){
            volume += upVolume;
            // BGM.volume = volume;
            PlayerPrefs.SetFloat(PLAYER_PREF_BGM_VOLUME, volume);
        }
        else if(volume >= 0.1f && upVolume == -0.1f){
            volume += upVolume;
            if(volume < 0.1f){
                volume = 0;
            }
            // BGM.volume = volume;
            PlayerPrefs.SetFloat(PLAYER_PREF_BGM_VOLUME, volume);
        }
        
        
    }
    public void DestroyInstance(){
        Destroy(gameObject);
    }
}
