﻿using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;

public class QuizController : MonoBehaviour {


    public Text questionText;
    public GameObject answerPrefab;
    public GameObject answersContainer;
    public GameObject resultPopUp;
    public Text popupHeaderText, correctText, allText, bonusText; 

    private bool displayNextQuestion = true;
    private JsonData quizData;
    private bool clickedAnswer;
    private int maxRoundQuestions = 1, thisRoundQuestion=0;
    private int correctAnswers = 0, wrongAnswers = 0;
    bool newQuiz = true;
	
    void Start () {
        
        
	}
	
	
	void Update () {
        
            
            if (displayNextQuestion)
            {
                if (newQuiz)
                {
                    DisplayQuestion();
                    newQuiz = false;
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!endQuiz())
                        {
                            DisplayQuestion();
                        }
                        


                    }
                }
                
            }

            if (JsonDataManager.instance.allQuestionsDisplayed)
            {
                GameManager.instance.SetGameState(GameManager.GameState.GS_ISLAND);
            }

	}


    

    public void DisplayQuestion()
    {
        if (displayNextQuestion)
        {
            GameObject[] buttonsDestroy = GameObject.FindGameObjectsWithTag("Answer");
            if (buttonsDestroy != null)
            {
                for (int x = 0; x < buttonsDestroy.Length; x++)
                {
                    DestroyImmediate(buttonsDestroy[x]);
                }
            }
            JsonDataManager.instance.NewQuestionNumber();
            if (JsonDataManager.instance.allQuestionsDisplayed == false)
            {
                if (JsonDataManager.instance.currentQuestionFile == 's')
                {
                    quizData = JsonDataManager.instance.quizSpeciesData;
                }
                else if (JsonDataManager.instance.currentQuestionFile == 'o')
                {
                    quizData = JsonDataManager.instance.quizOtherData;
                }
                questionText.text = quizData["data"][JsonDataManager.instance.currentQuestionNumber]["question"].ToString();

                for (int i = 0; i < quizData["data"][JsonDataManager.instance.currentQuestionNumber]["answers"].Count; i++)
                {
                    GameObject answer = Instantiate(answerPrefab);
                    answer.GetComponentInChildren<Text>().text = quizData["data"][JsonDataManager.instance.currentQuestionNumber]["answers"][i].ToString();
                    answer.transform.SetParent(answersContainer.transform, false);

                    string x = i.ToString();

                    if (i == 0)
                    {
                        answer.name = "correctAnswer";
                        answer.GetComponent<Button>().onClick.AddListener(() => answerListener("0"));
                    }
                    else
                    {
                        answer.name = "wrongAnswer" + x;
                        answer.GetComponent<Button>().onClick.AddListener(() => answerListener(x));
                    }
                    answer.transform.SetSiblingIndex(Random.Range(0, 3));
                }
                displayNextQuestion = false;
                thisRoundQuestion++;
                clickedAnswer = false;
            }
            
        }
        
    }


    public void answerListener(string x)
    {
        if (!clickedAnswer)
        {
            if (x == "0")
            {
                GameObject.Find("correctAnswer").GetComponent<Button>().image.color = Color.green;
                GameManager.instance.BehaviouralData("Good answer");
                correctAnswers++;
            }
            else
            {
                GameObject.Find("wrongAnswer" + x).GetComponent<Button>().image.color = Color.red;
                GameObject.Find("correctAnswer").GetComponent<Button>().image.color = Color.green;
                GameManager.instance.BehaviouralData("Bad answer");
                wrongAnswers++;
            }
            displayNextQuestion = true;
            clickedAnswer = true;
        }
        
        

    }


    public bool endQuiz()
    {
        if (clickedAnswer)
        {
            if (maxRoundQuestions <= thisRoundQuestion)
            
            {
                resultPopUp.SetActive(true);
                if (correctAnswers == maxRoundQuestions)
                {
                    popupHeaderText.text = "Gratulacje!";
                    correctText.text = correctAnswers.ToString();
                    allText.text = maxRoundQuestions.ToString();
                    GameManager.instance.GoodQuizAnswer();
                    bonusText.text = "Twoje drzewa otrzymują bonus do wzrostu";
                    return true;
                }
                else
                {
                    popupHeaderText.text = "Niestety!";
                    correctText.text = correctAnswers.ToString();
                    allText.text = maxRoundQuestions.ToString();
                    GameManager.instance.BadQuizAnswer();
                    bonusText.text = "Twoje drzewa otrzymują karę do wzrostu";
                    return true;
                }
               
            }
        }
        return false;
    }
   
    public  void ResetQuiz () 
    {
        newQuiz = true;
        resultPopUp.SetActive(false);
        thisRoundQuestion = 0;
        correctAnswers = 0;
        wrongAnswers = 0;
        displayNextQuestion = true;
        clickedAnswer = false;
        GameObject[] buttonsDestroy = GameObject.FindGameObjectsWithTag("Answer");
        if (buttonsDestroy != null)
        {
            for (int x = 0; x < buttonsDestroy.Length; x++)
            {
                DestroyImmediate(buttonsDestroy[x]);
            }
        }
    }

}
