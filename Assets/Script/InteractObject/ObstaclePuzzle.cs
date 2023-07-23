using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePuzzle : MonoBehaviour
{
    private TheGameManager gameManager;
    private SFXManager sfxManager;
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
    
    [Header("Gate Open - Key, keyid sesuaiin ama key yg mo disamain")]
    [SerializeField]private int keyID;
    

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
    [SerializeField] private bool atStart, isTwoGate;
    [SerializeField] private GameObject Gate1, Gate2;
    
    [SerializeField]private Vector3 obstaclePos_Start_1, obstaclePos_Start_2;
    [SerializeField]private Vector3 obstaclePos_End_1, obstaclePos_End_2;
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
            if(isTwoGate){
                if(atStart){
                    obstaclePos_Start_1 = Gate1.transform.localPosition;
                    obstaclePos_Start_2 = Gate2.transform.localPosition;
                }
                else{
                    obstaclePos_End_1 = Gate1.transform.localPosition;
                    obstaclePos_End_2 = Gate2.transform.localPosition;
                }
                
                
            }
            else{
                if(atStart){
                    obstaclePos_Start_1 = Gate1.transform.localPosition;
                }
                else{
                    obstaclePos_End_1 = Gate1.transform.localPosition;
                }
                
            }
            
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
        sfxManager = SFXManager.Instance;
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
                    if(keyID == player.GetKey().GetKeyID()){
                        player.UseKey();
                        SolvedPuzzle();
                    }
                    
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
    private void Do_CanInteractManyTimes_ForBlockingRoad(bool change){
        Gate1.GetComponent<Collider2D>().enabled = change;
        Do_CanInteractManyTimes();

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
                        gameObject.GetComponent<Collider2D>().enabled = false;
                        atStart = !atStart;
                        sfxManager.PlaySFX_OpenGate();
                        foreach(InteractObject interactObject in interactObjects){
                            if(interactObject.GetCanInteractManyTimes()){
                                interactObject.changeIsMoving(true);
                            } 
                            if(isPermanentSolve){
                                interactObject.CannotInteractManyTimes();
                            }
                            else{
                                interactObject.CanInteractManyTimes();
                            }
                        }
                        if(isTwoGate){
                            
                            LeanTween.moveLocal(Gate1, obstaclePos_End_1, obstacle_Speed).setOnComplete(
                                ()=> Do_CanInteractManyTimes_ForBlockingRoad(false)
                            );
                            LeanTween.moveLocal(Gate2, obstaclePos_End_2, obstacle_Speed).setOnComplete(
                                ()=> Gate2.GetComponent<Collider2D>().enabled = false
                            );
                        }
                        else{
                            LeanTween.moveLocal(Gate1, obstaclePos_End_1, obstacle_Speed).setOnComplete(
                                ()=> Do_CanInteractManyTimes_ForBlockingRoad(false)
                            );
                        }
                        
                        
                    }
                }
                else{
                    atStart = !atStart;
                    sfxManager.PlaySFX_OpenGate();
                    gameObject.GetComponent<Collider2D>().enabled = false;
                    // Debug.Log("solved");
                    if(isTwoGate){
                        
                        LeanTween.moveLocal(Gate1, obstaclePos_End_1, obstacle_Speed).setOnComplete(
                            ()=> Gate1.GetComponent<Collider2D>().enabled = false
                        );
                        LeanTween.moveLocal(Gate2, obstaclePos_End_2, obstacle_Speed).setOnComplete(
                            ()=> Gate2.GetComponent<Collider2D>().enabled = false
                        );
                    }
                    else{
                        // Debug.Log("solved???");
                        LeanTween.moveLocal(Gate1, obstaclePos_End_1, obstacle_Speed).setOnComplete(
                            ()=> Gate1.GetComponent<Collider2D>().enabled = false
                        );
                    }
                    
                    // LeanTween.move(gameObject, obstaclePos_End, obstacle_Speed); kalo gerak 1 doang
                    
                }
                
            }
            else
            {
                if(gateOpenType == GateOpenType.Lever){
                    if(!isAnswerRight){
                        atStart = !atStart;
                        sfxManager.PlaySFX_CloseGate();
                        gameObject.GetComponent<Collider2D>().enabled = true;
                        foreach(InteractObject interactObject in interactObjects){
                            if(interactObject.GetCanInteractManyTimes()){
                                interactObject.changeIsMoving(true);
                            } 
                        }
                        if(isTwoGate){
                            Gate1.GetComponent<Collider2D>().enabled = true;
                            Gate2.GetComponent<Collider2D>().enabled = true;
                            LeanTween.moveLocal(Gate1, obstaclePos_Start_1, obstacle_Speed).setOnComplete(
                                ()=> Do_CanInteractManyTimes()
                            );
                            LeanTween.moveLocal(Gate2, obstaclePos_Start_2, obstacle_Speed);
                        }
                        else{
                            Gate1.GetComponent<Collider2D>().enabled = true;
                            LeanTween.moveLocal(Gate1, obstaclePos_Start_1, obstacle_Speed).setOnComplete(
                                ()=> Do_CanInteractManyTimes()
                            );
                        }
                    }
                }
                else{
                    atStart = !atStart;
                    sfxManager.PlaySFX_CloseGate();
                    gameObject.GetComponent<Collider2D>().enabled = true;
                    if(isTwoGate){
                        Gate1.GetComponent<Collider2D>().enabled = true;
                        Gate2.GetComponent<Collider2D>().enabled = true;
                        LeanTween.moveLocal(Gate1, obstaclePos_Start_1, obstacle_Speed);
                        LeanTween.moveLocal(Gate2, obstaclePos_Start_2, obstacle_Speed);
                    }
                    else{
                        Gate1.GetComponent<Collider2D>().enabled = true;
                        LeanTween.moveLocal(Gate1, obstaclePos_Start_1, obstacle_Speed);
                    }
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
            sfxManager.PlaySFX_OpenDoor();
            //jalankan animasi or ya ini fade out~
            LeanTween.color(spriteRenderer.gameObject, new Color(1f, 1f, 1f, 0f), colorChange_Speed).setEase(LeanTweenType.easeOutQuad);

        }
    }



}
