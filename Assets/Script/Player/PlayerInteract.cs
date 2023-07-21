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
    private void Awake() {
        // playerPosNow = GetComponent<Transform>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start() {
        hasKey = false;
        hasUseKey = false;
    }
    private void Update() {
        // if(playerPosNow.position == playerPositionLast)
        //     {
        //         isPlayerMove = false;
                
        //     }
        // else if(playerPosNow.position != playerPositionLast)
        // {
        //     if(playerPositionLast.y == playerPosNow.position.y && playerPositionLast.x != playerPosNow.position.x)
        //     {
        //         if(playerMovement.GetDirX() == 0)
        //         {
        //             isPlayerMove = false;
        //         }
        //         else{
        //             isPlayerMove = true;
                            
        //         }
                        
        //     }
        //     else if(playerPositionLast.y != playerPosNow.position.y && playerPositionLast.x == playerPosNow.position.x)
        //     {
        //         isPlayerMove = true;
        //     }
        //     else if(playerPositionLast.y != playerPosNow.position.y && playerPositionLast.x != playerPosNow.position.x)
        //     {
        //         isPlayerMove = true;
        //     }
            
        //     playerPositionLast = playerPosNow.position;   
        // }

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
    public void AddKey(){
        hasKey = true;
        hasUseKey = false;
    }

    //ini function yg dipake pintu kalo gethaskey = true dan collider ngecollide dgn pintu
    public void UseKey(){
        hasKey = false;
        hasUseKey = true;
    }

    public bool GetHasKey(){
        return hasKey;
    }
    public bool GetHasUseKey(){
        return hasUseKey;// kalo si key ada konek ke player dan playerhasusekey == true, maka key hancurin dirinya
    }
}
