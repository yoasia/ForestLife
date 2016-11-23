using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;

public class QuizController : MonoBehaviour {


    public Text questionText;
    public int questionNumber = 0;
    public GameObject answerPrefab;
    public GameObject answersContainer;
    public GameObject resultPopUp;
    public Text popupHeaderText, correctText, allText, bonusText; 

    private bool displayNextQuestion = true;
    private string filePath;
    private string jsonString;
    private JsonData questionData;
    private bool clickedAnswer;
    private int maxRoundQuestions = 3, thisRoundQuestion=0;
    private int correctAnswers = 0, wrongAnswers = 0;

	// Use this for initialization
	void Start () {
        LoadQuestions("Questions1");
        DisplayQuestion();
	}
	
	// Update is called once per frame
	void Update () {

        if (displayNextQuestion)
        {
            if (Input.GetMouseButtonDown(0))
            {
                endQuiz();
                DisplayQuestion();
            }
        }

	}


    public void LoadQuestions(string fileName)
    {
        filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "QuizQuestions\\"+ fileName +".json");
        jsonString = System.IO.File.ReadAllText(filePath);
        questionData = JsonMapper.ToObject(jsonString);

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

            questionText.text = questionData["data"][questionNumber]["question"].ToString();

            for (int i = 0; i < questionData["data"][questionNumber]["answers"].Count; i++)
            {
                GameObject answer = Instantiate(answerPrefab);
                answer.GetComponentInChildren<Text>().text = questionData["data"][questionNumber]["answers"][i].ToString();
                answer.transform.SetParent(answersContainer.transform);

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
            questionNumber++;
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
                wrongAnswers++;
            }
            displayNextQuestion = true;
            clickedAnswer = true;
        }
        
        

    }


    public void endQuiz()
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

                }
                else
                {
                    popupHeaderText.text = "Niestety!";
                    correctText.text = correctAnswers.ToString();
                    allText.text = maxRoundQuestions.ToString();
                    bonusText.text = "Tym razem nie otrzymujesz bonusu";

                }

            }
        }
    }
   


}
