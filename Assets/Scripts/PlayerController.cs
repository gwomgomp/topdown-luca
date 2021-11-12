using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject carPrefab;
    [SerializeField]
    private GameObject cameraContainer;
    [SerializeField]
    private MapController map;
    
    private GameObject car;
    private Rigidbody carRigidBody;
    private CarData carData;
    private Transform carBody;

    private float resetTimer = 0;
    private float defaultCameraHeight;

    private float currentMaxSpeed;
    private bool playerIsInBooster = false;

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

    private bool initialized = false;

    public delegate void SpeedChanged(float speed);
    public static event SpeedChanged OnSpeedChange;

    void Start()
    {
        if (map != null) {
            SpawnCar(map);
        }
    }

    void Update()
    {
        if (!initialized) {
            return;
        }

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

    public void SpawnCar(MapController map) {
        if (initialized) {
            return;
        }

        car = Instantiate(carPrefab, map.startingLine.transform.position, map.startingLine.transform.rotation);
        car.transform.Translate(Vector3.back * 5);
        carRigidBody = car.GetComponent<Rigidbody>();
        carData = car.GetComponent<CarData>();
        currentMaxSpeed = carData.MaxSpeed;
        CarData.OnBoost += HandleBooster;
        carBody = car.transform.Find("Body").transform;
        defaultCameraHeight = cameraContainer.transform.position.y;
        initialized = true;
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
        if (playerIsInBooster) {
            acceleration = carData.BoostAcceleration;
        }
        float breaking = Input.GetAxis("Breaking");

        Vector3 direction = Vector3.zero;
        float power = 0;
        if (breaking > 0) {
            direction = carRigidBody.velocity.normalized * -1;
            power = breaking * carData.BreakingPower;
        } else if (acceleration > 0) {
            direction = car.transform.forward;
            power = Mathf.Abs(acceleration) * carData.Acceleration;
        } else if (acceleration < 0) {
            direction = car.transform.forward * -1;
            power = Mathf.Abs(acceleration) * carData.ReverseAcceleration;
        }

        carRigidBody.AddForce(direction * power * Time.deltaTime);
        if (playerIsInBooster) {
            currentMaxSpeed = carData.MaxSpeed * carData.BoostMaxSpeedMultiplier;
        } else if (currentMaxSpeed > carData.MaxSpeed) {
            currentMaxSpeed = Mathf.MoveTowards(currentMaxSpeed, carData.MaxSpeed, carData.BoostDecay * Time.deltaTime);
        }
        carRigidBody.velocity = Vector3.ClampMagnitude(carRigidBody.velocity, currentMaxSpeed);
        OnSpeedChange(carRigidBody.velocity.magnitude);
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
            if (map.lastCheckpoint != null) {
                newPosition = map.lastCheckpoint.transform.position;
                newRotation = map.lastCheckpoint.transform.rotation;
            }
            car.transform.SetPositionAndRotation(newPosition, newRotation);
            resetTimer = resetCooldown;
        } else if (resetTimer > 0) {
            resetTimer -= Time.deltaTime;
        }
    }

    private void HandleBooster(bool inBooster) {
        playerIsInBooster = inBooster;
    }
}
