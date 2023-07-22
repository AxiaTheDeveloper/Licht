using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Lever_Animator : MonoBehaviour
{
    [SerializeField]private Interact_Lever lever;
    public void AnimationDone(){
        lever.AnimationFinish();
    }
}
