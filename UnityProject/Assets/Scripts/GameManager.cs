using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public Camera seedCamera;
    public Camera worldCamera;
    public Camera selectCamera;

    public GameObject mainCanvas;
    public GameObject selectCanvas;
    public GameObject popupCanvas;
    public GameObject treeChooserCanvas;
    public GameObject startCanvas;

    public GameObject seed;
    public GameObject seedPrefab;

    public Terrain terrain;
    public TerrainManager terrainManager;

    public List<GameObject> treesSpecies;

    public List<GameObject> treesOnIsland;

    public List<GameObject> selectedTrees = new List<GameObject>();

    public float maxWind = 5F;

    public int maxTimeBetweenWindChange = 60;
    public int minTimeBetweenWindChange = 30;
    public Vector2 Wind { private set; get; }

    public int timeBetweenSeeds = 180;

    private float timeToWindChange;
    private float transitionTimeLeft;
    private Vector2 windChangePerSecond;

    private Quaternion seedDefaultRotation;

    public float timeBetweenSavingData = 0.1F;

    private float timeToNextDataSave = 0;
    private float timeToNextSeed;

    public static GameManager instance;

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
    
    void Start()
    {
        seedDefaultRotation = seed.transform.rotation;

        timeToNextSeed = timeBetweenSeeds;

        Wind = UnityEngine.Random.insideUnitCircle * maxWind;
        timeToWindChange = UnityEngine.Random.Range(minTimeBetweenWindChange, maxTimeBetweenWindChange + 1);

        terrainManager = terrain.GetComponent<TerrainManager>();

        Time.timeScale = 0;
        if (currentGameState == GameState.GS_SEED)
            Time.timeScale = 1;
        //currentGameState = GameState.GS_SEED;

        CameraChange();
    }
    
    void Update()
    {
        if (transitionTimeLeft > 0)
        {
            transitionTimeLeft -= Time.deltaTime;
            Wind += windChangePerSecond * Time.deltaTime;
        }

        timeToWindChange -= Time.deltaTime;

        if (timeToWindChange < 0)
            WindChange();

        timeToNextDataSave -= Time.deltaTime;

        if (timeToNextDataSave < 0)
        {
            BehaviouralData("");
            timeToNextDataSave = timeBetweenSavingData;
        }

        if (currentGameState != GameState.GS_SEED)
        {
            timeToNextSeed -= Time.deltaTime;
            if (timeToNextSeed < 0)
                NewSeedPopup();
        }
        //Debug.LogFormat("Wind: {0}", Wind);
    }


    public void ResetSeed()
    {
        seed.GetComponent<SeedController>().Reset();
    }

    private void NewSeedPopup()
    {
        //Do implementacji - wybieranie drzewa znad którego start nowego nasiona i wywołanie metody NewSeed przekazując wybrane drzewo jako parametr
        //NewSeed(treesOnIsland.First());
    }

    public void NewSeed(GameObject selectedTree)
    {
        timeToNextSeed = timeBetweenSeeds;
        var species = selectedTree.GetComponent<TreeController>().species;
        var position = selectedTree.transform.position;
        position.y += 15;
        seed = (GameObject)Instantiate(seedPrefab, position, seedDefaultRotation);
        seedCamera = seed.GetComponentInChildren<Camera>();
        SetGameState(GameState.GS_SEED);
        BehaviouralData("Start of new seed stage");
        //Debug.LogFormat(species);
    }

    public bool seedLanding(float x, float z, string type, bool automatic = false)
    {
        if (terrainManager.CanGrow(x, z))
        {
            var treeToAdd = treesSpecies.First(s => s.GetComponent<TreeController>().species.ToLower() == type.ToLower());
            var pos = new Vector3(x, 0, z);
            pos.y = terrain.SampleHeight(pos);

            var rotation = UnityEngine.Random.Range(0, 360);
            var tree = (GameObject)Instantiate(treeToAdd, pos, Quaternion.Euler(0, rotation, 0));
            treesOnIsland.Add(tree);
            
            if (automatic == false)
            {
                timeToNextSeed = timeBetweenSeeds;
                BehaviouralData("Good seed landing");
                OnGoodLandingPopup();
            }
        }
        else
        {
            if (automatic == false)
            {
                OnBadLandingPopup();
                BehaviouralData("Bad seed landing");
            }
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
        }
    }

    public void SetGameState(GameState newGameState)
    {
        popupCanvas.GetComponent<PopupController>().BadLandingPopupOff();
        popupCanvas.GetComponent<PopupController>().GoodLandingPopupOff();
        currentGameState = newGameState;

        if (newGameState == GameState.GS_ISLAND)
        {
            Time.timeScale = 1;
        }
        else if (newGameState == GameState.GS_SEED)
        {
            Time.timeScale = 1;
        }
        else if (newGameState == GameState.GS_SELECTING)
        {
            Time.timeScale = 1;
            //selectingMode = true;
        }
        else if (newGameState == GameState.GS_START_MENU)
        {
            Time.timeScale = 0;
        }
        else if (newGameState == GameState.GS_SELECT_TREEKIND)
        {
            Time.timeScale = 0;
        }

        CameraChange();
    }

    public void SetGameState(string newGameState)
    {
        if (newGameState.ToLower() == "island")
        {
            SetGameState(GameState.GS_ISLAND);
        }
        else if (newGameState.ToLower() == "seed")
        {
            SetGameState(GameState.GS_SEED);
        }
        else if (newGameState.ToLower() == "select")
        {
            SetGameState(GameState.GS_SELECTING);
            //selectingMode = true;
        }
        else if (newGameState.ToLower() == "startmenu")
        {
            SetGameState(GameState.GS_START_MENU);
        }
        else if (newGameState.ToLower() == "selecttree")
        {
            SetGameState(GameState.GS_SELECT_TREEKIND);
        }

        CameraChange();
    }

    public void ReturnIslandView()
    {
        currentGameState = GameState.GS_ISLAND;

        CameraChange();
        Time.timeScale = 1;
    }

    private void WindChange()
    {
        transitionTimeLeft = UnityEngine.Random.Range(5, 11);
        var newWind = UnityEngine.Random.insideUnitCircle * maxWind;
        var windChange = newWind - Wind;
        windChangePerSecond = windChange / transitionTimeLeft;
        timeToWindChange = UnityEngine.Random.Range(minTimeBetweenWindChange, maxTimeBetweenWindChange + 1);
    }


    public float TreeDistance(float x, float z)
    {
        float min_dist = Pythagoras(terrain.terrainData.size.x, terrain.terrainData.size.z) + 1;

        foreach (GameObject tree in treesOnIsland)
        {
            float dist = Pythagoras((x - tree.transform.position.x), (z - tree.transform.position.z));

            if (dist < min_dist)
            {
                min_dist = dist;
            }
        }

        return min_dist;
    }

    float Pythagoras(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }


    void BehaviouralData(String game_event)
    {
        DeviceOrientation orientation = Input.deviceOrientation;
        Vector3 acceleration = Input.acceleration;
        Compass compass = Input.compass;
        Touch first_touch = Input.GetTouch(0);
        Touch second_touch = Input.GetTouch(1);

        List<String> data_list = new List<String>();

        data_list.Add(orientation.ToString());
        data_list.Add(acceleration.x.ToString());
        data_list.Add(acceleration.y.ToString());
        data_list.Add(acceleration.z.ToString());
        data_list.Add(compass.headingAccuracy.ToString());
        data_list.Add(compass.magneticHeading.ToString());
        data_list.Add(compass.trueHeading.ToString());
        data_list.Add(Input.touchCount.ToString());
        data_list.Add(first_touch.fingerId.ToString());
        data_list.Add(first_touch.deltaTime.ToString());
        data_list.Add(first_touch.type.ToString());
        data_list.Add(first_touch.tapCount.ToString());
        data_list.Add(first_touch.phase.ToString());
        data_list.Add(first_touch.position.x.ToString());
        data_list.Add(first_touch.position.y.ToString());
        data_list.Add(first_touch.deltaPosition.x.ToString());
        data_list.Add(first_touch.deltaPosition.y.ToString());
        data_list.Add(first_touch.radius.ToString());
        data_list.Add(second_touch.fingerId.ToString());
        data_list.Add(second_touch.deltaTime.ToString());
        data_list.Add(second_touch.type.ToString());
        data_list.Add(second_touch.tapCount.ToString());
        data_list.Add(second_touch.phase.ToString());
        data_list.Add(second_touch.position.x.ToString());
        data_list.Add(second_touch.position.y.ToString());
        data_list.Add(second_touch.deltaPosition.x.ToString());
        data_list.Add(second_touch.deltaPosition.y.ToString());
        data_list.Add(second_touch.radius.ToString());
        data_list.Add(currentGameState.ToString());
        data_list.Add(game_event);
    }
}
