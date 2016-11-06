using UnityEngine;
using System.Collections;



public class GameManager : MonoBehaviour {


    public static GameManager instance;

    public enum GameState
    {
        GS_MENU,
        GS_GAME,
        GS_POINT_SELECT,
        GS_POINT_DESELECT,
        GS_RECT_SELECT,
        GS_RECT_DESELECT,
        GS_MOVE_CAMERA

    }


    public GameState currentGameState = GameState.GS_MENU;


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

    public void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        

    }

}
