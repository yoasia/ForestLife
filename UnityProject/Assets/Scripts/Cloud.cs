using UnityEngine;
using System.Collections;
using System;

public class Cloud : MonoBehaviour {

    public ParticleSystem rain;
    public float speed = 5.0f;
    public float maxLifeTime = 60.0f;
    public Vector3 maxScale = new Vector3(70, 35, 70);
    //float startRainTime = 5.0f;
    //float endRainTime = 29.0f;
    //float scaleTime = 5.0f;

    Vector3 scale = new Vector3(0.1F, 0.05F, 0.1F);
    Vector3 startScale = new Vector3(0.1F, 0.05F, 0.1F);
    bool ifItRains = false;
    bool stopRain = false;
    bool ifScale = true;
    bool canBeDestroyed = false;

    //Accelerator variable
    static float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
    static float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    private Vector3 startPosition = new Vector3(0, 0, 0);
    private float scalingTime = 20.0f;
    private float lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
    private float timer = 0;
    private float scalingTimer = 0;
    private Vector3 lowPassValue = Vector3.zero;
    private Vector3 acceleration;
    private Vector3 deltaAcceleration;


    void Start() {
        rain.Stop();
        StartCoroutine(ChangeScaleAfterTime(maxLifeTime / 3.0f));
        StartCoroutine(DestroyAfterTime(maxLifeTime));

        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;

        //obrót wg kierunku wiatru
        Vector3 relativePos = new Vector3(GameManager.Wind.x, 0, GameManager.Wind.y);
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        //rotation = new Quaternion(0, rotation.y, 0, rotation.w);
        transform.rotation = rotation;
        transform.Rotate(0, -90, 0);


        //obliczanie początkowej pozycji chmóry w zależności od kierunku wiatru
        //(Chmurka zawsze musi przelecieć nad wyspą)
        float terrainPositionx = Terrain.activeTerrain.transform.position.x;
        float terrainPositiony = Terrain.activeTerrain.transform.position.z;

        Vector3 terrainDimension = Terrain.activeTerrain.terrainData.size;


        //środek terenu
        float terrainCentrumX = terrainPositionx + terrainDimension.x / 2.0f;
        float terrainCentrumY = terrainPositiony + terrainDimension.z / 2.0f;

        //wysokość pozostawiamy taką samą
        startPosition.y = transform.position.y;

        //parametry równania kwadratowego
        //float a, b, c;
        //a = 1 + terrainCentrumY * terrainCentrumY;
        //b = -2 * (terrainCentrumX + terrainCentrumY * (terrainCentrumY / terrainCentrumX));
        //c = terrainCentrumX * terrainCentrumX + terrainCentrumY * terrainCentrumY - 500 * 500;


        //float delta = b * b - 4 * a * c;
        ////obliczanie punktów wspólnych prostej wiatru i okręgu (wyspy)

        //double x1 = (-b + Math.Pow(delta, 0.5)) / (2 * c);
        //double x2 = (-b - Math.Pow(delta, 0.5)) / (2 * c);

        //double xBigger, xSmaller, yBigger, ySmaller;

        //if (x1 > x2) {
        //    xBigger = x1;
        //    xSmaller = x2;

        //    yBigger = (terrainCentrumY / terrainCentrumX) * xBigger;
        //    ySmaller = (terrainCentrumY / terrainCentrumX) * xSmaller;

        //}
        //else {
        //    xBigger = x2;
        //    xSmaller = x1;

        //    yBigger = (terrainCentrumY / terrainCentrumX) * xBigger;
        //    ySmaller = (terrainCentrumY / terrainCentrumX) * xSmaller;
        //}



        //if (GameManager.Wind.x > 0)
        //    startPosition.x = (float)xSmaller;
        //else
        //    startPosition.x = (float)xBigger;

        //if (GameManager.Wind.y > 0)
        //    startPosition.y = (float)ySmaller;
        //else
        //    startPosition.y = (float)yBigger;


        //    //przemieszczenie chmury tak by przelatywała nad wyspą
        //double x = Math.Pow((500.0 * 500.0) / (1 + Math.Pow((double)GameManager.Wind.y / GameManager.Wind.x, 2)), 0.5);
        //double y = Math.Pow((500.0 * 500.0) - x * x, 0.5);

        //if (GameManager.Wind.x > 0 )
        //    startPosition.x = -(float)x;
        //else
        //    startPosition.x = (float)x;

        //if (GameManager.Wind.y > 0)
        //    startPosition.z = -(float)y;
        //else
        //    startPosition.z = (float)y;
        setInvisible(false);

        //pozycja początkowa
        startPosition.x = terrainCentrumX -( GameManager.Wind.x * 500);
        startPosition.z = terrainCentrumY - (GameManager.Wind.y * 500);
        transform.position = startPosition;
        
    }

