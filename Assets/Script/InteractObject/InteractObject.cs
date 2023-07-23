using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lever
{
    public void Interact(Interact_Lever lever){
        lever.UseLever();
    }
}

public class Beacon
{
    public void Interact(Interact_Beacon beacon, PlayerLight playerLight){
        beacon.LightUpBeacon(playerLight);
    }
}

public class LightSource
{
    public void Interact(Interact_LightSource lightSource, PlayerLight playerLight){
        lightSource.GetLightSource(playerLight);
    }
}

public class KeyGate
{
    public void Interact(Interact_KeyGate keyGate, PlayerInteract playerInteract){
        keyGate.GivePlayerInteract(playerInteract);
    }
}
public class InteractObject : MonoBehaviour
{
    private TheGameManager gameManager;
    [SerializeField]private GameObject popUp;
    [SerializeField]private bool isInRange;
    [SerializeField]private bool canInteractManyTimes;
    private bool alreadyInteract;

    public enum InteractType{
        lightCollider, smallCollider, playerCollider
    }
    [SerializeField]private InteractType interactType;
    private string compareTagCollider;

    public enum InteracTObjectType{
        Lever, Beacon, LightSource, KeyGate
    }
    [SerializeField]private InteracTObjectType interactObjectType;

    private Lever LeverClass;
    private Beacon BeaconClass;
    private LightSource LightSourceClass;
    private KeyGate KeyGateClass;

    [SerializeField]private Interact_Lever lever;
    [SerializeField]private Interact_Beacon beacon;
    [SerializeField]private Interact_LightSource lightSource;
    [SerializeField]private Interact_KeyGate keyGate;

    private PlayerLight playerLight;


    //khusus siapapun yg kehubung ke obstacle puzzle dn bisa dipake terus~
    private bool isObstacleMoving; // ini tu sebenernya buat tandain kalo obstacle lg gerak ya gabisa di interact dl takutnya error kalo obstacle lg ke bawah eh diinteract balik eh hrs balik ke atas ntr malah error

    
    private void Awake() {
        popUp = gameObject.transform.GetChild(0).gameObject;
        alreadyInteract = false;
        
        isInRange = false;
        if(interactType == InteractType.lightCollider){
            compareTagCollider = "ColliderLampu";
        }
        else if(interactType == InteractType.smallCollider){
            compareTagCollider = "ColliderKecil";
        }
        else if(interactType == InteractType.playerCollider){
            compareTagCollider = "Player";
        }

        if(interactObjectType == InteracTObjectType.Lever)
        {
            LeverClass = new Lever();
        }
        else if(interactObjectType == InteracTObjectType.Beacon)
        {
            BeaconClass = new Beacon();
        }
        else if(interactObjectType == InteracTObjectType.LightSource)
        {
            LightSourceClass = new LightSource();
        }
        else if(interactObjectType == InteracTObjectType.KeyGate)
        {
            KeyGateClass = new KeyGate();
        }
    }
    private void Start() {
        gameManager = TheGameManager.Instance;
        popUp.gameObject.SetActive(false);
        isObstacleMoving = false;
    }
    private void Update() {
        
        if(isInRange && gameManager.IsIngame()){
            if(GetInputInteract()){
                if(canInteractManyTimes){
                    if(!isObstacleMoving){
                        InteractFunction();
                    }
                    
                }
                else{
                    if(!alreadyInteract){
                        alreadyInteract = true;
                        popUp.gameObject.SetActive(false);
                        InteractFunction();
                    }
                }
                
            }
        }
    }

    private void InteractFunction(){
        alreadyInteract = true;
        if(interactObjectType == InteracTObjectType.Lever)
        {
            LeverClass.Interact(lever);
        }
        else if(interactObjectType == InteracTObjectType.Beacon)
        {
            if(!beacon.IsLitUp()){
                BeaconClass.Interact(beacon, playerLight);
            }
            if(beacon.IsLitUp()){
                popUp.gameObject.SetActive(false);
            }
            
        }
        else if(interactObjectType == InteracTObjectType.LightSource)
        {
            LightSourceClass.Interact(lightSource, playerLight);
        }
    }

    private bool GetInputInteract(){
        return Input.GetKeyDown(KeyCode.F);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag(compareTagCollider))
        {
            if(interactObjectType == InteracTObjectType.KeyGate){
                PlayerInteract playerInteract = other.GetComponentInParent<PlayerInteract>();
                if(gameManager.IsIngame() && !keyGate.GetHasInteract() && !playerInteract.GetHasKey()){
                    playerInteract.AddKey(keyGate);
                    KeyGateClass.Interact(keyGate, playerInteract);
                }
            }
            else{
                isInRange = true;
                
                if(canInteractManyTimes){
                    if(interactObjectType == InteracTObjectType.Beacon){
                        if(!beacon.IsLitUp()){
                            popUp.gameObject.SetActive(true);
                        }
                    }
                    else{
                        if(interactObjectType == InteracTObjectType.Lever){
                            if(!isObstacleMoving){
                                popUp.gameObject.SetActive(true);
                            }
                        }
                        else{
                            popUp.gameObject.SetActive(true);
                        }
                        
                    }
                    
                }
                else{
                    if(!alreadyInteract){
                        popUp.gameObject.SetActive(true);
                    }
                }
                playerLight = other.GetComponentInParent<PlayerLight>();
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag(compareTagCollider))
        {
            isInRange = false;
            if(popUp.activeSelf){
                popUp.gameObject.SetActive(false);
            }
            playerLight = null;
        }
    }

    public Interact_Lever GetInteract_Lever(){
        return lever;
    }

    public void changeIsMoving(bool change){
        isObstacleMoving = change;
        //jd pas ini dimasukkin yang change = false abis dijalanin animasi gerak itu dan trnyata gabisa diinteract lg levernya ya si popup gabakal berubah jadi true
        if(canInteractManyTimes){
            popUp.gameObject.SetActive(!change);
        }
        
    }
    public bool GetCanInteractManyTimes(){
        return canInteractManyTimes;
    }

    public void CannotInteractManyTimes(){
        //function ini khusus yg tdnya infinite dan gapunya syarat utk matiin dirinya jd bisa matiin dirinya, misal lever bisa digerakkin infinite sebelum berhasil solve puzzle, pas puzzle ud solve nah brarti ini dinyalakan functionnya
        if(canInteractManyTimes){
            canInteractManyTimes = false;
        }
    }
    public void CanInteractManyTimes(){
        if(!canInteractManyTimes){
            canInteractManyTimes = true;
        }
    }
}
