using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Interact_KeyGate : MonoBehaviour
{
    private PlayerInteract player;
    private bool hasInteracted;
    [SerializeField]private float keyMoveSpeed;
    private Transform keyPositionPlayer;
    [SerializeField]private float wasMoveDuration, moveDelay;

    private bool wasMove;

    [SerializeField]private int keyID;

    [SerializeField]private Light2D glow;
    private void Awake() {
        hasInteracted = false;
        wasMove = false;
        glow.intensity = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("ColliderLampu")){
            glow.intensity = 1f;
        }
    }

    public void GivePlayerInteract(PlayerInteract playerSet){
        player = playerSet;
        keyPositionPlayer = player.GetPlayerPositionKey();
        hasInteracted = true;
    }

    public int GetKeyID(){
        return keyID;
    }
    private void Update() {
        if(player){
            if(player.GetHasUseKey()){
                Destroy(this.gameObject);
            }
            if(transform.position != keyPositionPlayer.position && !wasMove){
                wasMove = true;
                // StartCoroutine(moveDelayCountDown());
                LeanTween.move(this.gameObject, keyPositionPlayer.position, keyMoveSpeed).setEase(LeanTweenType.easeOutElastic);
                StartCoroutine(wasMoveCountDown());
            }
        }
    }
    
    private IEnumerator wasMoveCountDown(){
        yield return new WaitForSeconds(wasMoveDuration);
        LeanTween.cancel(this.gameObject);
        wasMove = false;
    }

    public bool GetHasInteract(){
        return hasInteracted;
    }
}
