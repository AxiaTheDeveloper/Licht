using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lever
{
    public void Interact(){
        Debug.Log("lever");
    }
}
public class Torch
{
    public void Interact(){
        Debug.Log("Torch");
    }
}
public class InteractObject : MonoBehaviour
{
    private TheGameManager gameManager;
    [SerializeField]private GameObject popUp;
    [SerializeField]private bool isInRange;
    public enum InteractType{
        lightCollider, smallCollider
    }
    [SerializeField]private InteractType interactType;
    private string compareTagCollider;

    public enum InteracTObjectType{
        Lever, Torch
    }
    [SerializeField]private InteracTObjectType interactObjectType;

    private Lever LeverClass;
    private Torch TorchClass;
    

    
    private void Awake() {
        popUp = gameObject.transform.GetChild(0).gameObject;
        
        isInRange = false;
        if(interactType == InteractType.lightCollider){
            compareTagCollider = "ColliderLampu";
        }
        else if(interactType == InteractType.smallCollider){
            compareTagCollider = "ColliderKecil";
        }

        if(interactObjectType == InteracTObjectType.Lever)
        {
            LeverClass = new Lever();
        }
        else if(interactObjectType == InteracTObjectType.Torch)
        {
            TorchClass = new Torch();
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
                    LeverClass.Interact();
                }
                else if(interactObjectType == InteracTObjectType.Torch)
                {
                    TorchClass.Interact();
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
            isInRange = true;
            popUp.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag(compareTagCollider))
        {
            isInRange = false;
            popUp.gameObject.SetActive(false);
        }
    }
}
