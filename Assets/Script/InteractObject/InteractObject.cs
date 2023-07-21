using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lever
{
    public void Interact(Interact_Lever lever){
        Debug.Log("lever");
    }
}

public class Beacon
{
    public void Interact(){
        Debug.Log("Beacon");
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
    [SerializeField]private Interact_LightSource lightSource;
    [SerializeField]private Interact_KeyGate keyGate;

    private PlayerLight playerLight;

    
    private void Awake() {
        popUp = gameObject.transform.GetChild(0).gameObject;
        
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
    }
    private void Update() {
        
        if(isInRange && gameManager.IsIngame()){
            if(GetInputInteract()){
                if(interactObjectType == InteracTObjectType.Lever)
                {
                    LeverClass.Interact(lever);
                }
                else if(interactObjectType == InteracTObjectType.Beacon)
                {
                    BeaconClass.Interact();
                }
                else if(interactObjectType == InteracTObjectType.LightSource)
                {
                    LightSourceClass.Interact(lightSource, playerLight);
                }
            }
        }
    }

    private bool GetInputInteract(){
        return Input.GetKeyDown(KeyCode.F);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag(compareTagCollider))
        {
            if(interactObjectType == InteracTObjectType.KeyGate){
                if(gameManager.IsIngame() && !keyGate.GetHasInteract()){
                    PlayerInteract playerInteract = other.GetComponentInParent<PlayerInteract>();
                    playerInteract.AddKey();
                    KeyGateClass.Interact(keyGate, playerInteract);
                }
                
            }
            else{
                isInRange = true;
                popUp.gameObject.SetActive(true);
                playerLight = other.GetComponentInParent<PlayerLight>();
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag(compareTagCollider))
        {
            isInRange = false;
            popUp.gameObject.SetActive(false);
            playerLight = null;
        }
    }
}
