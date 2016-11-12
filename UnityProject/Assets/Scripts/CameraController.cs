using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 0.2f;       
    public float moveSpeed = 0.2f;
    public float turnSpeed = 0.1f;
    

    
   

    void Update()
    {

		//if (GameManager.instance.selectingMode == false) {
			RotateCamera ();
			ZoomCamera ();
			MoveCamera ();
		//}

    }
    
    


    public void RotateCamera()
    {
        if (Input.touchCount == 2)
        {
            Touch touchOne = Input.GetTouch(1);
            //Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, -touchOne.deltaPosition.y * turnSpeed);
            transform.RotateAround(transform.position, Vector3.up, -touchOne.deltaPosition.x * turnSpeed);
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
            float touchDeltaPosition = deltaMagnitudeDiff * zoomSpeed;
            
            transform.Translate(0, 0, -touchDeltaPosition);


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

