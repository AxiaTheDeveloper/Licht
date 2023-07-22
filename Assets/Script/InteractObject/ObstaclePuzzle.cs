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
    [SerializeField]private List<Interact_Lever> levers;
    [Tooltip("Sesuaiin ama byk lever")]
    [SerializeField]private bool[] leverPuzzleAnswer;
    [SerializeField]private bool isAnswerRight;
    [SerializeField] private bool isPermanentSolve; // kalo true berarti kalo misal puzzle solve(gerak pertama, maka gabakal bisa digerakkin lg), ini jg khusus lever si
    



    

    [Header("Gate Open - Beacon")]
    [SerializeField]private List<Interact_Beacon> beacons;


    [Header("Puzzle - BlockingRoadMove")]
    
    [SerializeField] private List<InteractObject> interactObjects; // ini khusus utk lever aja ya, karena cuma lever doang yg bisa revert balik dirinya
    [SerializeField] private bool atStart;
    
    [SerializeField]private Vector3 obstaclePos_Start;
    [SerializeField]private Vector3 obstaclePos_End;
    [SerializeField]private float obstacle_Speed;
    [Header("Puzzle - Missing - Gausa dimasukkin Gapapa")]
    [SerializeField]private bool isMissing;
    [SerializeField]private SpriteRenderer spriteRenderer;// dipake missing dan blocking road gone
    [SerializeField]private float colorChange_Speed;
    [Header("Puzzle - BlockingRoadGone - Gausa dimasukkin - Gapapa")]
    [SerializeField]private Collider2D obstacleCollider; // dipake missing dan blocking road gone
    //mungkin playeranimator di sini kalo perlu


    //lever bisa solve semua, tp yg trakhir isPermanence hrs dinyalain
    // yg lain jg bs solve semua 
    
    private void Awake() {
        if(gateOpenType == GateOpenType.Lever){
            
            isAnswerRight = false;
        }
        if(puzzleType == PuzzleType.Missing || puzzleType == PuzzleType.BlockingRoadGone){
            if(!obstacleCollider){
                obstacleCollider = GetComponent<Collider2D>();
            }
            if(!spriteRenderer){
                spriteRenderer = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            }   
            if(puzzleType == PuzzleType.Missing){
                isMissing = true;
            }
        }
        if(puzzleType == PuzzleType.BlockingRoadMove){
            obstaclePos_Start = transform.position;
            atStart = true;
        }
        if(gateOpenType == GateOpenType.Lever){
            foreach(Interact_Lever lever in levers){
                interactObjects.Add(lever.gameObject.GetComponent<InteractObject>());
            }
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
            foreach(Interact_Lever lever in levers)
            {
                lever.OnUsedLever += lever_OnUsedLever;
            }
            
        }

        gameManager = TheGameManager.Instance;
    }

    private void lever_OnUsedLever(object sender, EventArgs e)
    {
        bool allSame = true;
        for(int i=0;i<levers.Count;i++){
            if(leverPuzzleAnswer[i] != levers[i].GetLeverOn()){
                allSame = false;
                break;
            }
        }
        isAnswerRight = allSame;
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

    private void Do_CanInteractManyTimes(){
        foreach(InteractObject interactObject in interactObjects){
            if(interactObject.GetCanInteractManyTimes()){
                interactObject.changeIsMoving(false);
            } 
        }
        
        
    }

    private void Do_CanInteractManyTimes_ForMissing(){
        obstacleCollider.enabled = true;
        Do_CanInteractManyTimes();

    }

    private void SolvedPuzzle(){
        if(puzzleType == PuzzleType.BlockingRoadMove)
        {   
            if(atStart)
            {
                if(gateOpenType == GateOpenType.Lever){
                    if(isAnswerRight){
                        atStart = !atStart;
                        foreach(InteractObject interactObject in interactObjects){
                            if(interactObject.GetCanInteractManyTimes()){
                                interactObject.changeIsMoving(true);
                            } 
                            if(isPermanentSolve){
                                interactObject.CannotInteractManyTimes();
                            }
                        }
                        LeanTween.move(gameObject, obstaclePos_End, obstacle_Speed).setOnComplete(
                            ()=> Do_CanInteractManyTimes()
                        );
                        
                    }
                }
                else{
                    atStart = !atStart;
                    LeanTween.move(gameObject, obstaclePos_End, obstacle_Speed);
                    
                }
                
            }
            else
            {
                if(gateOpenType == GateOpenType.Lever){
                    if(!isAnswerRight){
                        atStart = !atStart;
                        foreach(InteractObject interactObject in interactObjects){
                            if(interactObject.GetCanInteractManyTimes()){
                                interactObject.changeIsMoving(true);
                            } 
                        }
                        LeanTween.move(gameObject, obstaclePos_Start, obstacle_Speed).setOnComplete(
                            ()=> Do_CanInteractManyTimes()
                        );
                        
                    }
                }
                else{
                    atStart = !atStart;
                    LeanTween.move(gameObject, obstaclePos_Start, obstacle_Speed);
                    
                }
                
            }
            
            
        }
        else if(puzzleType == PuzzleType.Missing)
        {
            if(isMissing){
                if(gateOpenType == GateOpenType.Lever){
                    if(isAnswerRight){
                        isMissing = !isMissing;
                        foreach(InteractObject interactObject in interactObjects){
                            if(interactObject.GetCanInteractManyTimes()){
                                interactObject.changeIsMoving(true);
                            } 
                            if(isPermanentSolve){
                                interactObject.CannotInteractManyTimes();
                            }
                        }
                        LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 1f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad).setOnComplete(
                            ()=> Do_CanInteractManyTimes_ForMissing()
                        );
                    }
                }
                else{
                    isMissing = !isMissing;
                    LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 1f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad).setOnComplete(
                        ()=> obstacleCollider.enabled = true
                    );
                    
                }
                
            }
            else{
                if(gateOpenType == GateOpenType.Lever){
                    if(!isAnswerRight){
                        isMissing = !isMissing;
                        foreach(InteractObject interactObject in interactObjects){
                            if(interactObject.GetCanInteractManyTimes()){
                                interactObject.changeIsMoving(true);
                            } 
                        }
                        obstacleCollider.enabled = false;
                        LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 0f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad).setOnComplete(
                            ()=> Do_CanInteractManyTimes()
                        );
                        
                    }
                }
                else{
                    isMissing = !isMissing;
                    obstacleCollider.enabled = false;
                    LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 0f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad);
                    
                }
                
            }
            
            
        }
        else if(puzzleType == PuzzleType.BlockingRoadGone)
        {
            obstacleCollider.enabled = false;
            //jalankan animasi or ya ini fade out~
            LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 0f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad);

        }
    }



}