    void Update () {

        transform.Translate(new Vector3(GameManager.Wind.x, 0, GameManager.Wind.y) * speed * Time.deltaTime, Space.World);
        timer += Time.deltaTime;

        //cloud scaling
        if (ifScale)
        {
            Scale();
        }
        //jeżeli miną czas skalowania
        if (timer >= scalingTime)
        {
            scalingTimer = 0;
            ifScale = false;
        }
        shakeDetection();
        
    }

    void shakeDetection ()
    {
        acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here, with suitable guards in the if check above, if necessary to not, to not fire again if they're already being performed.
            Debug.Log("Shake event detected at time " + Time.time);
            if(!stopRain)
                StartRain();
        }
    }

    void Scale()
    {
        transform.localScale += scale;

        //odmierzamy ile czasu minęło od rozpoczęcia skalowania
        scalingTimer += Time.deltaTime;

        if (transform.localScale.x <= 0)
            ifScale = false;
        else if (transform.localScale.x >= maxScale.x || 
            transform.localScale.y >= maxScale.y ||
            transform.localScale.z >= maxScale.z)
            ifScale = false;
        
    }

    void StartRain ()
    {
        ifItRains = true;
        rain.Play();
        StartCoroutine(StartIrrigating(1, 3));
    }
    void StopRain()
    {
        ifItRains = false;
        rain.Stop();
    }

    IEnumerator StartRainAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        StartRain();  
    }
    IEnumerator StopRainAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        stopRain = true;
        StopRain();
    }
    IEnumerator DontScaleAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        scale = new Vector3(0.0F, 0.0F, 0.0F);
    }
    IEnumerator ChangeScaleAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        scale = new Vector3(0, 0, 0);
    }
    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        scale = -startScale;
        ifScale = true;
        scalingTime = timer + scalingTime;
        //destroy rain after 5 s
        StartCoroutine(DestroyParticleAfterTime(scalingTime - 3));
        
        //set cloud invisible 
        setInvisible(true);
        //destroy cloud object
        StartCoroutine(DestroyCloudAfterTime(scalingTime));

    }

    void setInvisible (bool trueOrFalse)
    {
        if(trueOrFalse)
        {
            Color color = GetComponent<Renderer>().material.color;
            color.a = 0.0f;
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
        else
        {
            Color color = GetComponent<Renderer>().material.color;
            color.a = 1.0f;
            GetComponent<Renderer>().material.SetColor("_Color", color);
        } 
     }

    IEnumerator DestroyParticleAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        //unchild the particle system from object
        //rain.transform.parent = null;

        DestroyObject(rain);
    }

    IEnumerator DestroyCloudAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyObject(gameObject);
    }
    IEnumerator StartIrrigating(int strength, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            if(ifItRains)
            {
                Vector3 dimensionOfCloud = GetComponent<Renderer>().bounds.size;

                int x1 = (int) transform.position.x;
                int z1 = (int)transform.position.z;
                int x2 = x1 + (int) dimensionOfCloud.x;
                int z2 = z1 + (int) dimensionOfCloud.z;

                GameManager.instance.terrainManager.WaterArea(x1, z1, x2, z2, strength);
            }
        }
    }

   

}
