using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TriviaButtonController : MonoBehaviour {

    public Button button;
    public Text labelText;
    public GameObject newImage;

    private Trivia trivia;
    private TriviaListController triviaListController;
    

	// Use this for initialization
	void Start () {

        button.onClick.AddListener(HandleClick);
	}
	
	// Update is called once per frame
	void Update () {

        
	}

    public void Setup(Trivia t, TriviaListController tc )
    {
        trivia = t;
        labelText.text = t.title;
        triviaListController = tc;
        newImage.SetActive(true);
    }

    public void HandleClick()
    {
        newImage.SetActive(false);
        triviaListController.DisplayTriviaContent(trivia);
    }
}
