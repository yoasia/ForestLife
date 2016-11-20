using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

    public ParticleSystem rain;
    Vector3 scale = new Vector3(0.1F, 0.05F, 0.1F);
    public float speed = 10.0f;
    //public Vector3 maxScale = new Vector3(70.0F, 70.0F, 70.0F);
    float lifeTime = 35.0f;
    float startRainTime = 5.0f;
    float EndRainTime = 15.0f;
    float scaleTime = 5.0f;

    // Use this for initialization
    void Start () {
        rain.Stop();
        StartCoroutine(StartRainAfterTime(lifeTime/3.0f));
        StartCoroutine(StopRainAfterTime(3.0f*lifeTime/4.0f));
        //StartCoroutine(DontScaleAfterTime(scaleTime));
        StartCoroutine(ChangeScaleAfterTime(lifeTime/2.0f));
        StartCoroutine(DestroyAfterTime(lifeTime));

    }

    // Update is called once per frame
    void Update () {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        transform.localScale += scale;
	}

    void StartRain ()
    {
        rain.Play();
    }
    void StopRain()
    {
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
        DestroyObject(gameObject);
    }
}
