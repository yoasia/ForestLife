using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using LitJson;


[System.Serializable]
public class Trivia
{
    public int id;
    public string title;
    public string content;
}

public class TriviaListController : MonoBehaviour {

    public List<Trivia> triviaList;
    public Transform triviaListPanel;
    public GameObject triviaButtonPrefab;

    public GameObject triviaContentPopup;
    public GameObject triviaListWindowPanel;
    public Text triviaHeaderText;
    public Text triviaContentText;
    
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    

    public void LoadNewTrivia()
    {
        if (!JsonDataManager.instance.allTriviaDisplayed)
        {

            Trivia newTrivia = new Trivia();
            newTrivia.id = JsonDataManager.instance.currentTriviaNumber;
            newTrivia.title = JsonDataManager.instance.triviaData["data"][JsonDataManager.instance.currentTriviaNumber]["title"].ToString();
            newTrivia.content = JsonDataManager.instance.triviaData["data"][JsonDataManager.instance.currentTriviaNumber]["content"].ToString();

            JsonDataManager.instance.SetNextTriviaNumber();
            AddTrivia(newTrivia);
        }

    }
    

  

    public void AddTrivia(Trivia newTrivia)
    {
        triviaList.Add(newTrivia);
        GameObject newButton = (GameObject)GameObject.Instantiate(triviaButtonPrefab);
        newButton.transform.SetParent(triviaListPanel, false);
        newButton.GetComponent<TriviaButtonController>().Setup(newTrivia, this);

    }

    public void DisplayTriviaContent(Trivia t)
    {
        triviaListWindowPanel.SetActive(false);
        triviaContentPopup.SetActive(true);
        triviaHeaderText.text = t.title;
        triviaContentText.text = t.content;

    }

    public void HideTriviaContent()
    {
        triviaListWindowPanel.SetActive(true);
        triviaContentPopup.SetActive(false);

    }

}
