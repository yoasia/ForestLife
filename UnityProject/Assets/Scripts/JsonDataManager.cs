using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class JsonDataManager : MonoBehaviour {

    public static JsonDataManager instance;
    public int triviaNumber;
    public int questionNumber;
    public int currentTriviaNumber, currentQuestionNumber;

    public JsonData triviaData;
    public JsonData quizData;

    

    public bool questionsLoaded = false;
    public bool triviaLoaded = false;
    public bool allTriviaDisplayed = false;
    public bool allQuestionsDisplayed = false;

    public string treeSpecies;

    private List<int> notUsedQuestions = new List<int>();
    
    private string tFilePath;
    private string triviaJsonString;

    private string qFilePath;
    private string quizJsonString;




    public void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadTriviaData()
    {
        if (treeSpecies != null)
        {

            tFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Trivia\\" + treeSpecies + ".json");
            if (System.IO.File.Exists(tFilePath))
            {
                triviaJsonString = System.IO.File.ReadAllText(tFilePath);
                triviaData = JsonMapper.ToObject(triviaJsonString);
                currentTriviaNumber = 0;
                JsonData td = triviaData["data"];
                triviaNumber = td.Count; 
                triviaLoaded = true;
            }
        }
        
        

    }

    public void LoadQuizData()
    {
        if (treeSpecies != null)
        {
            qFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "QuizQuestions\\" + treeSpecies + ".json");
            if (System.IO.File.Exists(qFilePath))
            {
                quizJsonString = System.IO.File.ReadAllText(qFilePath);
                quizData = JsonMapper.ToObject(quizJsonString);
                currentQuestionNumber = 0;
                JsonData td = quizData["data"];
                questionNumber = td.Count; 
                questionsLoaded = true;
            }
        }
        

    }

    public void SetNextTriviaNumber()
    {
        if (currentTriviaNumber < triviaNumber)
        {
            notUsedQuestions.Add(currentTriviaNumber);
            currentTriviaNumber++;
            if (currentTriviaNumber >= triviaNumber)
            {
                allTriviaDisplayed = true;
            }
            if (notUsedQuestions.Count > 0)
            {
                allQuestionsDisplayed = false;
            }
        }
        
        
    }

    public void NewQuestionNumber()
    {
        int index = Random.Range(0, (notUsedQuestions.Count - 1));
        currentQuestionNumber = notUsedQuestions[index];
        notUsedQuestions.RemoveAt(index);

        if (notUsedQuestions.Count <= 0)
        {
            allQuestionsDisplayed = true;
        }
    }



}
