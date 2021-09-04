using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject car;

    private Rigidbody carRigidBody;
    private Collider carCollider;
    private CarData carData;
    private Transform carBody;

    private static float tiltPower = 10;

    void Start()
    {
        carRigidBody = car.GetComponent<Rigidbody>();
        carData = car.GetComponent<CarData>();
        carBody = car.transform.Find("Body").transform;
    }

    void Update()
    {
        if (IsCarGrounded()) {
            HandleJumping();
            HandleAcceleration();
            HandleTurning();
        }
    }

    bool IsCarGrounded() {
        Vector3 raycastOrigin = car.transform.position;
        raycastOrigin.y += 0.5f;
        return Physics.Raycast(raycastOrigin, Vector3.down, 1);
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

    void HandleTurning() {
        float steering = Input.GetAxis("Horizontal");
        float percentageMaxSpeed = carRigidBody.velocity.magnitude / carData.MaxSpeed;
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
}
