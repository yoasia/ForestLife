using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewSeedChooseCanvasController : MonoBehaviour
{

    public GameObject popupPanel;
    public GameObject choosingTreePanel;
    public Camera cam;

    private GameObject selectedTree;

    bool choosingMode = false;
    Ray ray;
    RaycastHit hit;
    private Animator animatorPopup;


    // Use this for initialization
    void Start()
    {

        animatorPopup = popupPanel.GetComponent<Animator>();
        animatorPopup.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (choosingMode)
        {
            if (!choosingTreePanel.activeSelf)
            {
                choosingTreePanel.SetActive(true);
            }
            if (popupPanel.activeSelf)
            {
                popupPanel.SetActive(false);
            }
            PointSelect();
        }
        else
        {
            if (choosingTreePanel.activeSelf)
            {
                choosingTreePanel.SetActive(false);
            }
            if (!popupPanel.activeSelf)
            {
                PopupOn();
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PopupOff();
                    choosingMode = true;

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
                    if (selectedTree == null)
                    {
                        selectedTree = hit.transform.gameObject;
                        //selectedTree.GetComponent<Renderer>().material.color = Color.blue;
                        selectedTree.GetComponent<TreeController>().SelectTree();
                    }
                    else
                    {
                        //selectedTree.GetComponent<TreeController>().ReturnDefaultColour();
                        selectedTree.GetComponent<TreeController>().UnselectTreeByType();
                        selectedTree = hit.transform.gameObject;
                        selectedTree.GetComponent<TreeController>().SelectTree();
                    }

                }

            }

        }

    }

    public void SetNewSeed()
    {
        if (selectedTree != null)
        {
            popupPanel.SetActive(false);
            choosingTreePanel.SetActive(false);
            //selectedTree.GetComponent<TreeController>().ReturnDefaultColour();
            selectedTree.GetComponent<TreeController>().UnselectTreeByType();
            choosingMode = false;

            GameManager.instance.NewSeed(selectedTree);
            selectedTree = null;
        }

    }


    public void PopupOn()
    {
        popupPanel.SetActive(true);
        animatorPopup.enabled = true;
        animatorPopup.Play("NewSeedPopupIn");

    }
    public void PopupOff()
    {
        if (popupPanel.activeSelf)
        {
            animatorPopup.Play("NewSeedPopupOut");
            popupPanel.SetActive(false);
        }
    }

}
