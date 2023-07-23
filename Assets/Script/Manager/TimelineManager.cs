using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager Instance{get; private set;}
    [SerializeField]private PlayableDirector director;

    [Header("Cauldron Type")]
    [SerializeField]private TimelineAsset ending;
    [SerializeField]private CreditUI credit;
    private bool startTimeline;
    private PlayerMovement playerMovement;
    
    private void Awake() {
        Instance = this;
        startTimeline = false;
    }

    private void Start() {
        director.stopped += OnTimelineStopped;
        playerMovement = PlayerMovement.Instance;
    }
    private void Update() {
        if(startTimeline && playerMovement.GetIsOnGround()){
            playerMovement.gameObject.transform.position = new Vector3(130f, 30f, 0f);
            startTimeline = false;
            director.Play();
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {

        credit.ShowUI();
    }

    public void Start_Ending(){
        director.playableAsset = ending;
        startTimeline = true;
        
    }
}
