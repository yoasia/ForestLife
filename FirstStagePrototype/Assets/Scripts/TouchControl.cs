using System;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.2f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.2f;        // The rate of change of the orthographic size in orthographic mode.
    public float moveSpeed = 0.2f;
    public float maxZoom = 179.9f, minZoom = 0.1f;
    public float turnSpeed = 0.2f;


    void Update()
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
            float touchDeltaPosition = deltaMagnitudeDiff * perspectiveZoomSpeed;
            //Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
            //zoom

            Debug.Log(touchOne.deltaPosition.x);
            //Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, -touchOne.deltaPosition.y * turnSpeed);
           Camera.main.transform.RotateAround(Camera.main.transform.position, Vector3.up, touchOne.deltaPosition.x * turnSpeed);
           Camera.main.transform.Translate(0, 0, -touchDeltaPosition);

        }
        if (Input.touchCount == 1)
        {
            
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                
                Camera.main.transform.Translate(-touchDeltaPosition.x * moveSpeed, -touchDeltaPosition.y * moveSpeed, 0);
            }

           
        }

    }
}
