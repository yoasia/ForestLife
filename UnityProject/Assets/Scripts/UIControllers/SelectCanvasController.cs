﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SelectCanvasController : MonoBehaviour
{



    public Camera cam;
    public GameObject rightMenuPanel;
    public bool selectingMode = true;

    public Button selectButton, deselectButton;
    public Button acceptSelButton, returnButton;
    public Button subKora, subKorzen, subLiscie;

    public EventSystem eventSystem;
    public Text koraAddText, korzenAddText, liscieAddText;
    public Text infoText;
    public List<GameObject> selectedTrees = new List<GameObject>();


    int addBark = 0, addRoots = 0, addLeaves = 0;
    int upgradeAddValue = 10;
    Ray ray;
    RaycastHit hit;
    private Animator animatorRightMenu;
    private bool rightMenuVisable = false;


    void Start()
    {
        animatorRightMenu = rightMenuPanel.GetComponent<Animator>();
        animatorRightMenu.enabled = false;
        SelectModeOn();
    }


    void Update()
    {
        if (GameManager.instance.currentGameState == GameManager.GameState.GS_SELECTING)
        {
            PointSelect();
        }
    }




    public void RightMenuOnOff()
    {
        foreach (GameObject t in selectedTrees)
        {

            t.GetComponent<TreeController>().SelectTree("green");
        }
        infoText.text = "";
        ClearValues();
        DisableSubButtons();
        if (rightMenuVisable)
        {
            rightMenuVisable = false;
            animatorRightMenu.Play("ChangeTreesPamMenuOut");
            acceptSelButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(true);
            
        }
        else
        {
            animatorRightMenu.enabled = true;
            animatorRightMenu.Play("ChangeTreesPamMenuIn");
            rightMenuVisable = true;
            acceptSelButton.gameObject.SetActive(false);
            returnButton.gameObject.SetActive(false);
            
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
                            hit.transform.gameObject.GetComponent<TreeController>().SelectTree("green");
                            selectedTrees.Add(hit.transform.gameObject);
                        }

                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<TreeController>().UnselectTree();
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
        infoText.text = "";
        foreach (GameObject t in selectedTrees)
        {

            t.GetComponent<TreeController>().SelectTree("green");
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
                t.GetComponent<TreeController>().SelectTree("red");
            }

        }




    }

    public void UpgradeTrees()
    {
        bool upgraded = false;
        if (addRoots != 0 || addLeaves != 0 || addBark != 0)
        {
            foreach (GameObject t in selectedTrees)
            {
                if (t.GetComponent<TreeController>().CanBeUpgraded(addRoots, addLeaves, addBark))
                {

                    t.GetComponent<TreeController>().Upgrade(addRoots, addLeaves, addBark);
                    upgraded = true;
                }

            }
            if (upgraded)
            {

                infoText.text = "Zaznaczone drzewa zostały ulepszone";

            }
            else
            {
                infoText.text = "Nie można wykonać ulepszenia";
            }
        }
        else
        {
            infoText.text = "wybierz wartości ulepszenia";
        }

        ClearValues();
        DisableSubButtons();
        DeleteRedSelection();

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
            t.GetComponent<TreeController>().UnselectTree();
        }

        selectedTrees.Clear();

    }

    public void DeleteRedSelection()
    {
        foreach (GameObject t in selectedTrees)
        {
            t.GetComponent<TreeController>().SelectTree("green");
        }


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
