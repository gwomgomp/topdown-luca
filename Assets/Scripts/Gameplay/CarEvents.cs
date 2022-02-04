using UnityEngine;

class CarEvents : MonoBehaviour
{
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