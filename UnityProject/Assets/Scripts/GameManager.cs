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

    public List<Object> treesOnIsland;

    public List <GameObject> selectedTrees = new List<GameObject>();

    public float maxWind = 5F;

    public int maxTimeBetweenWindChange = 60;
    public int minTimeBetweenWindChange = 30;
    public Vector2 Wind { private set; get; }

    private float timeToWindChange;
    private float transitionTimeLeft;
    private Vector2 windChangePerSecond;

    public static GameManager instance;

    public TerrainManager terrainManager;

    public enum GameState
    {
        GS_SEED,
        GS_ISLAND,
        GS_START_MENU,
        GS_SELECT_TREEKIND,
        GS_SELECTING

    }


    public GameState currentGameState = GameState.GS_START_MENU;


    public void Awake()
    {
        instance = this;
    }

    
    void Start () {

        Wind = Random.insideUnitCircle * maxWind;
        timeToWindChange = Random.Range(minTimeBetweenWindChange, maxTimeBetweenWindChange + 1);

        terrainManager = terrain.GetComponent<TerrainManager>();

        Time.timeScale = 0;
        if(currentGameState == GameState.GS_SEED)
            Time.timeScale = 1;
        //currentGameState = GameState.GS_SEED;



        CameraChange();
    }
	
	
	void Update () {

        if(transitionTimeLeft > 0)
        {
            transitionTimeLeft -= Time.deltaTime;
            Wind += windChangePerSecond * Time.deltaTime;
        }

        timeToWindChange -= Time.deltaTime;
        if (timeToWindChange < 0)
            WindChange();

        //Debug.LogFormat("Wind: {0}", Wind);
    }


    public bool seedLanding(float x, float z, string type)
    {
        if (terrainManager.CanGrow(x, z))
        {
            var treeToAdd = treesSpecies.First(s => s.species.ToLower() == type.ToLower());
            var pos = new Vector3(x, 0, z);
            pos.y = terrain.SampleHeight(pos);

            var tree = Instantiate(treeToAdd, pos, new Quaternion(0, 0, 0, 0));
            treesOnIsland.Add(tree);

            OnGoodLandingPopup();
        }
        else
        {
            OnBadLandingPopup();
        }

        return true;
    }


    public void OnGoodLandingPopup()
    {
        selectCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        popupCanvas.SetActive(true);
        treeChooserCanvas.SetActive(false);
        startCanvas.SetActive(false);

        worldCamera.enabled = true;
        seedCamera.enabled = false;
        selectCamera.enabled = false;
        currentGameState = GameState.GS_ISLAND;
        seed.SetActive(false);

        popupCanvas.GetComponent<PopupController>().BadLandingPopupOff();
        popupCanvas.GetComponent<PopupController>().GoodLandingPopupOn();
    }

    public void OnBadLandingPopup()
    {
        selectCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        popupCanvas.SetActive(true);
        treeChooserCanvas.SetActive(false);
        startCanvas.SetActive(false);

        worldCamera.enabled = true;
        seedCamera.enabled = false;
        selectCamera.enabled = false;
        seed.SetActive(false);

        popupCanvas.GetComponent<PopupController>().GoodLandingPopupOff();
        popupCanvas.GetComponent<PopupController>().BadLandingPopupOn();
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
            selectCamera.enabled = false;
            if (seed != null)
            {
                seedCamera.enabled = false;
                seed.SetActive(false);
            }
            Time.timeScale = 1;
        }
        else if (currentGameState == GameState.GS_SEED)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(true);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(false);
            worldCamera.enabled = false;
            if (seed != null)
            {
                seed.SetActive(true);
                seedCamera.enabled = true;
            }
            selectCamera.enabled = false;
            Time.timeScale = 1;
            
        }
        
        else if (currentGameState == GameState.GS_SELECTING)
        {
            selectCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(false);

            worldCamera.enabled = false;
            if (seed != null)
            {
                seed.SetActive(false);
                seedCamera.enabled = false;
            }
            selectCamera.enabled = true;
            Time.timeScale = 1;
        }
        else if (currentGameState == GameState.GS_START_MENU)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(true);

            worldCamera.enabled = true;
            if (seed != null)
            {
                seed.SetActive(false);
                seedCamera.enabled = false;
            }
            selectCamera.enabled = false;
            Time.timeScale = 0;
        }
        else if (currentGameState == GameState.GS_SELECT_TREEKIND)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(true);
            startCanvas.SetActive(false);

            worldCamera.enabled = true;
            selectCamera.enabled = false;
            if (seed != null)
            {
                seedCamera.enabled = false;
                seed.SetActive(false);
            }
            Time.timeScale = 0;
        }
        //CameraChange();
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
            Time.timeScale = 1;
            
        }
        else if (newGameState == "seed")
        {
            currentGameState = GameState.GS_SEED;
            Time.timeScale = 1;

        }
        else if (newGameState == "select")
        {
            currentGameState = GameState.GS_SELECTING;
            Time.timeScale = 1;
            //selectingMode = true;
        }
        else if (newGameState == "startMenu")
        {
            currentGameState = GameState.GS_START_MENU;
            Time.timeScale = 0;

        }
        else if (newGameState == "selectTree")
        {
            currentGameState = GameState.GS_SELECT_TREEKIND;
            Time.timeScale = 0;
        }
        
        
        CameraChange();

    }
    public void ReturnIslendView()
    {

        currentGameState = GameState.GS_ISLAND;

        CameraChange();
        Time.timeScale = 1;
    }

    private void WindChange()
    {
        transitionTimeLeft = Random.Range(5, 11);
        var newWind = Random.insideUnitCircle * maxWind;
        var windChange = newWind - Wind;
        windChangePerSecond = windChange / transitionTimeLeft;
        timeToWindChange = Random.Range(minTimeBetweenWindChange, maxTimeBetweenWindChange + 1);
    }

}
