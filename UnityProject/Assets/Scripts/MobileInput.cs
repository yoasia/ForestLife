using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public float verticalDeadzone = 15F;
    public float horizontalDeadzone = 10F;

    public float maximumRoll = 85f;
    public float maximumPitch = 30f;

    private float zeroRoll;
    private float zeroPitch;

    private Vector3 currentAcc;


    void Update()
    {
        currentAcc = GetAcceleration();
    }

    public float GetAxis(string axis)
    {
        if (currentAcc != Vector3.zero)
        {
            Debug.LogFormat("x: {0}", Input.acceleration.x);
            Debug.LogFormat("y: {0}", Input.acceleration.y);
            Debug.LogFormat("z: {0}", Input.acceleration.z);
            float angle;
            switch (axis)
            {
                case "Vertical":
                    angle = GetPitch(currentAcc) - zeroPitch;
                    if (Mathf.Abs(angle) < verticalDeadzone)
                        angle = 0;
                    return Mathf.InverseLerp(-maximumPitch, maximumPitch, angle) * 2 - 1;
                case "Horizontal":
                    angle = GetRoll(currentAcc) - zeroRoll;
                    if (Mathf.Abs(angle) < horizontalDeadzone)
                        angle = 0;
                    return Mathf.InverseLerp(-maximumRoll, maximumRoll, angle) * 2 - 1;
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

    public void ResetAxes()
    {
        zeroPitch = GetPitch(GetAcceleration());
        zeroRoll = 0;
    }

    private float GetRoll(Vector3 acceleration)
    {
        float miu = 0.001f;

        int sign;
        float angle;
        if (Input.acceleration.y > 0)
            sign = -1;
        else
            sign = 1;

        var tmp = Mathf.Sqrt(acceleration.y * acceleration.y + miu * acceleration.z * acceleration.z);
        angle = Mathf.Atan2(acceleration.x, sign * tmp) * Mathf.Rad2Deg;
        Debug.LogFormat("Horizontal: {0}", angle);

        return angle;
    }

    private float GetPitch(Vector3 acceleration)
    {
        float angle;

        var tmp = Mathf.Sqrt(acceleration.x * acceleration.x + acceleration.y * acceleration.y);
        angle = Mathf.Atan2(-acceleration.z, tmp) * Mathf.Rad2Deg;
        Debug.LogFormat("Vertical: {0}", angle);

        return angle;
    }

    private Vector3 GetAcceleration()
    {
        float period = 0.0F;
        Vector3 acc = Vector3.zero;
        AccelerationEvent accEvent;
        for (int i = 0; i < Input.accelerationEventCount; i++)
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

