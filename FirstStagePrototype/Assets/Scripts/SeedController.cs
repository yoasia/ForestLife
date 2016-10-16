using UnityEngine;
using System.Collections;

public class SeedController : MonoBehaviour {

    public float Speed = 100.0F;
    public float RotationSpeed = 1.0F;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float move = Input.GetAxis("Vertical") * Speed;
        float rotation = Input.GetAxis("Horizontal") * RotationSpeed;

        //move *= Time.deltaTime;
        //rotation *= Time.deltaTime;

        GetComponent<Rigidbody>().AddRelativeForce(0, move, 0);
        GetComponent<Rigidbody>().AddTorque(0, rotation, 0);
    }
}
