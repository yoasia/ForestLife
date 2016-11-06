using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.2f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.2f;        // The rate of change of the orthographic size in orthographic mode.
    public float moveSpeed = 0.2f;
    public float maxZoom = 179.9f, minZoom = 0.1f;
    public float turnSpeed = 0.1f;
    bool collision = false;

    public Texture2D selectionHeighlight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);
    private Vector3 startClick = -Vector3.one;
    Ray ray;
    RaycastHit hit;

    void Update()
    {

        //RectSelect();
        PointSelect();
        //---obrót, zoom, przesuwanie---- 
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

            //Debug.Log(touchOne.deltaPosition.x);
            //Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, -touchOne.deltaPosition.y * turnSpeed);
            transform.RotateAround(transform.position, Vector3.up, -touchOne.deltaPosition.x * turnSpeed);
            transform.Translate(0, 0, -touchDeltaPosition);
            

        }
        if (Input.touchCount == 1)
        {

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {

                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                 transform.Translate(-touchDeltaPosition.x * moveSpeed, -touchDeltaPosition.y * moveSpeed, 0);
                
            }


        }

        //---obrót, zoom, przesuwanie---- 

    }
    private void PointSelect ()
    {

        if (Input.touchCount > 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Debug.DrawRay(ray.origin, ray.direction * 20);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.transform.gameObject.tag == "Tree") {
                    hit.transform.gameObject.GetComponent<TreeControll>().selected = true;
                }
                

            }

        }
    
    }
    private void RectSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {

            startClick = Input.mousePosition;


        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selection.width <0)
            {
                selection.x += selection.width;
                selection.width = -selection.width;
            }
            if(selection.height <0)
            {
                selection.y += selection.height;
                selection.height = -selection.height;
            }

            startClick = -Vector3.one;
            
        }
        if (Input.GetMouseButton(0))
        {
            selection = new Rect(startClick.x, 
                InverseMouseY(startClick.y), 
                Input.mousePosition.x - startClick.x, 
                InverseMouseY(Input.mousePosition.y) - InverseMouseY(startClick.y));
        }
    }

    private void OnGUI ()
    {
        if (startClick != -Vector3.one )
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selection, selectionHeighlight);
        }
    }

    public static float InverseMouseY (float y)
    {
        return Screen.height - y;
    }

}

