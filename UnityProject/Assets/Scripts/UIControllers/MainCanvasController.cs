﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour {


    public Camera cam;
    public GameObject leftMenuPanel;
    public GameObject rightMenuPanel;
    public List<GameObject> selectedTrees = new List<GameObject>();
    public Slider koraSlider, liscieSlider, korzenSlider;
    public Text livePointsText;
    public GameObject selectionParticle;

    public Text treeAmountText;
    public Text timeText;
    public Image weatherImage;
    public Image windImage;
    public Sprite rainSprite, sunSprite, claudSprite;

    public Button SelectionModeButton, LeftMenuButton;


    public GameObject activeTree;

    private Animator animatorLeftMenu;
    private bool leftMenuVisable = false;

    private Animator animatorRightMenu;
    private bool rightMenuVisable = false;
    Ray ray;
    RaycastHit hit;

	
	void Start () {

        animatorLeftMenu = leftMenuPanel.GetComponent<Animator>();
        animatorLeftMenu.enabled = false;
        animatorRightMenu = rightMenuPanel.GetComponent<Animator>();
        animatorRightMenu.enabled = false;
        activeTree = null;
	}
	
	
	void Update () {

        if (GameManager.instance.currentGameState == GameManager.GameState.GS_ISLAND)
        {
            PointSelect();
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
                            activeTree.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                        }
                        hit.transform.gameObject.GetComponent<TreeController>().SelectTree();
                        activeTree = hit.transform.gameObject;
                        
                        activeTree.GetComponent<Renderer>().material.shader = Shader.Find("markBlue");
                        
                        
                        
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
                                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("markBlue");
                            }
                            else
                            {
                                activeTree = null;
                                
                            }
                        }
                        hit.transform.gameObject.GetComponent<TreeController>().DeselectTree();
                        
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
        
        
        koraSlider.value = activeTree.GetComponent<TreeController>().kora;
        liscieSlider.value = activeTree.GetComponent<TreeController>().liscie;
        korzenSlider.value = activeTree.GetComponent<TreeController>().korzen;
        livePointsText.text = activeTree.GetComponent<TreeController>().lp.ToString();
    }

    public void ChangeActive(Button directionButton)
    {
        int index = selectedTrees.FindIndex(m => m.GetInstanceID() == activeTree.GetInstanceID());
        if (directionButton.name == "LeftButton")
        {
            if (index > 0)
            {
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                activeTree = selectedTrees[index - 1];
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("markBlue");
            }
            else
            {
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                activeTree = selectedTrees[selectedTrees.Count - 1];
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("markBlue");
            }
        }
        else
        {
            if (index < selectedTrees.Count - 1)
            {
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                activeTree = selectedTrees[index + 1];
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("markBlue");
            }
            else
            {
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                activeTree = selectedTrees[0];
                activeTree.GetComponent<Renderer>().material.shader = Shader.Find("markBlue");
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
                //windImage.transform.Rotate(0, 0, 90);
                break;
            case 'w':
                windImage.transform.localEulerAngles = new Vector3(0, 0, 90);
                //windImage.transform.Rotate(0, 0, -90);
                break;
            default:
                windImage.transform.localEulerAngles = new Vector3(0, 0, 0);
                //windImage.transform.Rotate(0, 0, 0);
                break;

        }
    }



}