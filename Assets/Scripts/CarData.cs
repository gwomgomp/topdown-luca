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
    public float JumpStrength { get; private set; }

    public delegate void Crashed();
    public static event Crashed OnCrash;

    private void OnCollisionEnter(Collision other) {
        if (!other.gameObject.CompareTag("Ground")) {
            OnCrash();
        }
    }
}