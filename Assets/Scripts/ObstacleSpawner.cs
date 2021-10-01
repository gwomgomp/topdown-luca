using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [field: SerializeField]
    internal GameObject Obstacle { get; private set; }
    [field: SerializeField]
    internal int Count { get; private set; }
    [field: SerializeField]
    internal float Distance { get; private set; }
    [field: SerializeField]
    internal float SafeZone { get; private set; }

    void Start() {
        for (int i = 0; i < Count; i++) {
            float x = Random.Range(-Distance, Distance);
            float z = Random.Range(-Distance, Distance);
            Vector3 position = new Vector3(x, 0, z);
            if (position.magnitude <= SafeZone) {
                position = position.normalized * SafeZone;
            }
            position.y += 5;
            Instantiate(Obstacle, position, Random.rotationUniform);
        }
    }
}
