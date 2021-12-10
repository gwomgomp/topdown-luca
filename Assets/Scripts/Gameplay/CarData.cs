using UnityEngine;

class CarData : MonoBehaviour
{
    [field: SerializeField]
    public float Acceleration { get; private set; }
    [field: SerializeField]
    public float ReverseAcceleration { get; private set; }
    [field: SerializeField]
    public float BreakingPower { get; private set; }
    [field: SerializeField]
    public float TurnRate { get; private set; }
    [field: SerializeField]
    public float MaxSpeed { get; private set; }
    [field: SerializeField]
    public float BoostDecay { get; private set; }
    [field: SerializeField]
    public float BoostAcceleration { get; private set; }
    [field: SerializeField]
    public float BoostMaxSpeedMultiplier { get; private set; }
    [field: SerializeField]
    public float JumpStrength { get; private set; }

    public delegate void Collided(Collision collision);
    public static event Collided OnCollision;

    public delegate void Boosted(bool inBooster);
    public static event Boosted OnBoost;

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Ground")) {
            OnCollision(collision);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Booster")) {
            OnBoost(true);
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.CompareTag("Booster")) {
            OnBoost(false);
        }
    }
}