using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SelectCanvasController : MonoBehaviour {

    
    
    public Camera cam;
    public GameObject rightMenuPanel;
    public bool selectingMode = true;

    public Button selectButton, deselectButton;
    public Button acceptSelButton, returnButton;
    public Button subKora, subKorzen, subLiscie;
    
    public EventSystem eventSystem;
    public Text koraAddText, korzenAddText, liscieAddText;

    public List<GameObject> selectedTrees = new List<GameObject>();


    int addBark = 0, addRoots = 0, addLeaves = 0;
    int upgradeAddValue = 10;
    Ray ray;
    RaycastHit hit;
    private Animator animatorRightMenu;
    private bool rightMenuVisable = false;

	
	void Start () {
        animatorRightMenu = rightMenuPanel.GetComponent<Animator>();
        animatorRightMenu.enabled = false;
        SelectModeOn();
	}
	
	
	void Update () {
        if (GameManager.instance.currentGameState == GameManager.GameState.GS_SELECTING)
        {
            PointSelect();
        }
	}




    public void RightMenuOnOff()
    {
        foreach (GameObject t in selectedTrees)
        {

            t.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse"); ;
        }
        ClearValues();
        DisableSubButtons();
        if (rightMenuVisable)
        {
            rightMenuVisable = false;
            animatorRightMenu.Play("ChangeTreesPamMenuOut");
            //selectButton.gameObject.SetActive(true);
            //deselectButton.gameObject.SetActive(true);
            acceptSelButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(true);
            //Time.timeScale = 1;
        }
        else
        {
            animatorRightMenu.enabled = true;
            animatorRightMenu.Play("ChangeTreesPamMenuIn");
            rightMenuVisable = true;
            //selectButton.gameObject.SetActive(false);
            //deselectButton.gameObject.SetActive(false);
            acceptSelButton.gameObject.SetActive(false);
            returnButton.gameObject.SetActive(false);
            //Time.timeScale = 0;
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

                    if (selectingMode)
                    {
                        if (selectedTrees.Contains(hit.transform.gameObject) == false)
                        {
                            hit.transform.gameObject.GetComponent<TreeController>().SelectTree();
                            selectedTrees.Add(hit.transform.gameObject);
                        }

                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<TreeController>().DeselectTree();
                        selectedTrees.Remove(hit.transform.gameObject);
                    }
                }



            }

        }

    }

    public void SelectModeOn()
    {
        selectingMode = true;
        Color c = new Color(0.98f, 1, 0.13f, 1);

        selectButton.image.color = c;
        deselectButton.image.color = Color.white;
        

    }

    public void SelectModeOff()
    {
        selectingMode = false;
        Color c = new Color(0.98f, 1, 0.13f, 1);
        deselectButton.image.color = c;
        selectButton.image.color = Color.white;
        

    }




    public void setValuesToUpgradeTrees(Button b)
    {
        foreach (GameObject t in selectedTrees)
        {
            
            t.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse"); ;
        }
        
        if (b.name == "AddKoraButton")
        {
            addBark += upgradeAddValue;
            koraAddText.text = "+" + addBark;
        }
        if (b.name == "AddLiscieButton")
        {
            addLeaves += upgradeAddValue;
            liscieAddText.text = "+" + addLeaves;
        }
        if (b.name == "AddKorzenButton")
        {
            addRoots += upgradeAddValue;
            korzenAddText.text = "+" + addRoots;
        }
        if (b.name == "SubKoraButton")
        {
            if (addBark - upgradeAddValue >= 0)
            {
                addBark -= upgradeAddValue;
                koraAddText.text = "+" + addBark;
            }

        }
        if (b.name == "SubLiscieButton")
        {
            if (addLeaves - upgradeAddValue >= 0)
            {
                addLeaves -= upgradeAddValue;
                liscieAddText.text = "+" + addLeaves;
            }
        }
        if (b.name == "SubKorzenButton")
        {
            if (addRoots - upgradeAddValue >= 0)
            {
                addRoots -= upgradeAddValue;
                korzenAddText.text = "+" + addRoots;
            }
        }

        foreach (GameObject t in selectedTrees)
        {
            if (t.GetComponent<TreeController>().CanBeUpgraded(addRoots, addLeaves, addBark) != true)
            {
                //jesli nie wyswietlenie go na czerwono
                t.GetComponent<Renderer>().material.shader = Shader.Find("markRed");
            }
            
        }
        



    }

    public void UpgradeTrees()
    {
        foreach (GameObject t in selectedTrees)
        {
            if (t.GetComponent<TreeController>().CanBeUpgraded(addRoots, addLeaves, addBark))
            {
                t.GetComponent<TreeController>().Upgrade(addRoots, addLeaves, addBark);
            }

        }
    }

    public void DisableSubButtons()
    {
        subKora.interactable = false;
        subKorzen.interactable = false;
        subLiscie.interactable = false;

    }

    public void DisableSubButtons(Button b)
    {
        if (b.name == "SubKoraButton")
        {
            if (addBark <= 0)
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
            if (addLeaves <= 0)
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
            if (addRoots <= 0)
            {
                b.interactable = false;
            }
            else
            {
                b.interactable = true;
            }
        }
    }

    public void DeleteSelection()
    {
        foreach (GameObject t in selectedTrees)
        {
            t.GetComponent<TreeController>().DeselectTree();
        }

        selectedTrees.Clear();

    }

    public void ClearValues()
    {
        koraAddText.text = "+0";
        liscieAddText.text = "+0";
        korzenAddText.text = "+0";
        addRoots = 0;
        addLeaves = 0;
        addBark = 0;
    }


}
