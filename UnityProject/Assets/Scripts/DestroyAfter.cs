using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

    public int time = 30;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator destroyAfterTime()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
