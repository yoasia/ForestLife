using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Camera seedCamera;
    public Camera worldCamera;

    public GameObject leftMenuPanel;
	public bool selectingMode = false;

    private Animator animatorLeftMenu;
    private bool leftMenuVisable = false;
	public Button es;
	public Sprite s;


	Ray ray;
	RaycastHit hit;

    public static GameManager instance;

    public enum GameState
    {
        GS_SEED,
        GS_ISLAND,
        GS_MENU

    }


    public GameState currentGameState = GameState.GS_ISLAND;


    public void Awake()
    {
        instance = this;
    }

    
    void Start () {
	
        animatorLeftMenu = leftMenuPanel.GetComponent<Animator>();
        animatorLeftMenu.enabled = false;
    }
	
	
	void Update () {
        CameraChange();
		if (selectingMode) {
			PointSelect ();

		}


    }


    public void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        

    }


    public void CameraChange()
    {

        if (currentGameState == GameState.GS_ISLAND)
        {
            worldCamera.enabled = true;
            seedCamera.enabled = false;
        }
        else if (currentGameState == GameState.GS_SEED)
        {
            worldCamera.enabled = false;
            seedCamera.enabled = true;
        }
    }

    public void LeftMenuOnOff()
    {
        if (leftMenuVisable) {
            leftMenuVisable = false;
            animatorLeftMenu.Play("LeftMenuSlideOut");
            //set back the time scale to normal time scale
            Time.timeScale = 1;
        }
        else
        {
            animatorLeftMenu.enabled = true;
            animatorLeftMenu.Play("LeftMenuSlideIn");
            leftMenuVisable = true;
            //freeze the timescale
            Time.timeScale = 0;
        }
        
    }
    
	public void SelectModeOnOff() {
		if (selectingMode) {
			selectingMode = false;
			Sprite temp = es.image.sprite;

			es.image.sprite = s;
			s = temp;
			//es.current.SetSelectedGameObject(selectedButton.gameObject, new BaseEventData(EventSystem.current));
			//es.Select();
		}
		else {
			Sprite temp = es.image.sprite;

			es.image.sprite = s;
			s = temp;
			selectingMode = true;
			//es.OnDeselect(;

		}

	}
	private void PointSelect()
	{

		if (Input.touchCount > 0)
		{
			ray = worldCamera.ScreenPointToRay(Input.GetTouch(0).position);
			Debug.DrawRay(ray.origin, ray.direction * 2000);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Debug.Log ("hit");
				Debug.Log (hit.transform.gameObject.name);
				if (hit.transform.gameObject.tag == "Tree")
				{
					Debug.Log ("drzewo");
					if (hit.transform.gameObject.GetComponent<TreeController> ().selected) 
					{
						
						hit.transform.gameObject.GetComponent<TreeController> ().selected = false;
						Debug.Log ("odznacz");
					}
					else 
					{
						hit.transform.gameObject.GetComponent<TreeController> ().selected = true;
						Debug.Log ("zaznacz");
					}
				}



			}

		}

	}

}
