using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField]private AudioSource walkSFX, jumpSFX, useLeverSFX, getKeySFX, lightBeaconSFX, changeLightSourceSFX, openGateSFX, closeGateSFX, openDoorKeySFX, deadSFX, windSFX;
    private float volume;
    private const string PLAYER_PREF_SFX_VOLUME = "SFX_Volume";
    public static SFXManager Instance {get; private set;}
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        volume = PlayerPrefs.GetFloat(PLAYER_PREF_SFX_VOLUME, 0.3f);
        UpdateAllVolume();
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
            UpdateAllVolume();
            PlayerPrefs.SetFloat(PLAYER_PREF_SFX_VOLUME, volume);
        }
        else if(volume >= 0.1f && upVolume == -0.1f){
            volume += upVolume;
            if(volume < 0.1f){
                volume = 0;
            }
            UpdateAllVolume();
            PlayerPrefs.SetFloat(PLAYER_PREF_SFX_VOLUME, volume);
        }
        
        
    }

    private void UpdateAllVolume(){
        if(walkSFX)walkSFX.volume = volume;
        if(jumpSFX)jumpSFX.volume = volume;
        if(useLeverSFX)useLeverSFX.volume = volume;
        if(getKeySFX)getKeySFX.volume = volume;
        if(lightBeaconSFX)lightBeaconSFX.volume = volume;
        if(changeLightSourceSFX)changeLightSourceSFX.volume = volume;
        if(openGateSFX)openGateSFX.volume = volume;
        if(closeGateSFX)closeGateSFX.volume = volume;
        if(openDoorKeySFX)openDoorKeySFX.volume = volume;
        if(deadSFX)deadSFX.volume = volume;
        if(windSFX)windSFX.volume = volume;
    }

    public void PlaySFX_PlayerWalk(){ // playermovement
        walkSFX.Play();
    }
    public void StopSFX_PlayerWalk(){
        StartCoroutine(StopSFXWalk_Courotine());

    }
    private IEnumerator StopSFXWalk_Courotine(){
        yield return new WaitForSeconds(0.05f);
        walkSFX.Stop();
    }
    public bool isPlayedSFX_PlayerWalk(){
        return walkSFX.isPlaying;
    }

    public void PlaySFX_WindSFX(){ //ntr pas on player enter collider aja idk
        windSFX.Play();
    }
    public void StopSFX_WindSFX(){
        StartCoroutine(StopSFXWind_Courotine());
    }
    private IEnumerator StopSFXWind_Courotine(){
        yield return new WaitForSeconds(0.05f);
        windSFX.Stop();
    }
    public bool isPlayedSFX_WindSFX(){
        return windSFX.isPlaying;
    }

    public void PlaySFX_PlayerJump(){ // PlayerMovement
        jumpSFX.Play();
    }
    public void PlaySFX_UseLever(){ //interactObjects
        useLeverSFX.Play();
    }
    public void PlaySFX_GetKey(){
        getKeySFX.Play();
    }
    public void PlaySFX_LightBeacon(){
        lightBeaconSFX.Play();
    }
    public void PlaySFX_ChangeLightSource(){
        changeLightSourceSFX.Play();
    }
    public void PlaySFX_OpenGate(){
        openGateSFX.Play();
    }
    public void PlaySFX_CloseGate(){
        closeGateSFX.Play();
    }
    public void PlaySFX_OpenDoor(){ //
        openDoorKeySFX.Play();
    }
    public void PlaySFX_PlayerDead(){ //di TheGameManager
        deadSFX.Play();
    }

}
