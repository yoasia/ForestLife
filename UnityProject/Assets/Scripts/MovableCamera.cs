using UnityEngine;
using System.Collections;

public class MovableCamera : MonoBehaviour
{
    public static MovableCamera instance;
    public GameObject water;
    public float minDistFromTerrain = 10f;
    public float maxDistFromTerrain = 300f;
    public float speed = 2.0f;
    public float rotationSpeed = .05f;

    private float startZ;
    private float actualDist;
    private Vector2 dragStartPos;
    private Vector3 moveVec;
    private Vector3 terrainCentrum;
    private Vector3 terrainDimensions;
    private float avgDeltaTime = 0;
    private float waterPosition;

    private bool canBeMoved = true;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1;
        startZ = transform.position.z;

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

        waterPosition = water.transform.position.y + 3;
    }

    void Update()
    {

        if (canBeMoved)
            moveCamera();

    }

    private void moveCamera()
    {
        if (Time.timeScale > 0)
            avgDeltaTime = (Time.deltaTime + avgDeltaTime) / 2.0f;
        //This section for move the camera position only limited values . 
        // You can Change the values for your requirements.
        //Here the camera will move with in your required portion on the screen.

        if (transform.position.z >= terrainCentrum.z + terrainDimensions.z/2 + maxDistFromTerrain)
            transform.Translate(new Vector3(0, 0, -1), Space.World);
        if (transform.position.z <= terrainCentrum.z - terrainDimensions.z / 2 - maxDistFromTerrain)
            transform.Translate(new Vector3(0, 0, +1), Space.World);
        if (transform.position.y >= terrainCentrum.y + terrainDimensions.y / 2 + maxDistFromTerrain)
            transform.Translate(new Vector3(0, -1, 0), Space.World);
        if (transform.position.y <= terrainCentrum.y - terrainDimensions.y / 2 + minDistFromTerrain)
            transform.Translate(new Vector3(0, +1, 0), Space.World);
        if (transform.position.x >= terrainCentrum.x + terrainDimensions.x/2 + maxDistFromTerrain)
            transform.Translate(new Vector3(-1, 0, 0), Space.World);
        if (transform.position.x <= terrainCentrum.x - terrainDimensions.x/2 - maxDistFromTerrain)
            transform.Translate(new Vector3(1, 0, 0), Space.World);
        if (transform.position.y <= waterPosition)
            transform.Translate(new Vector3(0, 1, 0), Space.World);

        //check if is too cloce to terrain
        float heightTerrainInMyPosition = TerrainManager.instance.GetHeight(transform.position.x, transform.position.z);
        if(heightTerrainInMyPosition != 1.0/0.0)
            if (transform.position.y < minDistFromTerrain + TerrainManager.instance.GetHeight(transform.position.x, transform.position.z))
                transform.Translate(new Vector3(0, +1, 0), Space.World);

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
                    //obrót
                    rotateAfterTouch(touch);
                    dragStartPos = touch.position;
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

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended)
            {
                dragStartPos = touch.position;
                moveVec = Vector2.zero;

                //przesunięcie
                moveAfterTouch(touch);
                dragStartPos = touch.position;

                //przybliż / oddal
                zoomInAfterTouch(touch, touch1);
            }

            if (touch.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
            {
                //przesunięcie
                moveAfterTouch(touch);
                dragStartPos = touch.position;

                //przybliż / oddal
                zoomInAfterTouch(touch, touch1);

            }
        }
    }

    private void moveAfterTouch(Touch touch)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
        pos.z = startZ;
        transform.position = Camera.main.ScreenToWorldPoint(touch.position);
        //here i gave condition to move camera with in required position
        moveVec = -(touch.position - dragStartPos) * speed;
        if (Time.timeScale > 0)
            moveVec *= Time.deltaTime;
        else
            moveVec *= avgDeltaTime;


        if (ifCanBeMoved(moveVec))
        {
            transform.Translate(moveVec);
            Vector3 val = moveVec;
            dragStartPos = touch.position;
        }
        else
        {
            //sprawdź jak dużo możesz jeszcze przesunąć kamerę
            Vector3 newMove = howMuchCanBeMoved(moveVec);
            transform.Translate(newMove, Space.World);
        }

    }

    private void zoomInAfterTouch(Touch touch, Touch touch1)
    {
        Vector2 curDist = touch.position - touch1.position;
        Vector2 prevDist = (touch.position - touch.deltaPosition) - (touch1.position - touch1.deltaPosition);
        float delta = curDist.magnitude - prevDist.magnitude;
        if (Mathf.Abs(delta) > 10)
        {
            Vector3 move = new Vector3(0, 0, delta * .5f);
            if (ifCanBeMoved(move))
                transform.Translate(move);
            else
            {
                Vector3 newMove = howMuchCanBeMoved(move);
                transform.Translate(newMove);
            }
        }
    }
    private void rotateAfterTouch(Touch touch)
    {
        moveVec = -(touch.position - dragStartPos);

        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime * moveVec.x);
        transform.RotateAround(transform.position, transform.right, rotationSpeed * Time.deltaTime * (-moveVec.y));

    }

    public bool ifCanBeMoved(Vector3 movement)
    {

        float terrainHeightInNewPoint = TerrainManager.instance.GetHeight(transform.position.x + movement.x, transform.position.z + movement.z);
        if ((transform.position.z + movement.z >= terrainCentrum.z + terrainDimensions.z/2 + maxDistFromTerrain) ||
            (transform.position.z + movement.z <= terrainCentrum.z  - terrainDimensions.z/2 - maxDistFromTerrain) ||
            (transform.position.y + movement.y >= terrainCentrum.y + terrainDimensions.y / 2 + maxDistFromTerrain) ||
            (transform.position.y + movement.y <= terrainCentrum.y - terrainDimensions.y / 2 + minDistFromTerrain) ||
            (transform.position.x + movement.x >= terrainCentrum.x + terrainDimensions.x/2 + maxDistFromTerrain) ||
            (transform.position.x + movement.x <= terrainCentrum.x - terrainDimensions.x/2 - maxDistFromTerrain) ||
            (transform.position.y <= waterPosition))
                return false;
        else if (transform.position.y + movement.y < terrainHeightInNewPoint + minDistFromTerrain)
        {
            if (terrainHeightInNewPoint == 1.0 / 0.0)
                return true;
            else
                return false;
        }
        else
            return true;
    }

    public Vector3 howMuchCanBeMoved(Vector3 movement)
    {

        float terrainHeightInNewPoint = TerrainManager.instance.GetHeight(transform.position.x + movement.x, transform.position.z + movement.z);

        if ((transform.position.z + movement.z >= terrainCentrum.z + terrainDimensions.z / 2 + maxDistFromTerrain) ||
            (transform.position.z + movement.z <= terrainCentrum.z - terrainDimensions.z / 2 - maxDistFromTerrain) ||
            (transform.position.y + movement.y >= terrainCentrum.y + terrainDimensions.y / 2 + maxDistFromTerrain) ||
            (transform.position.y + movement.y <= terrainCentrum.y - terrainDimensions.y / 2 + minDistFromTerrain) ||
            (transform.position.x + movement.x >= terrainCentrum.x + terrainDimensions.x / 2 + maxDistFromTerrain) ||
            (transform.position.x + movement.x <= terrainCentrum.x - terrainDimensions.x / 2 - maxDistFromTerrain) ||
            (transform.position.y <= waterPosition))
            {
                if (transform.position.z + movement.z >= terrainCentrum.z + terrainDimensions.z / 2 + maxDistFromTerrain)
                    movement.z =  (terrainCentrum.z + terrainDimensions.z / 2 + maxDistFromTerrain) - transform.position.z;
                if (transform.position.z + movement.z <= terrainCentrum.z - terrainDimensions.z / 2 - maxDistFromTerrain)
                    movement.z = terrainCentrum.z - terrainDimensions.z / 2 - maxDistFromTerrain - transform.position.z;
                if (transform.position.y + movement.y >= terrainCentrum.y + terrainDimensions.y / 2 + maxDistFromTerrain)
                    movement.y = terrainCentrum.y + terrainDimensions.y / 2 + maxDistFromTerrain - transform.position.y;
                if (transform.position.y + movement.y <= terrainCentrum.y - terrainDimensions.y / 2 + minDistFromTerrain)
                    movement.y = terrainCentrum.y + terrainDimensions.y / 2 + minDistFromTerrain - transform.position.y;
                if (transform.position.x + movement.x >= terrainCentrum.x + terrainDimensions.x / 2 + maxDistFromTerrain)
                    movement.x = terrainCentrum.x + terrainDimensions.x / 2 + maxDistFromTerrain - transform.position.x;
                if (transform.position.x + movement.x <= terrainCentrum.x - terrainDimensions.x / 2 - maxDistFromTerrain)
                    movement.x = terrainCentrum.x - terrainDimensions.x / 2 - maxDistFromTerrain - transform.position.x;
                if (transform.position.y <= waterPosition)
                    movement.y = waterPosition - transform.position.y;
                return movement;
            }
        else if (transform.position.y + movement.y < terrainHeightInNewPoint + minDistFromTerrain)
        {
            if (terrainHeightInNewPoint == 1.0 / 0.0)
                return movement;
            else
                return new Vector3(0, 0, 0);
        }
        else
            return movement;
    }

    public void dontMove()
    {
        canBeMoved = false;
    }
    public void canBeMove()
    {
        canBeMoved = true;
    }

    public void OnControllerColliderHit(Collision col)
    {
        print("OnControllerColliderHit");
    }

    public void OnCollisionEnter(Collision col)
    {
        print("OnCollsion");
    }

    public void OnCollisionStay(Collision col)
    {
        print("OnCollsionStay");
    }

    public void OnTriggerEnter()
    {
        print("OnTriggerEnter");
    }

}