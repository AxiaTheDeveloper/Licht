using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePuzzle : MonoBehaviour
{
    private TheGameManager gameManager;
    public enum GateOpenType{
        Lever, Beacon, Key
    }
    [SerializeField]private GateOpenType gateOpenType;
    public enum PuzzleType{
        BlockingRoadMove, Missing, BlockingRoadGone
    }
    //blocking road move - gerak dr tmpt asal
    //missing - ntr puzzle solve, gameobject muncul
    //blockingroadgone - puzzle solve, collider ilang smbil animasi buka pintu
    [SerializeField]private PuzzleType puzzleType;

    //ga mungkin gateopentype key bs missing puzzle type
    
    
    

    [Header("Gate Open - Lever")]
    //well kalo misal emg nanti sapa tau pengen 2 lever ato lebi, lever dikasih bool buat dicek kek beacons, bs aja true false true gitukan, bikin bool list gitu buat jd checkernya, kalo ga sesuai leverOn = false, kalo sesuai br on, tp ya kalo mau
    [SerializeField]private InteractObject lever;



    

    [Header("Gate Open - Beacon")]
    [SerializeField]private List<Interact_Beacon> beacons;


    [Header("Puzzle - BlockingRoadMove")]
    [SerializeField] private bool leverOn;
    [SerializeField]private Vector3 obstaclePos_Start;
    [SerializeField]private Vector3 obstaclePos_End;
    [SerializeField]private float obstacle_Speed;
    [Header("Puzzle - Missing - Gausa dimasukkin Gapapa")]
    [SerializeField]private SpriteRenderer spriteRenderer;// dipake missing dan blocking road gone
    [SerializeField]private float colorChange_Speed;
    [Header("Puzzle - BlockingRoadGone - Gausa dimasukkin - Gapapa")]
    [SerializeField]private Collider2D obstacleCollider; // dipake missing dan blocking road gone
    //mungkin playeranimator di sini kalo perlu
    
    private void Awake() {
        if(gateOpenType == GateOpenType.Lever){
            obstaclePos_Start = transform.position;
            leverOn = false;
        }
        if(puzzleType == PuzzleType.Missing || puzzleType == PuzzleType.BlockingRoadGone){
            if(!obstacleCollider){
                obstacleCollider = GetComponent<Collider2D>();
            }
            if(!spriteRenderer){
                spriteRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            }   
            
            

            //ato ini bisa tinggal lsg diganti di inspector

            // if(puzzleType == PuzzleType.Missing){
            //     obstacleCollider.enabled = false;
            //     spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            // }
        }
        
        
    }
    private void Start() {
        if(gateOpenType == GateOpenType.Beacon){
            foreach(Interact_Beacon beacon in beacons)
            {
                beacon.OnBeaconLitUp += beacon_OnBeaconLitUp;
            }
        }
        if(gateOpenType == GateOpenType.Lever){
            lever.GetInteract_Lever().OnUsedLever += lever_OnUsedLever;
        }

        gameManager = TheGameManager.Instance;
    }

    private void lever_OnUsedLever(object sender, EventArgs e)
    {
        //kalo mo kek 3 lever pake cr bawah
        SolvedPuzzle();
    }

    private void beacon_OnBeaconLitUp(object sender, EventArgs e)
    {
        bool allLitUp = true;
        foreach(Interact_Beacon beacon in beacons){
            if(!beacon.IsLitUp()){
                allLitUp = false;
                break;
            }
        }
        if(allLitUp){
            SolvedPuzzle();
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        if(gateOpenType == GateOpenType.Key){
            if(other.gameObject.CompareTag("Player")){
                PlayerInteract player = other.gameObject.GetComponent<PlayerInteract>();
                if(player.GetHasKey() && gameManager.IsIngame()){
                    player.UseKey();
                    SolvedPuzzle();
                }
            }
        }
        
    }

    private void SolvedPuzzle(){
        if(puzzleType == PuzzleType.BlockingRoadMove)
        {   
            if(!leverOn)
            {
                LeanTween.move(gameObject, obstaclePos_End, obstacle_Speed);
            }
            else
            {
                LeanTween.move(gameObject, obstaclePos_Start, obstacle_Speed);
            }
            leverOn = !leverOn;
            
        }
        else if(puzzleType == PuzzleType.Missing)
        {
            LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 1f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad).setOnComplete(
                ()=> obstacleCollider.enabled = true
            );
        }
        else if(puzzleType == PuzzleType.BlockingRoadGone)
        {
            obstacleCollider.enabled = false;
            //jalankan animasi or ya ini fade out~
            LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 0f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad);

        }
    }


}
