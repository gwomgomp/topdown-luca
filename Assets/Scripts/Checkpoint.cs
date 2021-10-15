using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [field: SerializeField]
    public int Index { get; private set; }

    private MapController.Reached OnCheckpoint;

    private void OnTriggerEnter(Collider other) {
        if (OnCheckpoint != null && other.CompareTag("Player")) {
            OnCheckpoint(this);
        }
    }

    public void SetHandler(MapController.Reached OnCheckpoint)
    {
        this.OnCheckpoint = OnCheckpoint;
    }

    public void SetMaterial(Material material)
    {
        GetComponentInChildren<MeshRenderer>().material = material;
    }
}