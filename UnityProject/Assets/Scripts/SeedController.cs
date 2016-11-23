using UnityEngine;

public class SeedController : MonoBehaviour
{
    public string species;

    public float Speed = 100.0F;
    public float RotationSpeed = 1.0F;

    public float HorizontalSway = 1F;
    public float VerticalSway = 1F;

    public MobileInput MobInput;
    public Camera SeedCamera;

    private Vector3 startingCameraPosition;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        MobInput.ResetAxes();
        startingCameraPosition = SeedCamera.transform.localPosition;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Reset()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rigidBody.velocity = new Vector3(0f, 0f, 0f);
        rigidBody.angularVelocity = new Vector3(0f, 0f, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Terrain")
        {
            var position = collision.contacts[0].point;
            if (GameManager.instance.seedLanding(position.x, position.z, species))
            {
                Destroy(gameObject);
                return;
            }
        }

        GameManager.instance.OnBadLandingPopup();
    }

    void FixedUpdate()
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

        //Debug.LogFormat("Vertical: {0}", vertAxis);
        //Debug.LogFormat("Horizontal: {0}", horAxis);

            move *= Time.deltaTime;
            rotation *= Time.deltaTime;

            rigidBody.AddRelativeForce(0, move, 0);
            rigidBody.AddTorque(0, rotation, 0);

            Vector3 newCameraPosition = new Vector3(startingCameraPosition.x - horAxis * HorizontalSway, startingCameraPosition.y - vertAxis * VerticalSway, startingCameraPosition.z);
            SeedCamera.transform.localPosition = newCameraPosition;

            if(transform.position.y < TerrainManager.instance.water.transform.position.y)
                GameManager.instance.OnBadLandingPopup();
        }
    }
}
