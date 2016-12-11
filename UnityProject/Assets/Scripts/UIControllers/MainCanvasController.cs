using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{


    public Camera cam;
    public GameObject leftMenuPanel;
    public GameObject rightMenuPanel;
    public List<GameObject> selectedTrees = new List<GameObject>();
    public Slider koraSlider, liscieSlider, korzenSlider;
    public Text livePointsText;
    public GameObject selectionParticle;
    public GameObject optionsPanel;
    public Text treeAmountText;
    public Text timeText;
    public Image weatherImage;
    public Image windImage;
    public Image treeHealthImage;
    public Sprite rainSprite, sunSprite, claudSprite;
    public Sprite veryGoodHealthSprite, goodHealthSprite, neutralHealthSprite, badHealthSprite, veryBadHealthSprite;

    public Button SelectionModeButton, LeftMenuButton;

    public Text QualityText;

    public GameObject activeTree;

    public CanvasGroup triviaPanel;



    private Animator animatorLeftMenu;
    private bool leftMenuVisable = false;

    private Animator animatorRightMenu;

    Ray ray;
    RaycastHit hit;
    public bool IsScreenPopup = false;
    

    void Start()
    {

        animatorLeftMenu = leftMenuPanel.GetComponent<Animator>();
        animatorLeftMenu.enabled = false;
        animatorRightMenu = rightMenuPanel.GetComponent<Animator>();
        animatorRightMenu.enabled = false;
        activeTree = null;
    }


    void Update()
    {

        if (GameManager.instance.currentGameState == GameManager.GameState.GS_ISLAND)
        {
            if (!IsScreenPopup)
            {
                SetTreeAmountText(GameManager.instance.treesOnIsland.Count);
                int t = (int)Time.time;
                SetDateText(t.ToString());
                PointSelect();
                 foreach(Touch touch in Input.touches){
                 if(touch.tapCount == 2)
                  DeselectAllTrees();
                 }
                if (selectedTrees.Count != 0)
                {
                    UpdateActiveTreeInfo();
                    RightMenuOnOff(true);
                }
                else
                {
                    RightMenuOnOff(false);
                }
            }


        }
    }


    private void PointSelect()
    {

        if (Input.GetMouseButtonDown(0))
        {

            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.transform.gameObject.tag == "Tree")
                {

                    if (selectedTrees.Contains(hit.transform.gameObject) == false)
                    {
                        if (activeTree != null)
                        {
                            
                            activeTree.GetComponent<TreeController>().SelectTree("green");
                        }

                        activeTree = hit.transform.gameObject;
                        activeTree.GetComponent<TreeController>().SelectTree("normal");
                        selectedTrees.Add(hit.transform.gameObject);
                    }
                    else
                    {
                        selectedTrees.Remove(hit.transform.gameObject);
                        if (hit.transform.gameObject == activeTree)
                        {
                            if (selectedTrees.Count != 0)
                            {
                                activeTree = selectedTrees[0];
                                activeTree.GetComponent<TreeController>().SelectTree("normal");
                            }
                            else
                            {
                                activeTree = null;

                            }
                        }
                        hit.transform.gameObject.GetComponent<TreeController>().UnselectTree();
                    }

                }



            }



        }

    }



    public void LeftMenuOnOff()
    {
        if (leftMenuVisable)
        {
            leftMenuVisable = false;
            animatorLeftMenu.Play("LeftMenuSlideOut");
            Time.timeScale = 1;
            SelectionModeButton.gameObject.SetActive(true);
            LeftMenuButton.gameObject.SetActive(true);
        }
        else
        {
            animatorLeftMenu.enabled = true;
            animatorLeftMenu.Play("LeftMenuSlideIn");
            leftMenuVisable = true;
            SelectionModeButton.gameObject.SetActive(false);
            LeftMenuButton.gameObject.SetActive(false);
            Time.timeScale = 0;
        }

    }

    public void UpdateActiveTreeInfo()
    {


        koraSlider.value = activeTree.GetComponent<TreeController>().barkStrength;
        liscieSlider.value = activeTree.GetComponent<TreeController>().leavesStrength;
        korzenSlider.value = activeTree.GetComponent<TreeController>().rootsStrength;
        int p = (int)activeTree.GetComponent<TreeController>().upgradePoints;
        livePointsText.text = p.ToString();
        float health = activeTree.GetComponent<TreeController>().healthPoints;
        if (health > 80)
        {
            treeHealthImage.sprite = veryGoodHealthSprite;
        }
        else if (health <= 80 && health > 60)
        {
            treeHealthImage.sprite = goodHealthSprite;
        }
        else if (health <= 60 && health > 40)
        {
            treeHealthImage.sprite = neutralHealthSprite;
        }
        else if (health <= 40 && health > 20)
        {
            treeHealthImage.sprite = badHealthSprite;
        }
        else
        {
            treeHealthImage.sprite = veryBadHealthSprite;
        }
    }

    public void ChangeActive(Button directionButton)
    {
        int index = selectedTrees.FindIndex(m => m.GetInstanceID() == activeTree.GetInstanceID());
        if (directionButton.name == "LeftButton")
        {
            if (index > 0)
            {
                activeTree.GetComponent<TreeController>().SelectTree("green");
                activeTree = selectedTrees[index - 1];
                activeTree.GetComponent<TreeController>().SelectTree();
                
            }
            else
            {
                activeTree.GetComponent<TreeController>().SelectTree("green");
                activeTree = selectedTrees[selectedTrees.Count - 1];
                activeTree.GetComponent<TreeController>().SelectTree();
            }
        }
        else
        {
            if (index < selectedTrees.Count - 1)
            {
                activeTree.GetComponent<TreeController>().SelectTree("green");
                activeTree = selectedTrees[index + 1];
                activeTree.GetComponent<TreeController>().SelectTree();
            }
            else
            {
                activeTree.GetComponent<TreeController>().SelectTree("green");
                activeTree = selectedTrees[0];
                activeTree.GetComponent<TreeController>().SelectTree();
            }
        }
    }

    public void RightMenuOnOff(bool on)
    {
        if (on)
        {

            animatorRightMenu.enabled = true;
            animatorRightMenu.Play("RightMenuSlideIn");



        }
        else
        {
            animatorRightMenu.Play("RightMenuSlideOut");


        }

    }


    public void SetDateText(string date)
    {
        timeText.text = date;
    }

    public void SetTreeAmountText(int treeAmount)
    {
        treeAmountText.text = treeAmount.ToString();
    }

    public void SetWeatherIcon(string weather)
    {
        switch (weather)
        {
            case "sun":
                weatherImage.sprite = sunSprite;
                break;
            case "rain":
                weatherImage.sprite = rainSprite;
                break;
            case "claud":
                weatherImage.sprite = claudSprite;
                break;
            default:
                weatherImage.sprite = claudSprite;
                break;

        }

    }

    public void SetWindDirection(char direction)
    {

        switch (direction)
        {
            case 's':
                windImage.transform.localEulerAngles = new Vector3(0, 0, 180);
                break;
            case 'e':
                windImage.transform.localEulerAngles = new Vector3(0, 0, -90);
                break;
            case 'w':
                windImage.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
            default:
                windImage.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;

        }
    }

    public void DeselectAllTrees()
    {
        foreach (GameObject t in selectedTrees)
        {
            t.GetComponent<TreeController>().UnselectTree();
        }
        activeTree = null;
        selectedTrees.Clear();
    }

    public void ShowTriviaPanel()
    {

        triviaPanel.alpha = 1;
        triviaPanel.interactable = true;
        triviaPanel.blocksRaycasts = true;
        if (leftMenuVisable)
        {
            leftMenuVisable = false;
            animatorLeftMenu.Play("LeftMenuSlideOut");
        }
        Time.timeScale = 0;
        SelectionModeButton.gameObject.SetActive(false);
        LeftMenuButton.gameObject.SetActive(false);
        IsScreenPopup = true;
    }

    public void HideTriviaPanel()
    {
        Time.timeScale = 1;
        triviaPanel.alpha = 0;
        triviaPanel.interactable = false;
        triviaPanel.blocksRaycasts = false;
        SelectionModeButton.gameObject.SetActive(true);
        LeftMenuButton.gameObject.SetActive(true);
        IsScreenPopup = false;
    }

    public void ChangeQuality(Button b)
    {
        if (b.name == "plus")
        {
            QualitySettings.IncreaseLevel();

        }
        else
        {
            QualitySettings.DecreaseLevel();
        }

        QualityText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void ShowOptionsPanel()
    {
        Time.timeScale = 0;
        if (leftMenuVisable)
        {
            leftMenuVisable = false;
            animatorLeftMenu.Play("LeftMenuSlideOut");
        }
        SelectionModeButton.gameObject.SetActive(false);
        LeftMenuButton.gameObject.SetActive(false);
        IsScreenPopup = true;
        optionsPanel.SetActive(true);
        QualityText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];

        
    }
    public void HideOptionsPanel()
    {
        optionsPanel.SetActive(false);
        Time.timeScale = 1;
        SelectionModeButton.gameObject.SetActive(true);
        LeftMenuButton.gameObject.SetActive(true);
        IsScreenPopup = false;
    }
}
