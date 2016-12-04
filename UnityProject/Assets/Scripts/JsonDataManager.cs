using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;

public class JsonDataManager : MonoBehaviour {

    public static JsonDataManager instance;
    public int triviaSpeciesNumber, triviaOtherNumber;
    public int questionSpeciesNumber, questionOtherNumber;
    public int currentTriviaNumber, currentQuestionNumber;

    public JsonData triviaSpeciesData, triviaOtherData;
    public JsonData quizSpeciesData, quizOtherData;
    public char currentTriviaFile = ' ', currentQuestionFile = ' ';
    

    public bool questionsLoaded = false;
    public bool triviaLoaded = false;
    public bool allTriviaDisplayed = false;
    public bool allQuestionsDisplayed = false;

    public string treeSpecies;

    private List<int> notUsedSpeciesQuestions = new List<int>();
    private List<int> notUsedOtherQuestions = new List<int>();

    private List<int> notDisplayedSpeciesTrivia = new List<int>();
    private List<int> notDisplayedOtherTrivia = new List<int>();

    private string tSpeciesFilePath, tOtherFilePath;
    private string triviaSpeciesJsonString, triviaOtherJsonString;

    private string qSpeciesFilePath, qOtherFilePath;
    private string quizSpeciesJsonString, quizOtherJsonString;




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

            tSpeciesFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, treeSpecies + ".json");
            
            //StartCoroutine("Jsont");
            if (tSpeciesFilePath.Contains("://"))
            {
                WWW www = new WWW(tSpeciesFilePath);
                //yield return www;
                triviaSpeciesJsonString = www.text;
            }
            else
            {
                triviaSpeciesJsonString = System.IO.File.ReadAllText(tSpeciesFilePath);
            }

            triviaSpeciesData = JsonMapper.ToObject(triviaSpeciesJsonString);
            JsonData td = triviaSpeciesData["data"];
            triviaSpeciesNumber = td.Count;
            for (int i = 0; i < triviaSpeciesNumber; i++)
            {
                notDisplayedSpeciesTrivia.Add(i);
            }
            
        }

        tOtherFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Ogolne" + ".json");

        //StartCoroutine("Jsont");
        if (tOtherFilePath.Contains("://"))
        {
            WWW www = new WWW(tOtherFilePath);
            //yield return www;
            triviaOtherJsonString = www.text;
        }
        else
        {
            triviaOtherJsonString = System.IO.File.ReadAllText(tOtherFilePath);
        }
        triviaOtherData = JsonMapper.ToObject(triviaOtherJsonString);
        JsonData tdo = triviaOtherData["data"];
        triviaOtherNumber = tdo.Count;
        for (int i = 0; i < triviaOtherNumber; i++)
        {
            notDisplayedOtherTrivia.Add(i);
        }

        currentTriviaNumber = 0;
        if (triviaOtherData != null && triviaSpeciesData != null)
            triviaLoaded = true;
    }

    //IEnumerator Jsont()
    //{
    //    if (tFilePath.Contains("://"))
    //    {
    //        WWW www = new WWW(tFilePath);
    //        yield return www;
    //        triviaJsonString = www.text;
    //    }
    //    else
    //    {
    //        triviaJsonString = System.IO.File.ReadAllText(tFilePath);
    //    }

    //}


    public void LoadQuizData()
    {
        if (treeSpecies != null)
        {
            qSpeciesFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, treeSpecies + "_q.json");
            
            //StartCoroutine("Jsonq");
            if (qSpeciesFilePath.Contains("://"))
            {
                WWW www = new WWW(qSpeciesFilePath);
                //yield return www;
                quizSpeciesJsonString = www.text;
            }
            else
            {
                quizSpeciesJsonString = System.IO.File.ReadAllText(qSpeciesFilePath);
            }
            quizSpeciesData = JsonMapper.ToObject(quizSpeciesJsonString);
            
            JsonData td = quizSpeciesData["data"];
            questionSpeciesNumber = td.Count;
            
        }

        qOtherFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Ogolne" + "_q.json");

        //StartCoroutine("Jsonq");
        if (qOtherFilePath.Contains("://"))
        {
            WWW www = new WWW(qOtherFilePath);
            //yield return www;
            quizOtherJsonString = www.text;
        }
        else
        {
            quizOtherJsonString = System.IO.File.ReadAllText(qOtherFilePath);
        }
        quizOtherData = JsonMapper.ToObject(quizOtherJsonString);
        JsonData tdo = quizOtherData["data"];
        questionOtherNumber = tdo.Count;
        currentQuestionNumber = 0;
        if (quizOtherData != null && quizSpeciesData != null)
            questionsLoaded = true;

    }

    //IEnumerator Jsonq()
    //{
    //    if (qFilePath.Contains("://"))
    //    {
    //        WWW www = new WWW(qFilePath);
    //        yield return www;
    //        quizJsonString = www.text;
    //    }
    //    else
    //    {
    //        quizJsonString = System.IO.File.ReadAllText(qFilePath);
    //    }
    //}

    public void SetNextTriviaNumber()
    {
        if (notDisplayedSpeciesTrivia.Count == 0 && notDisplayedOtherTrivia.Count == 0)
        {
            allTriviaDisplayed = true;
            currentTriviaFile = ' ';
        }
        else if (notDisplayedSpeciesTrivia.Count != 0 && notDisplayedOtherTrivia.Count == 0)
        {
            int index = Random.Range(0, (notDisplayedSpeciesTrivia.Count));
            currentTriviaNumber = notDisplayedSpeciesTrivia[index];
            notDisplayedSpeciesTrivia.RemoveAt(index);
            notUsedSpeciesQuestions.Add(currentTriviaNumber);
            currentTriviaFile = 's';
        }
        else if (notDisplayedSpeciesTrivia.Count == 0 && notDisplayedOtherTrivia.Count != 0)
        {
            int index = Random.Range(0, (notDisplayedOtherTrivia.Count));
            currentTriviaNumber = notDisplayedOtherTrivia[index];
            notDisplayedOtherTrivia.RemoveAt(index);
            notUsedOtherQuestions.Add(currentTriviaNumber);
            currentTriviaFile = 'o';
        }
        else
        {
            int speciesTrivia = Random.Range(0, 2);
            if (speciesTrivia == 1)
            {
                int index = Random.Range(0, (notDisplayedSpeciesTrivia.Count));
                currentTriviaNumber = notDisplayedSpeciesTrivia[index];
                notDisplayedSpeciesTrivia.RemoveAt(index);
                notUsedSpeciesQuestions.Add(currentTriviaNumber);
                currentTriviaFile = 's';
            }
            else
            {
                int index = Random.Range(0, (notDisplayedOtherTrivia.Count));
                currentTriviaNumber = notDisplayedOtherTrivia[index];
                notDisplayedOtherTrivia.RemoveAt(index);
                notUsedOtherQuestions.Add(currentTriviaNumber);
                currentTriviaFile = 'o';
            }
        }
        if (notUsedOtherQuestions.Count > 0)
        {
            allQuestionsDisplayed = false;
        }
        if (notUsedSpeciesQuestions.Count > 0)
        {
            allQuestionsDisplayed = false;
        }

       
        
    }

    public void NewQuestionNumber()
    {
        if (notUsedOtherQuestions.Count == 0 && notUsedSpeciesQuestions.Count == 0)
        {
            allQuestionsDisplayed = true;
            currentQuestionFile = ' ';
        }
        else if (notUsedOtherQuestions.Count != 0 && notUsedSpeciesQuestions.Count == 0)
        {
            int index = Random.Range(0, (notUsedOtherQuestions.Count));
            currentQuestionNumber = notUsedOtherQuestions[index];
            notUsedOtherQuestions.RemoveAt(index);
            currentQuestionFile = 'o';
        }
        else if (notUsedOtherQuestions.Count == 0 && notUsedSpeciesQuestions.Count != 0)
        {
            int index = Random.Range(0, (notUsedSpeciesQuestions.Count));
            currentQuestionNumber = notUsedSpeciesQuestions[index];
            notUsedSpeciesQuestions.RemoveAt(index);
            currentQuestionFile = 's';
        }
        else
        {
            int speciesQuestion = Random.Range(0, 2);
            if (speciesQuestion == 1)
            {
                int index = Random.Range(0, (notUsedSpeciesQuestions.Count));
                currentQuestionNumber = notUsedSpeciesQuestions[index];
                notUsedSpeciesQuestions.RemoveAt(index);
                currentQuestionFile = 's';
            }
            else
            {
                int index = Random.Range(0, (notUsedOtherQuestions.Count));
                currentQuestionNumber = notUsedOtherQuestions[index];
                notUsedOtherQuestions.RemoveAt(index);
                currentQuestionFile = 'o';
            }
        }

        if ((notUsedOtherQuestions.Count <= 0) && (notUsedSpeciesQuestions.Count <= 0))
        {
            allQuestionsDisplayed = true;
        }
        
        
    }



}
