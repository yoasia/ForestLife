using UnityEngine;
using System.Collections;

public class EmotionButtonClicked : MonoBehaviour {

    string answer;
	// Use this for initialization
	void Start () {
        answer = gameObject.name;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onClick()
    {
        EmotionsQuestionController.instance.afterAnswer(answer);
    }
}
