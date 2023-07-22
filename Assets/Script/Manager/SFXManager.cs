using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField]private AudioSource SFXA;
    private float volume;
    private const string PLAYER_PREF_SFX_VOLUME = "SFX_Volume";

    private void Start() {
        volume = PlayerPrefs.GetFloat(PLAYER_PREF_SFX_VOLUME, 0.3f);
        // UpdateAllVolume();
    }
    private void Update() {
        InputChangeVolume();
    }
    private void InputChangeVolume(){
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            UpdateBGM_Volume(-0.1f);
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow)){
            UpdateBGM_Volume(0.1f);
        }
    }

    public void UpdateBGM_Volume(float upVolume){
        if(volume < 1 && upVolume == 0.1f){
            volume += upVolume;
            // UpdateAllVolume();
            PlayerPrefs.SetFloat(PLAYER_PREF_SFX_VOLUME, volume);
        }
        else if(volume >= 0.1f && upVolume == -0.1f){
            volume += upVolume;
            if(volume < 0.1f){
                volume = 0;
            }
            // UpdateAllVolume();
            PlayerPrefs.SetFloat(PLAYER_PREF_SFX_VOLUME, volume);
        }
        
        
    }
    private void UpdateAllVolume(){
        SFXA.volume = volume;
    }
}