using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject car;
    [SerializeField]
    GameObject cameraContainer;

    private Rigidbody carRigidBody;
    private Collider carCollider;
    private CarData carData;
    private Transform carBody;

    private float resetTimer = 0;
    private float defaultCameraHeight;


    [SerializeField]
    private float tiltPower = 10;
    
    [SerializeField]
    private float resetCooldown = 5;
    
    [SerializeField]
    private float maxZoomIncrease = 20;
    
    [SerializeField]
    private float zoomSpeed = 5;
    
    [SerializeField]
    private float cameraSpeed = 5;
    [SerializeField]
    private float lookAhead = 5;

    void Start()
    {
        carRigidBody = car.GetComponent<Rigidbody>();
        carData = car.GetComponent<CarData>();
        carBody = car.transform.Find("Body").transform;
        defaultCameraHeight = cameraContainer.transform.position.y;
    }

    void Update()
    {
        Vector3 driveDirection = new Vector3(carRigidBody.velocity.x, 0, carRigidBody.velocity.z); // disregard vertical movement
        float percentageMaxSpeed = Mathf.Clamp01(driveDirection.magnitude / carData.MaxSpeed);
        if (IsCarGrounded()) {
            HandleJumping();
            HandleAcceleration();
            HandleTurning(percentageMaxSpeed);
        }
        UpdateCamera(percentageMaxSpeed);
        HandleReset(percentageMaxSpeed);
    }

    bool IsCarGrounded() {
        Vector3 raycastOrigin = car.transform.position;
        raycastOrigin.y += 0.5f;
        return Physics.Raycast(raycastOrigin, car.transform.up * -1, 1);
    }

    void HandleJumping() {
        if (Input.GetButtonDown("Jump")) {
            carRigidBody.AddForce(Vector3.up * carData.JumpStrength, ForceMode.Impulse);
        }
    }

    void HandleAcceleration() {
        float acceleration = Input.GetAxis("Vertical");
        float breaking = Input.GetAxis("Breaking");
        if (breaking <= 0) {
            float power = acceleration > 0 ? carData.Acceleration : carData.ReverseAcceleration;
            carRigidBody.AddForce(car.transform.forward * acceleration * power * Time.deltaTime);
        } else if (carRigidBody.velocity.magnitude > 0.5) {
            carRigidBody.AddForce(carRigidBody.velocity.normalized * -1 * breaking * carData.BreakingPower * Time.deltaTime);
        }
        carRigidBody.velocity = Vector3.ClampMagnitude(carRigidBody.velocity, carData.MaxSpeed);
    }

    void HandleTurning(float percentageMaxSpeed) {
        float steering = Input.GetAxis("Horizontal");
        if (percentageMaxSpeed > 0.1) {
            float forward = Mathf.Sign(Vector3.Dot(car.transform.forward, carRigidBody.velocity.normalized));
            car.transform.Rotate(car.transform.up, steering * carData.TurnRate * Time.deltaTime * forward);
        }
        carBody.localEulerAngles = new Vector3(
            steering * tiltPower * percentageMaxSpeed,
            carBody.localEulerAngles.y,
            carBody.localEulerAngles.z
        );
    }

    /* the position of cameraContainer is always above the player
       the position of the actual camera is used for relative movement like lookahead and zoom
    */
    void UpdateCamera(float percentageMaxSpeed) {
        cameraContainer.transform.position = new Vector3(
            car.transform.position.x,
            defaultCameraHeight, // gotta adjust this if the track height ever changes
            car.transform.position.z
        );
        Vector3 lookAheadPosition = carRigidBody.velocity.normalized * lookAhead * percentageMaxSpeed;
        Vector3 newCameraPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, lookAheadPosition, Time.deltaTime * cameraSpeed);
        float newCameraHeight = maxZoomIncrease * percentageMaxSpeed;
        newCameraPosition.y = Mathf.MoveTowards(Camera.main.transform.localPosition.y, newCameraHeight, Time.deltaTime * zoomSpeed);
        Camera.main.transform.localPosition = newCameraPosition;
    }

    void HandleReset(float percentageMaxSpeed) {
        if (percentageMaxSpeed <= 0.1 && resetTimer <= 0 && Input.GetButtonDown("Reset")) {
            Vector3 newPosition = car.transform.position + new Vector3(0, 5, 0);
            Quaternion newRotation = Quaternion.Euler(0, car.transform.localEulerAngles.y, 0);
            car.transform.SetPositionAndRotation(newPosition, newRotation);
            resetTimer = resetCooldown;
        } else if (resetTimer > 0) {
            resetTimer -= Time.deltaTime;
        }
    }
}
