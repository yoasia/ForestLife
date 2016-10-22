using UnityEngine;
using System.Collections;

public class SeedController : MonoBehaviour {

    public float Speed = 100.0F;
    public float RotationSpeed = 1.0F;

    public float horizontalSensitivity = 1.0F;
    public float verticalSensitivity = 1.0F;
    public float smoothness = 0.5F;

    public float verticalDeadzone = 0.15F;
    public float horizontalDeadzone = 0.15F;

    private Vector3 currentAcc;
    private Vector3 referenceAcc;

    // Use this for initialization
    void Start () {
        ResetAxes();
	}
	
	// Update is called once per frame
	void Update () {
        currentAcc = Vector3.Lerp(currentAcc, GetAcceleration() - referenceAcc, Time.deltaTime/smoothness);

        var vertAxis = GetAxis("Vertical");
        if (Mathf.Abs(vertAxis) < verticalDeadzone)
            vertAxis = 0;
        var horAxis = GetAxis("Horizontal");
        if (Mathf.Abs(horAxis) < horizontalDeadzone)
            horAxis = 0;
        float move = vertAxis * Speed;
        float rotation = horAxis * RotationSpeed;

        Debug.LogFormat("Vertical: {0}", GetAxis("Vertical"));
        Debug.LogFormat("Horizontal: {0}", GetAxis("Horizontal"));

        move *= Time.deltaTime;
        rotation *= Time.deltaTime;

        GetComponent<Rigidbody>().AddRelativeForce(0, move, 0);
        GetComponent<Rigidbody>().AddTorque(0, rotation, 0);
    }

    private float GetAxis(string axis)
    {
        if (Input.acceleration != Vector3.zero)
        {
            switch (axis)
            {
                case "Vertical":
                    return Mathf.Clamp(currentAcc.y * verticalSensitivity, -1, 1);
                case "Horizontal":
                    return Mathf.Clamp(currentAcc.x * horizontalSensitivity, -1, 1);
                default:
                    return 0;
            }
        }
        switch (axis)
        {
            case "Vertical":
                return Input.GetAxis("Vertical");
            case "Horizontal":
                return Input.GetAxis("Horizontal");
            default:
                return 0;
        }
    }

    private void ResetAxes()
    {
        referenceAcc = GetAcceleration();
        currentAcc = Vector3.zero;
    }

    private Vector3 GetAcceleration()
    {
        float period = 0.0F;
        Vector3 acc = Vector3.zero;
        AccelerationEvent accEvent;
        for(int i = 0; i < Input.accelerationEventCount; i++)
        {
            accEvent = Input.accelerationEvents[i];
            acc += accEvent.acceleration * accEvent.deltaTime;
            period += accEvent.deltaTime;
        }
        if (period > 0)
            acc /= period;
        return acc;
    }
}
