using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [field: SerializeField]
    internal GameObject Obstacle { get; private set; }
    [field: SerializeField]
    internal int Count { get; private set; }
    [field: SerializeField]
    internal float Distance { get; private set; }

    void Start() {
        for (int i = 0; i < Count; i++) {
            float x = Random.Range(-Distance, Distance);
            float z = Random.Range(-Distance, Distance);
            Vector3 position = new Vector3(x, 0, z);
            if (position.magnitude <= 10) {
                position = position.normalized * 10;
            }
            position.y += 5;
            Instantiate(Obstacle, position, Random.rotationUniform);
        }
    }
}
