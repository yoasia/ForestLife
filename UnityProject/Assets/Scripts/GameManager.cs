using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public Camera seedCamera;
    public Camera worldCamera;
    public Camera selectCamera;

    public GameObject seed;

    public GameObject leftMenuPanel;
	public bool selectingMode = false;

    private Animator animatorLeftMenu;
    private bool leftMenuVisable = false;
	public Button es;
	public Sprite s;
    public EventSystem eventSystem;
    public Text koraAddText, korzenAddText, liscieAddText;

    public List <GameObject> selectedTrees = new List<GameObject>();

    int addKora = 0, addKorzen = 0, addLiscie = 0;
    int upgradeAddValue = 10;
	Ray ray;
	RaycastHit hit;

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
	
        animatorLeftMenu = leftMenuPanel.GetComponent<Animator>();
        animatorLeftMenu.enabled = false;
        CameraChange();
    }
	
	
	void Update () {
        
        


    }


    


    public void CameraChange()
    {

        if (currentGameState == GameState.GS_ISLAND)
        {
            worldCamera.enabled = true;
            seedCamera.enabled = false;
            selectCamera.enabled = false;
            seed.SetActive(false);
        }
        else if (currentGameState == GameState.GS_SEED)
        {
            worldCamera.enabled = false;
            seedCamera.enabled = true;
            selectCamera.enabled = false;
            
        }
        else if (currentGameState == GameState.GS_SELECTING)
        {
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
            selectingMode = true;
        }
        
        CameraChange();

    }
    public void ReturnIslendView()
    {

        currentGameState = GameState.GS_ISLAND;
        CameraChange();
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
    
	
	



    public void setValuesToUpgradeTrees(Button b)
    {

        
        if(b.name == "AddKoraButton")
        {
            addKora += upgradeAddValue;
            koraAddText.text = "+" + addKora;
        }
        if(b.name == "AddLiscieButton")
        {
            addLiscie += upgradeAddValue;
            liscieAddText.text = "+" + addLiscie;
        }
        if (b.name == "AddKorzenButton")
        {
            addKorzen += upgradeAddValue;
           korzenAddText.text = "+" + addKorzen;
        }
        if (b.name == "SubKoraButton")
        {
            if (addKora - upgradeAddValue >= 0)
            {
                addKora -= upgradeAddValue;
                koraAddText.text = "+" + addKora;
            }
            
        }
        if (b.name == "SubLiscieButton")
        {
            if (addLiscie - upgradeAddValue >= 0)
            {
                addLiscie -= upgradeAddValue;
                liscieAddText.text = "+" + addLiscie;
            }
        }
        if (b.name == "SubKorzenButton")
        {
            if (addKorzen - upgradeAddValue >= 0)
            {
                addKorzen -= upgradeAddValue;
                korzenAddText.text = "+" + addKorzen;
            }
        }
        
            
                
    }

    public void DisableSubButtons(Button b)
    {
        if (b.name == "SubKoraButton")
        {
            if (addKora <= 0)
            {
                b.interactable = false;
            }
            else
            {
                b.interactable = true;
            }

        }
        if (b.name == "SubLiscieButton")
        {
            if (addLiscie <= 0)
            {
                b.interactable = false;
            }
            else
            {
                b.interactable = true;
            }
        }
        if (b.name == "SubKorzenButton")
        {
            if (addKorzen <= 0)
            {
                b.interactable = false;
            }
            else
            {
                b.interactable = true;
            }
        }
    } 
  

}
