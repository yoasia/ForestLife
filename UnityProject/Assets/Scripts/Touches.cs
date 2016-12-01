using UnityEngine;
using System.Collections;

public class Touches : MonoBehaviour
{
    public float minDist = 2.0f;
    public float maxDist = 5.0f;
    public Transform projectile;
    public float speed = 10.0f;
    private Vector3 moveVec;
    private float startZ;
    private float actualDist;
    private Vector2 dragStartPos;
    private Vector3 terrainCentrum;
    private Vector3 terrainDimensions;
    private Vector3 angleRotation = new Vector3(0, 1, 0);

    void Start()
    {
        startZ = projectile.position.z;

        terrainCentrum = new Vector3(0, 0, 0);
        terrainDimensions = new Vector3(0, 0, 0);

        float positionX = Terrain.activeTerrain.transform.position.x;
        float positionY = Terrain.activeTerrain.transform.position.y;
        float positionZ = Terrain.activeTerrain.transform.position.z;

        terrainDimensions = Terrain.activeTerrain.terrainData.size;

        //środek terenu
        terrainCentrum.x = positionX;
        terrainCentrum.y = positionY;
        terrainCentrum.z = positionZ;
    }

    void Update()
    {
        //This section for move the camera position only limited values . 
        // You can Change the values for your requirements.
        //Here the camera will move with in your required portion on the screen.

        if (projectile.position.z >= terrainCentrum.z + 400)
        {
            transform.position = transform.position + new Vector3(0.0f, 0.0f, -10.0f);
        }
        else if (projectile.position.z <= terrainCentrum.z - terrainDimensions.z)
        {
            transform.position = transform.position + new Vector3(0.0f, 0.0f, 10.0f);
        }

        if (projectile.position.y >= terrainCentrum.y +500)
        {
            transform.position = transform.position + new Vector3(0.0f, -10.0f, 0.0f);
        }
        else if (projectile.position.y <= terrainCentrum.y)
        {
            transform.position = transform.position + new Vector3(0.0f, 10.0f, 0.0f);
        }

        if (projectile.position.x >= terrainCentrum.x + terrainDimensions.x)
        {
            transform.position = transform.position + new Vector3(-5.0f, 0.0f, 0.0f);
        }
        else if (projectile.position.x <= terrainCentrum.z - terrainDimensions.z)
        {
            transform.position = transform.position + new Vector3(5.0f, 0.0f, 0.0f);
        }

        // This Section For to move camera according to your swipe on screen.       
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    dragStartPos = touch.position;
                    moveVec = Vector2.zero;
                    break;

                case TouchPhase.Moved:
                    Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
                    pos.z = startZ;
                    projectile.position = Camera.main.ScreenToWorldPoint(touch.position);
                    //here i gave condition to move camera with in required position
                    if (projectile.position.z >= terrainCentrum.z - terrainDimensions.z - 200  && projectile.position.z <= terrainCentrum.z + terrainDimensions.z + 200)
                    {
                        moveVec = -(touch.position - dragStartPos) * speed;
                        projectile.Translate(moveVec * Time.deltaTime);
                        Vector3 val = moveVec * Time.deltaTime;
                        dragStartPos = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                    dragStartPos = touch.position;
                    moveVec = Vector2.zero;
                    break;
            }

        }

        //This section for pinch Zooming on screen.       
        if (Input.touchCount == 2)
        {
            Touch touch = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            //dragStartPos = touch.position;
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended) {
                dragStartPos = touch.position;
                moveVec = Vector2.zero;
            }
            if (touch.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                //obrót
                moveVec = -(touch.position - dragStartPos);
                transform.RotateAround(transform.position, terrainCentrum.y * Vector3.up, -moveVec.x * speed * Time.deltaTime);
                dragStartPos = touch.position;
                        
                       
                Vector2 curDist = touch.position - touch1.position;
                Vector2 prevDist = (touch.position - touch.deltaPosition) - (touch1.position - touch1.deltaPosition);
                float delta = curDist.magnitude - prevDist.magnitude;
                if (Mathf.Abs(delta) > 10)
                    Camera.main.transform.Translate(0, 0, delta * .5f);


            }
        }


    }

    //public Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    //{
    //    return angle* ( point - pivot) + pivot;
    //}
    //public Vector3 RotatePointAroundPivot(Vector3 currentPosition, Vector3 angles) {
    //    Vector3 dir = currentPosition - terrainCentrum; // get point direction relative to pivot
    //    dir = Quaternion.Euler(angles) * dir; // rotate it
    //    currentPosition = dir + terrainCentrum; // calculate rotated point
    //    return currentPosition; // return it
    //}


    //Returns the rotated Vector3 using a Quaterion
    public Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Quaternion Angle)
    {
        return Angle * (Point - Pivot) + Pivot;
    }
    //Returns the rotated Vector3 using Euler
    public Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Vector3 Euler)
    {
        return RotateAroundPivot(Point, Pivot, Quaternion.Euler(Euler));
    }
    //Rotates the Transform's position using a Quaterion
    public void RotateAroundPivot( Vector3 Pivot, Quaternion Angle)
    {
        Camera.main.transform.position = RotateAroundPivot(Camera.main.transform.position, Pivot, Angle);
    }
    //Rotates the Transform's position using Euler
    public void RotateAroundPivot( Vector3 Pivot, Vector3 Euler)
    {
        Camera.main.transform.position = RotateAroundPivot(Camera.main.transform.position, Pivot, Quaternion.Euler(Euler));
    }
}