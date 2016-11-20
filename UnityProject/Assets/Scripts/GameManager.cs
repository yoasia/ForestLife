using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public Camera seedCamera;
    public Camera worldCamera;
    public Camera selectCamera;

    public GameObject mainCanvas;
    public GameObject selectCanvas;
    public GameObject popupCanvas;
    public GameObject treeChooserCanvas;
    public GameObject startCanvas;

    public GameObject seed;


    public Terrain terrain;
    public List<TreeController> treesSpecies;

    public List <GameObject> selectedTrees = new List<GameObject>();


    

    public static GameManager instance;

    public enum GameState
    {
        GS_SEED,
        GS_ISLAND,
        GS_START_MENU,
        GS_SELECT_TREEKIND,
        GS_SELECTING

    }


    public GameState currentGameState = GameState.GS_ISLAND;


    public void Awake()
    {
        instance = this;
    }

    
    void Start () {

        currentGameState = GameState.GS_SEED;

        animatorLeftMenu = leftMenuPanel.GetComponent<Animator>();
        animatorLeftMenu.enabled = false;

        CameraChange();
    }
	
	
	void Update () {
        
        


    }


    public bool seedLanding(float x, float z, string type)
    {
        var treeToAdd = treesSpecies.First(s=>s.species.ToLower() == type.ToLower());
        var pos = new Vector3(x, 0, z);
        pos.y = terrain.SampleHeight(pos);

        Instantiate(treeToAdd, pos, new Quaternion(0, 0, 0, 0));
        SetGameState(GameState.GS_ISLAND);

        return true;
    }


    public void CameraChange()
    {

        if (currentGameState == GameState.GS_ISLAND)

        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(true);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(false);

            worldCamera.enabled = true;
            seedCamera.enabled = false;
            selectCamera.enabled = false;
            seed.SetActive(false);
        }
        else if (currentGameState == GameState.GS_SEED)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(true);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(false);

            worldCamera.enabled = false;
            seedCamera.enabled = true;
            selectCamera.enabled = false;
            
        }
        else if (currentGameState == GameState.GS_SELECTING)
        {
            selectCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(false);

            worldCamera.enabled = false;
            seedCamera.enabled = false;
            selectCamera.enabled = true;
            seed.SetActive(false);
        }
        else if (currentGameState == GameState.GS_START_MENU)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(true);

            worldCamera.enabled = false;
            seedCamera.enabled = true;
            selectCamera.enabled = false;

        }
        else if (currentGameState == GameState.GS_SELECT_TREEKIND)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(true);
            startCanvas.SetActive(false);

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
