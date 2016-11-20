using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public Camera seedCamera;
    public Camera worldCamera;
    public Camera selectCamera;

    public GameObject mainCanvas;
    public GameObject selectCanvas;

    public GameObject seed;

    

    

    public static GameManager instance;

    public enum GameState
    {
        GS_SEED,
        GS_ISLAND,
        GS_MENU,
        GS_SELECTING

    }


    public GameState currentGameState = GameState.GS_ISLAND;


    public void Awake()
    {
        instance = this;
    }

    
    void Start () {
	
        
        CameraChange();
    }
	
	
	void Update () {
        
        


    }


    


    public void CameraChange()
    {

        if (currentGameState == GameState.GS_ISLAND)

        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(true);
            worldCamera.enabled = true;
            seedCamera.enabled = false;
            selectCamera.enabled = false;
            seed.SetActive(false);
        }
        else if (currentGameState == GameState.GS_SEED)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            worldCamera.enabled = false;
            seedCamera.enabled = true;
            selectCamera.enabled = false;
            
        }
        else if (currentGameState == GameState.GS_SELECTING)
        {
            selectCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            worldCamera.enabled = false;
            seedCamera.enabled = false;
            selectCamera.enabled = true;
            seed.SetActive(false);
        }
    }
    public void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        CameraChange();

    }
    public void SetGameState(string newGameState)
    {
        if (newGameState == "island")
        {
            currentGameState = GameState.GS_ISLAND;
            
            
        }
        else if (newGameState == "seed")
        {
            currentGameState = GameState.GS_SEED;
            

        }
        else if (newGameState == "select")
        {
            currentGameState = GameState.GS_SELECTING;
            //selectingMode = true;
        }
        
        CameraChange();

    }
    public void ReturnIslendView()
    {

        currentGameState = GameState.GS_ISLAND;
        CameraChange();
    }

    
  

}
