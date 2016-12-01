using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;

public class QuizController : MonoBehaviour {


    public Text questionText;
    //public int questionNumber = 0;
    public GameObject answerPrefab;
    public GameObject answersContainer;
    public GameObject resultPopUp;
    public Text popupHeaderText, correctText, allText, bonusText; 

    private bool displayNextQuestion = true;
    
    private bool clickedAnswer;
    private int maxRoundQuestions = 1, thisRoundQuestion=0;
    private int correctAnswers = 0, wrongAnswers = 0;
    bool newQuiz = true;
	// Use this for initialization
	void Start () {
        
        
	}
	
	// Update is called once per frame
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
            questionText.text = JsonDataManager.instance.quizData["data"][JsonDataManager.instance.currentQuestionNumber]["question"].ToString();

            for (int i = 0; i < JsonDataManager.instance.quizData["data"][JsonDataManager.instance.currentQuestionNumber]["answers"].Count; i++)
            {
                GameObject answer = Instantiate(answerPrefab);
                answer.GetComponentInChildren<Text>().text = JsonDataManager.instance.quizData["data"][JsonDataManager.instance.currentQuestionNumber]["answers"][i].ToString();
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


    public void answerListener(string x)
    {
        if (!clickedAnswer)
        {
            if (x == "0")
            {
                GameObject.Find("correctAnswer").GetComponent<Button>().image.color = Color.green;
                correctAnswers++;
            }
            else
            {
                GameObject.Find("wrongAnswer" + x).GetComponent<Button>().image.color = Color.red;
                GameObject.Find("correctAnswer").GetComponent<Button>().image.color = Color.green;
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
                    bonusText.text = "Twoje drzewa otrzymują +10 do punktów rozwoju";
                    return true;
                }
                else
                {
                    popupHeaderText.text = "Niestety!";
                    correctText.text = correctAnswers.ToString();
                    allText.text = maxRoundQuestions.ToString();
                    bonusText.text = "Tym razem nie otrzymujesz bonusu";
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
