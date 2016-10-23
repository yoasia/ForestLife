using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.2f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.2f;        // The rate of change of the orthographic size in orthographic mode.
    public float moveSpeed = 0.2f;
    public float maxZoom = 179.9f, minZoom = 0.1f;

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (Camera.main.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minZoom);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                //Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                float touchDeltaPosition = deltaMagnitudeDiff * perspectiveZoomSpeed;
                // Clamp the field of view to make sure it's between 0 and 180.
                //Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
                Camera.main.transform.Translate(0, 0, -touchDeltaPosition);
            }
        }
        if (Input.touchCount == 1)
        {
            
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                // Get movement of the finger since last frame
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                // Move object across XY plane
                Camera.main.transform.Translate(-touchDeltaPosition.x * moveSpeed, -touchDeltaPosition.y * moveSpeed, 0);
            }

           
        }

    }
}
