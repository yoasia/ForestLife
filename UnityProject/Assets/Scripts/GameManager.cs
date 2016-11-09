using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Camera seedCamera;
    public Camera worldCamera;

    public GameObject leftMenuPanel;

    private Animator animatorLeftMenu;
    private bool leftMenuVisable = false;



    public static GameManager instance;

    public enum GameState
    {
        GS_SEED,
        GS_ISLAND,
        GS_MENU

    }


    public GameState currentGameState = GameState.GS_ISLAND;


    public void Awake()
    {
        instance = this;
    }

    
    void Start () {
	
        animatorLeftMenu = leftMenuPanel.GetComponent<Animator>();
        animatorLeftMenu.enabled = false;
    }
	
	
	void Update () {
        CameraChange();



    }


    public void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        

    }


    public void CameraChange()
    {

        if (currentGameState == GameState.GS_ISLAND)
        {
            worldCamera.enabled = true;
            seedCamera.enabled = false;
        }
        else if (currentGameState == GameState.GS_SEED)
        {
            worldCamera.enabled = false;
            seedCamera.enabled = true;
        }
    }

    public void LeftMenuOnOff()
    {
        if (leftMenuVisable) {
            leftMenuVisable = false;
            animatorLeftMenu.Play("LeftMenuSlideOut");
            //set back the time scale to normal time scale
            Time.timeScale = 1;
        }
        else
        {
            animatorLeftMenu.enabled = true;
            animatorLeftMenu.Play("LeftMenuSlideIn");
            leftMenuVisable = true;
            //freeze the timescale
            Time.timeScale = 0;
        }
        
    }
    
   


}
