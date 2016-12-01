using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    public float periodOfCreatingCloud = 180.0f;
    public float questionPeriod = 180.0f;
    public GameObject cloud;
    public GameObject emotionalQuestions;

    public Terrain terrain;
    public List<TreeController> treesSpecies;

    public List <GameObject> selectedTrees = new List<GameObject>();
    static public Vector2 Wind { private set; get; }

    public float maxWind = 5F;

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

        //currentGameState = GameState.GS_SEED;
        Wind = UnityEngine.Random.insideUnitCircle * maxWind;
        StartCoroutine(createCloudByTime(periodOfCreatingCloud));
        StartCoroutine(askAboutEmotions(questionPeriod));


        //CameraChange();
    }


    void Update () {

        CameraChange();


    }


    public bool seedLanding(float x, float z, string type)
    {
        var treeToAdd = treesSpecies.First(s=>s.species.ToLower() == type.ToLower());
        var pos = new Vector3(x, 0, z);
        pos.y = terrain.SampleHeight(pos);

        Instantiate(treeToAdd, pos, new Quaternion(0, 0, 0, 0));
        //SetGameState(GameState.GS_ISLAND);
        GoodLandingPopup();

        return true;
    }


    public void GoodLandingPopup()
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

        popupCanvas.GetComponent<PopupController>().BadLandingPopupOff();
        popupCanvas.GetComponent<PopupController>().GoodLandingPopupOn();
    }

    public void BadLandingPopup()
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
            seed.SetActive(true);
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

            worldCamera.enabled = true;
            seedCamera.enabled = false;
            selectCamera.enabled = false;
            seed.SetActive(false);
        }
        else if (currentGameState == GameState.GS_SELECT_TREEKIND)
        {
            selectCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            popupCanvas.SetActive(false);
            treeChooserCanvas.SetActive(true);
            startCanvas.SetActive(false);

            worldCamera.enabled = true;
            seedCamera.enabled = false;
            selectCamera.enabled = false;
            seed.SetActive(false);
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
        else if (newGameState == "startMenu")
        {
            currentGameState = GameState.GS_START_MENU;
        
        }
        else if (newGameState == "selectTree")
        {
            currentGameState = GameState.GS_SELECT_TREEKIND;
            
        }
        
        
        CameraChange();

    }

    public void ReturnIslendView()
    {

        currentGameState = GameState.GS_ISLAND;
        CameraChange();
    }

    IEnumerator createCloudByTime(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            Instantiate(cloud);    
        }
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
        if(cloneQuestionPanel == null)
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



}
