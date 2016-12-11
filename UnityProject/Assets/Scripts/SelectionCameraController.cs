using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionCameraController : MonoBehaviour {

    public Camera cam;
    

    public float zoomSpeed = 0.2f;
    public float moveSpeed = 0.2f;

    
	void Start () {

        
        
        
	}
	
	
	void Update () {
        if (GameManager.instance.currentGameState == GameManager.GameState.GS_SELECTING)
        {
            ZoomCamera();
            MoveCamera();

            
        }
        
        
	}


    
   

    public void ZoomCamera()
    {
        if (Input.touchCount == 2)
        {

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);


            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;


            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            

            cam.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

            
            cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);


        }
    }
    public void MoveCamera()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                transform.Translate(-touchDeltaPosition.x * moveSpeed, 0, 0);
                transform.Translate(0, -touchDeltaPosition.y * moveSpeed, 0);
            }
        }
    }
}
