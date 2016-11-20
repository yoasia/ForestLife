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
    
    public EventSystem eventSystem;
    public Text koraAddText, korzenAddText, liscieAddText;

    public List<GameObject> selectedTrees = new List<GameObject>();
    

    int addKora = 0, addKorzen = 0, addLiscie = 0;
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
            addKora += upgradeAddValue;
            koraAddText.text = "+" + addKora;
        }
        if (b.name == "AddLiscieButton")
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

        foreach (GameObject t in selectedTrees)
        {
            //wywolanie funkcji mowiacej czy można zupgradowac drzewo
            //t.GetComponent<TreeController>().FunkcjaMowiacaCzyDrzewoMozeMiecUpgrade();
            //jesli nie wyswietlenie go na czerwono
            t.GetComponent<Renderer>().material.shader = Shader.Find("markRed");
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

    public void DeleteSelection()
    {
        foreach (GameObject t in selectedTrees)
        {
            t.GetComponent<TreeController>().DeselectTree();
        }

        selectedTrees.Clear();

    }


}
