using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]private Transform keyPosition;
    private PlayerMovement playerMovement;
    // private Vector3 playerPositionLast;

    private bool hasKey, hasUseKey;
    //kalo semua kunci sama dan bisadobel ? kita pake counter ae, kalo ga pake bool, buat ngecek ke pintu, trus kalo pintu is locked dan trnyata player masuk triggernya dn punya kunci dia lsg ambil kuncinya dan unlock pintu ~

    private bool isPlayerMove;
    private Interact_KeyGate keyGate;
    private void Awake() {
        // playerPosNow = GetComponent<Transform>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start() {
        hasKey = false;
        hasUseKey = false;
    }

    public bool GetIsPlayerMove(){
        return isPlayerMove;
    }
    // public Vector3 GetPlayerPositionNow(){
    //     return playerPosNow.position;
    // }
    public Transform GetPlayerPositionKey(){
        return keyPosition;
    }
    public void AddKey(Interact_KeyGate newKey){
        hasKey = true;
        hasUseKey = false;
        keyGate = newKey;
    }
    //cuma bisa ama 1 kunci aja

    //ini function yg dipake pintu kalo gethaskey = true dan collider ngecollide dgn pintu
    public void UseKey(){
        hasKey = false;
        hasUseKey = true;
        keyGate = null;
    }

    public bool GetHasKey(){
        return hasKey;
    }
    public bool GetHasUseKey(){
        return hasUseKey;// kalo si key ada konek ke player dan playerhasusekey == true, maka key hancurin dirinya
    }
    public Interact_KeyGate GetKey(){
        return keyGate;
    }
}
