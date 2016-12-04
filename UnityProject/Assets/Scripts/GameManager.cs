using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera seedCamera;
    public Camera worldCamera;
    public Camera selectCamera;

    public GameObject mainCanvas;
    public GameObject selectCanvas;
    public GameObject popupCanvas;
    public GameObject treeChooserCanvas;
    public GameObject startCanvas;

    public GameObject quizCanvas;

    public GameObject newSeedCanvas;
    public GameObject triviaCanvas;


    public GameObject seed;
    public float periodOfCreatingCloud = 180.0f;
    public float questionPeriod = 180.0f;
    public GameObject cloud;
    public GameObject emotionalQuestions;
    public GameObject seedPrefab;

    public Terrain terrain;
    public TerrainManager terrainManager;

    static public Vector2 Wind { private set; get; }
    public List<GameObject> treesSpecies;

    public float maxWind = 5F;
    public List<GameObject> treesOnIsland;

    public List<GameObject> selectedTrees = new List<GameObject>();

    public int maxTimeBetweenWindChange = 60;
    public int minTimeBetweenWindChange = 30;

    public int timeBetweenSeeds = 180;

    public float quizFactor = 1F;
    public float maxFactor = 1.5F;
    public float minFactor = 0.5F;

    private float timeToWindChange;
    private float transitionTimeLeft;
    private Vector2 windChangePerSecond;

    private Quaternion seedDefaultRotation;

    public float timeBetweenSavingData = 0.1F;
    private bool firstSave = true;
    private String behaviouralDataFile = "beh_data.csv";

    private float timeToNextDataSave = 0;
    private float timeToNextSeed;

    public float timeBetweenTrivia = 15F;
    public float lastTrivia = 0;
    public float timeBetweenQuiz = 20F;
    public float lastQuiz = 0;
    public int triviasBeforeQuiz = 2;

    public float score = 0;

    public enum GameState
    {
        GS_SEED,
        GS_ISLAND,
        GS_START_MENU,
        GS_SELECT_TREEKIND,
        GS_SELECTING,
        GS_QUIZ

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
        Wind = UnityEngine.Random.insideUnitCircle * maxWind;
        StartCoroutine(createCloudByTime(periodOfCreatingCloud));
        StartCoroutine(askAboutEmotions(questionPeriod));

        CameraChange();
    }

    void Update()
    {
        //załadowanie nowej ciekawostki
        if (currentGameState == GameState.GS_ISLAND && lastTrivia > timeBetweenTrivia)
        {
            if (JsonDataManager.instance.triviaLoaded)
            {
                triviaCanvas.GetComponent<TriviaListController>().LoadNewTrivia();
                MyNotifications.CallNotification("nowa ciekawostka", 3.0f);
                triviasBeforeQuiz--;
                lastTrivia = 0;
            }
        }
        lastTrivia += Time.deltaTime;

        //wywołanie nowego quizu
        if (currentGameState == GameState.GS_ISLAND && lastQuiz > timeBetweenQuiz)
        {

            if (!JsonDataManager.instance.allQuestionsDisplayed)
            {
                lastQuiz = 0;
                SetGameState(GameState.GS_QUIZ);
            }

        }
        if (triviasBeforeQuiz <= 0)
            lastQuiz += Time.deltaTime;

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

        if (currentGameState != GameState.GS_SEED && currentGameState != GameState.GS_QUIZ && currentGameState == GameState.GS_ISLAND)
        {
            timeToNextSeed -= Time.deltaTime;
            if (timeToNextSeed < 0)
            {
                NewSeedPopup();
            }
        }

        score = GetScore();
    }


    public void ResetSeed()
    {
        seed.GetComponent<SeedController>().Reset();
    }

    public void NewSeedPopup()
    {
        selectCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        popupCanvas.SetActive(false);
        treeChooserCanvas.SetActive(false);
        startCanvas.SetActive(false);
        mainCanvas.GetComponent<MainCanvasController>().DeselectAllTrees();
        newSeedCanvas.SetActive(true);

        worldCamera.enabled = true;

        selectCamera.enabled = false;
        currentGameState = GameState.GS_ISLAND;

        if (seed != null)
        {
            seedCamera.enabled = false;
            seed.SetActive(false);
        }
        //Time.timeScale = 0;
    }

    public void NewSeed(GameObject selectedTree)
    {
        timeToNextSeed = timeBetweenSeeds;
        var species = selectedTree.GetComponent<TreeController>().species;
        var position = selectedTree.transform.position;
        position.y += 15;
        seed = (GameObject)Instantiate(seedPrefab, position, seedDefaultRotation);
        seed.GetComponent<SeedController>().species = species;
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

        quizCanvas.SetActive(false);

        newSeedCanvas.SetActive(false);

        worldCamera.enabled = true;

        selectCamera.enabled = false;
        currentGameState = GameState.GS_ISLAND;

        if (seed != null)
        {
            seedCamera.enabled = false;
            seed.SetActive(false);
        }

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

        quizCanvas.SetActive(false);

        newSeedCanvas.SetActive(false);

        worldCamera.enabled = true;

        selectCamera.enabled = false;

        if (seed != null)
        {
            seedCamera.enabled = false;
            seed.SetActive(false);
        }

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

            quizCanvas.SetActive(false);

            newSeedCanvas.SetActive(false);

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

            quizCanvas.SetActive(false);

            seed.SetActive(true);

            worldCamera.enabled = false;
            newSeedCanvas.SetActive(false);

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

            quizCanvas.SetActive(false);

            newSeedCanvas.SetActive(false);

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

            quizCanvas.SetActive(false);

            newSeedCanvas.SetActive(false);

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

            quizCanvas.SetActive(false);

            worldCamera.enabled = true;
            seedCamera.enabled = false;
            selectCamera.enabled = false;
            seed.SetActive(false);
        }
        else if (currentGameState == GameState.GS_QUIZ)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(false);
            startCanvas.SetActive(false);
            quizCanvas.SetActive(true);

            newSeedCanvas.SetActive(false);

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
            MovableCamera.instance.canBeMove();
        }
        else if (newGameState == GameState.GS_SEED)
        {
            Time.timeScale = 1;
        }
        else if (newGameState == GameState.GS_SELECTING)
        {
            MovableCamera.instance.canBeMove();
            Time.timeScale = 1;
            //selectingMode = true;
        }
        else if (newGameState == GameState.GS_START_MENU)
        {
            Time.timeScale = 0;
            MovableCamera.instance.dontMove();
        }
        else if (newGameState == GameState.GS_SELECT_TREEKIND)
        {
            Time.timeScale = 0;
        }
        else if (newGameState == GameState.GS_QUIZ)
        {
            Time.timeScale = 0;
            MovableCamera.instance.dontMove();
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
        else if (newGameState.ToLower() == "quiz")
        {
            SetGameState(GameState.GS_QUIZ);
        }

        //CameraChange();
    }


    public void ReturnIslendView()
    {
        currentGameState = GameState.GS_ISLAND;

        CameraChange();
        Time.timeScale = 1;
    }

    IEnumerator createCloudByTime(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Instantiate(cloud);
        }
    }

    public void GoodQuizAnswer()
    {
        quizFactor += 0.1F;
        if (quizFactor > maxFactor)
            quizFactor = maxFactor;
    }

    public void BadQuizAnswer()
    {
        quizFactor -= 0.1F;
        if (quizFactor < minFactor)
            quizFactor = minFactor;
    }

    private void WindChange()
    {
        transitionTimeLeft = UnityEngine.Random.Range(5, 11);
        var newWind = UnityEngine.Random.insideUnitCircle * maxWind;
        var windChange = newWind - Wind;
        windChangePerSecond = windChange / transitionTimeLeft;
        timeToWindChange = UnityEngine.Random.Range(minTimeBetweenWindChange, maxTimeBetweenWindChange + 1);
    }

    IEnumerator askAboutEmotions(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);


            showEmotionsQuestion();
        }
    }

    public void showEmotionsQuestion()
    {
        GameObject cloneQuestionPanel = GameObject.FindWithTag("Clone");
        if (currentGameState == GameState.GS_ISLAND)
            if (cloneQuestionPanel == null)
                Instantiate(emotionalQuestions);
    }


    public bool saveEmotionalState()
    {
        string delimiter = ",";

        return true;
    }

    static public void addRowToFile(string filePath, string data)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(data);

        File.AppendAllText(filePath, sb.ToString());
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
        //Compass compass = Input.compass;
        Touch first_touch;
        bool is_first_touch = false;
        Touch second_touch;
        bool is_second_touch = false;

        String data_list = "";

        if (firstSave == true)
        {
            data_list += "orientation" + ",";
            data_list += "acceleration_x" + ",";
            data_list += "acceleration_y" + ",";
            data_list += "acceleration_z" + ",";
            //data_list+="compass_heading_accuracy"+",";
            //data_list+="compass_magnetic_heading"+",";
            //data_list+="compass_true_heading"+",";
            data_list += "touch_count" + ",";

            data_list += "first_touch_finger_id" + ",";
            data_list += "first_touch_delta_time" + ",";
            //data_list+="first_touch_type"+",";
            data_list += "first_touch_tap_count" + ",";
            data_list += "first_touch_phase" + ",";
            data_list += "first_touch_position_x" + ",";
            data_list += "first_touch_position_y" + ",";
            //data_list+="first_touch_delta_position_x"+",";
            //data_list+="first_touch_delta_position_y"+",";
            //data_list+="first_touch_radius"+",";

            data_list += "second_touch_finger_id" + ",";
            data_list += "second_touch_delta_time" + ",";
            //data_list+="second_touch_type"+",";
            data_list += "second_touch_tap_count" + ",";
            data_list += "second_touch_phase" + ",";
            data_list += "second_touch_position_x" + ",";
            data_list += "second_touch_position_y" + ",";
            //data_list+="second_touch_delta_position_x"+",";
            //data_list+="second_touch_delta_position_y"+",";
            //data_list+="second_touch_radius"+",";

            data_list += "current_game_state" + ",";
            data_list += "game_event" + ",";
            data_list += "score";

            addRowToFile(behaviouralDataFile, data_list);
            data_list = "";

            firstSave = false;
        }

        try
        {
            first_touch = Input.GetTouch(0);
            is_first_touch = true;
        }
        catch (Exception e)
        {
            is_first_touch = false;
        }

        try
        {
            second_touch = Input.GetTouch(1);
            is_second_touch = true;
        }
        catch (Exception e)
        {
            is_second_touch = false;
        }

        data_list += orientation.ToString() + ",";
        data_list += acceleration.x.ToString() + ",";
        data_list += acceleration.y.ToString() + ",";
        data_list += acceleration.z.ToString() + ",";
        //data_list+=compass.headingAccuracy.ToString()+",";
        //data_list+=compass.magneticHeading.ToString()+",";
        //data_list+=compass.trueHeading.ToString()+",";
        data_list += Input.touchCount.ToString() + ",";

        if (is_first_touch == true)
        {
            data_list += first_touch.fingerId.ToString() + ",";
            data_list += first_touch.deltaTime.ToString() + ",";
            //data_list+=first_touch.type.ToString()+",";
            data_list += first_touch.tapCount.ToString() + ",";
            data_list += first_touch.phase.ToString() + ",";
            data_list += first_touch.position.x.ToString() + ",";
            data_list += first_touch.position.y.ToString() + ",";
            //data_list+=first_touch.deltaPosition.x.ToString()+",";
            //data_list+=first_touch.deltaPosition.y.ToString()+",";
            //data_list+=first_touch.radius.ToString()+",";
        }
        else
        {
            data_list += "" + ",";
            data_list += "" + ",";
            //data_list+=""+",";
            data_list += "" + ",";
            data_list += "" + ",";
            data_list += "" + ",";
            data_list += "" + ",";
            //data_list+=""+",";
            //data_list+=""+",";
            //data_list+=""+",";
        }

        if (is_second_touch == true)
        {
            data_list += second_touch.fingerId.ToString() + ",";
            data_list += second_touch.deltaTime.ToString() + ",";
            //data_list+=second_touch.type.ToString()+",";
            data_list += second_touch.tapCount.ToString() + ",";
            data_list += second_touch.phase.ToString() + ",";
            data_list += second_touch.position.x.ToString() + ",";
            data_list += second_touch.position.y.ToString() + ",";
            //data_list+=second_touch.deltaPosition.x.ToString()+",";
            //data_list+=second_touch.deltaPosition.y.ToString()+",";
            //data_list+=second_touch.radius.ToString()+",";
        }
        else
        {
            data_list += "" + ",";
            data_list += "" + ",";
            //data_list+=""+",";
            data_list += "" + ",";
            data_list += "" + ",";
            data_list += "" + ",";
            data_list += "" + ",";
            //data_list+=""+",";
            //data_list+=""+",";
            //data_list+=""+",";
        }

        data_list += currentGameState.ToString() + ",";
        data_list += game_event + ",";
        data_list += score.ToString("F2");

        addRowToFile(behaviouralDataFile, data_list);
    }	


    private float GetScore()
    {
        float newScore = 0;
        for (int i = 0; i < treesOnIsland.Count; i++)
        {
            var tree = treesOnIsland[i].GetComponent<TreeController>();
            if (tree.isAlive)
                newScore += tree.healthPoints * tree.size;
        }
        return newScore;
    }
}
