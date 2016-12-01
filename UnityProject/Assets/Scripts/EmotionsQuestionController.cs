using UnityEngine;
using System.Collections;
using System;

public class EmotionsQuestionController : MonoBehaviour {

    static public EmotionsQuestionController instance;
    public Canvas mainCanvas;
    public Canvas panelQuestions1;
    public Canvas panelQuestions2;
    public Canvas panelQuestions3;
    public string fileName;

    string[] answers;
    public Canvas[] panelsQuestions;
    private int currentQuestion = 0;

    
    // Use this for initialization
    void Start () {
        instance = this;
        panelsQuestions = new Canvas[3];
        answers = new string[5];
        panelsQuestions[0] = panelQuestions1;
        panelsQuestions[1] = panelQuestions2;
        panelsQuestions[2] = panelQuestions3;

        //wyświetlamy pierwsze pytanie
        panelQuestions1.enabled = true;
        panelQuestions2.enabled = false;
        panelQuestions3.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void afterAnswer(string answer)
    {
        panelsQuestions[currentQuestion].enabled = false;

        answers[currentQuestion++] = answer;

        if (currentQuestion == panelsQuestions.Length)
        {
            //jeżeli to było ostatnie pytanie
            answers[currentQuestion++] = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            answers[currentQuestion++] = DateTime.Now.TimeOfDay.ToString();
            saveAnswer();
            Destroy();
            return;
        }
        panelsQuestions[currentQuestion].enabled = true;

    }

    void saveAnswer()
    {
        string newRow = String.Join(",", answers);
        GameManager.addRowToFile(fileName, newRow);
    }

    void Destroy()
    {
        DestroyObject(gameObject);
    }
}
