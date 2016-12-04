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
    public char fileSymbol;
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
        JsonDataManager.instance.SetNextTriviaNumber();
        if (!JsonDataManager.instance.allTriviaDisplayed)
        {
            
            Trivia newTrivia = new Trivia();
            newTrivia.id = JsonDataManager.instance.currentTriviaNumber;
            if (JsonDataManager.instance.currentTriviaFile == 's')
            {
                
                newTrivia.fileSymbol = 's';
                newTrivia.title = JsonDataManager.instance.triviaSpeciesData["data"][JsonDataManager.instance.currentTriviaNumber]["title"].ToString();
                newTrivia.content = JsonDataManager.instance.triviaSpeciesData["data"][JsonDataManager.instance.currentTriviaNumber]["content"].ToString();
            }
            else if (JsonDataManager.instance.currentTriviaFile == 'o')
            {
                
                newTrivia.fileSymbol = 'o';
                newTrivia.title = JsonDataManager.instance.triviaOtherData["data"][JsonDataManager.instance.currentTriviaNumber]["title"].ToString();
                newTrivia.content = JsonDataManager.instance.triviaOtherData["data"][JsonDataManager.instance.currentTriviaNumber]["content"].ToString();
            }
            
            
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
