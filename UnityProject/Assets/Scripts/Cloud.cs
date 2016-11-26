using UnityEngine;
using System.Collections; 

public class Cloud : MonoBehaviour {

    public ParticleSystem rain;
    Vector3 scale = new Vector3(0.1F, 0.05F, 0.1F);
    public float speed = 10.0f;
    //public Vector3 maxScale = new Vector3(70.0F, 70.0F, 70.0F);
    float lifeTime = 30.0f;
    float startRainTime = 5.0f;
    float endRainTime = 29.0f;
    float scaleTime = 5.0f;
    bool ifItRains = false;
    bool stopRain = false;
    bool ifScale = true;


    //Accelerator variable
    static float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
    static float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    private float lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
    private Vector3 lowPassValue = Vector3.zero;
    private Vector3 acceleration;
    private Vector3 deltaAcceleration;


    void Start () {
        rain.Stop();

        //StartCoroutine(StartRainAfterTime(lifeTime/3.0f));
        //StartCoroutine(StopRainAfterTime(3.0f*lifeTime/4.0f));
        ////StartCoroutine(DontScaleAfterTime(scaleTime));
        //StartCoroutine(ChangeScaleAfterTime(lifeTime/2.0f));
        //StartCoroutine(DestroyAfterTime(lifeTime));

        //StartCoroutine(StartRainAfterTime(startRainTime));
        StartCoroutine(StopRainAfterTime(endRainTime - 1));
        //StartCoroutine(DontScaleAfterTime(scaleTime));
        StartCoroutine(ChangeScaleAfterTime(lifeTime / 2.0f));
        StartCoroutine(DestroyAfterTime(lifeTime));

        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    void Update () {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        //cloud scaling
        Scale();
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
        if (ifScale)
        {
            transform.localScale += scale;
            if (transform.localScale.x <= 0)
                ifScale = false;
        }
    }

    void StartRain ()
    {
        ifItRains = true;
        rain.Play();
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
        StartCoroutine(StartIrrigating(1, 3));
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
        scale = -scale;
    }
    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        //unchild the particle system from object
        rain.transform.parent = null;

        //destroy rain after 5 s
        StartCoroutine(DestroyParticleAfterTime(3));


        //set cloud invisible 
        setInvisible();
         //destroy cloud object
         StartCoroutine(DestroyCloudAfterTime(5));

    }

    void setInvisible ()
    {
        Color color = GetComponent<Renderer>().material.color;
        color.a = 0.0f;
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    IEnumerator DestroyParticleAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
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
