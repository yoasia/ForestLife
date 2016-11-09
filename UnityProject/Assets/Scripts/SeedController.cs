using UnityEngine;
using System.Collections;

public class SeedController : MonoBehaviour
{

    public float Speed = 100.0F;
    public float RotationSpeed = 1.0F;

    public float HorizontalSway = 1F;
    public float VerticalSway = 1F;

    public MobileInput MobInput;
    public Camera SeedCamera;

    private Vector3 startingCameraPosition;

    // Use this for initialization
    void Start()
    {
        MobInput.ResetAxes();
        startingCameraPosition = SeedCamera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState == GameManager.GameState.GS_SEED)
        {
            var baseSpeed = Speed * 1.5F;

            var vertAxis = MobInput.GetAxis("Vertical");
            var horAxis = MobInput.GetAxis("Horizontal");
            if (Mathf.Abs(vertAxis) == 1)
                horAxis = 0;
            if (horAxis != 0)
                vertAxis = 0;
            float move = vertAxis * Speed + baseSpeed;
            float rotation = horAxis * RotationSpeed;

            Debug.LogFormat("Vertical: {0}", vertAxis);
            Debug.LogFormat("Horizontal: {0}", horAxis);

            move *= Time.deltaTime;
            rotation *= Time.deltaTime;

            GetComponent<Rigidbody>().AddRelativeForce(0, move, 0);
            GetComponent<Rigidbody>().AddTorque(0, rotation, 0);

            Vector3 newCameraPosition = new Vector3(startingCameraPosition.x - horAxis * HorizontalSway, startingCameraPosition.y - vertAxis * VerticalSway, startingCameraPosition.z);
            SeedCamera.transform.localPosition = newCameraPosition;

        }
    }
}
