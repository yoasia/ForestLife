using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionCameraController : MonoBehaviour {


    public GameObject rightMenuPanel;
    public bool selectingMode = true;

    private Animator animatorRightMenu;
    private bool rightMenuVisable = false;
    public Button selectButton, deselectButton;
    public Sprite selSpriteNorm, deselSppriteNorm, selSpriteHi, deselSppriteHi;
    public Camera cam;
    public GameObject canvas;

    public float zoomSpeed = 0.2f;
    public float moveSpeed = 0.2f;

    Ray ray;
    RaycastHit hit;

	// Use this for initialization
	void Start () {

        animatorRightMenu = rightMenuPanel.GetComponent<Animator>();
        animatorRightMenu.enabled = false;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.instance.selectingMode)
        {
            ZoomCamera();
            MoveCamera();

            PointSelect();
        }
        //Button b = GameObject.Find("UI/SelectCanvas/AcceptSelectionButton").GetComponent<Button>();
        
        
	}


    public void RightMenuOnOff()
    {
        if (rightMenuVisable)
        {
            rightMenuVisable = false;
            animatorRightMenu.Play("ChangeTreesPamMenuOut");
            //set back the time scale to normal time scale
            //Time.timeScale = 1;
        }
        else
        {
            animatorRightMenu.enabled = true;
            animatorRightMenu.Play("ChangeTreesPamMenuIn");
            rightMenuVisable = true;
            //freeze the timescale
            //Time.timeScale = 0;
        }

    }



    public void SelectModeOn()
    {
        selectingMode = true;
       selectButton.image.sprite = selSpriteHi;
       deselectButton.image.sprite = deselSppriteNorm;
       
    }

    public void SelectModeOff()
    {
        selectingMode = false;
            
        selectButton.image.sprite = selSpriteNorm;
        deselectButton.image.sprite = deselSppriteHi;

    }
    private void PointSelect()
    {

        if (Input.touchCount > 0)
        {
            ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
            //Debug.DrawRay(ray.origin, ray.direction * 2000);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Tree")
                {
                    
                    if (selectingMode)
                    {
                        if (GameManager.instance.selectedTrees.Contains(hit.transform.gameObject) == false)
                        {
                            hit.transform.gameObject.GetComponent<TreeController>().selected = true;
                            GameManager.instance.selectedTrees.Add(hit.transform.gameObject);
                        }
                        
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<TreeController>().selected = false;
                        GameManager.instance.selectedTrees.Remove(hit.transform.gameObject);
                    }
                }



            }

        }

    }

    public void ZoomCamera()
    {
        if (Input.touchCount == 2)
        {

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);


            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;


            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            

            cam.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

            
            cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);


        }
    }
    public void MoveCamera()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                transform.Translate(-touchDeltaPosition.x * moveSpeed, 0, 0);
                transform.Translate(0, -touchDeltaPosition.y * moveSpeed, 0);
            }
        }
    }
}
