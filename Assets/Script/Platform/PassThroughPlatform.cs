using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : MonoBehaviour
{
    private Collider2D colliderPlatform;
    private bool isPlayerOnPlatform;
    private Collider2D playerCollider;
    [SerializeField]private float UnignoreCollisionCountdown;
    
    private void Awake() {
        colliderPlatform = GetComponent<Collider2D>();
    }
    private void Start() {
        isPlayerOnPlatform = false;
    }
    private void Update() {
        
        if(isPlayerOnPlatform && GetInputDown()){
            colliderPlatform.enabled = false;
            isPlayerOnPlatform = false;
            StartCoroutine(UnignoreCountdown());
        }
    }
    private IEnumerator UnignoreCountdown(){
        yield return new WaitForSeconds(UnignoreCollisionCountdown);
        colliderPlatform.enabled = true;
    }
    private bool GetInputDown(){
        return Input.GetKeyDown(KeyCode.S);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            // Debug.Log("ko kah?" + playerCollider);
            // playerCollider = other.gameObject.GetComponent<Collider2D>();
            isPlayerOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            // Debug.Log("koko?" + playerCollider);
            // playerCollider = null;
            isPlayerOnPlatform = false;
        }
    }

}
