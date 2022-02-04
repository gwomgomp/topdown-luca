using UnityEngine;
using System.Linq;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private Material reachedMaterial;
    [SerializeField]
    private Material targetMaterial;
    [SerializeField]
    private Material inactiveMaterial;

    [field: SerializeField]
    public Checkpoint startingLine { get; private set; }
    public Checkpoint lastCheckpoint { get; private set; }
    public Checkpoint nextCheckpoint { get; private set; }
    private Checkpoint[] checkpoints;
    private Checkpoint firstCheckpoint;

    public delegate void Reached(Checkpoint checkpoint);
    public static event Reached OnCheckpoint;

    public delegate void Lapped();
    public static event Lapped OnLap;

    private void Start() {
        OnCheckpoint += FindNextCheckpoint;
        checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        InitializeCheckpoints(true);
    }

    private void OnDestroy() {
        OnCheckpoint -= FindNextCheckpoint;
    }

    public void InitializeCheckpoints(bool setHandler = false) {
        foreach (var checkpoint in checkpoints) {
            checkpoint.SetMaterial(inactiveMaterial);
            if (setHandler) {
                checkpoint.SetHandler((checkpoint) => OnCheckpoint(checkpoint));
            }
        }
        startingLine.SetMaterial(targetMaterial);
        nextCheckpoint = startingLine;
    }

    private void FindNextCheckpoint(Checkpoint reachedCheckpoint)
    {
        if (reachedCheckpoint.Equals(nextCheckpoint)) {
            reachedCheckpoint.SetMaterial(reachedMaterial);
            lastCheckpoint = reachedCheckpoint;

            if (reachedCheckpoint.Equals(startingLine)) {
                FinishLap();
            } else {
                SetNextCheckpoint(reachedCheckpoint);
            }
        }
    }

    private void FinishLap() {
        OnLap();
        SetNextCheckpoint(startingLine);
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint != nextCheckpoint) {
                checkpoint.SetMaterial(inactiveMaterial);
            }
        }
    }

    private void SetNextCheckpoint(Checkpoint reachedCheckpoint) {
        nextCheckpoint = checkpoints.Where(c => c.Index == reachedCheckpoint.Index + 1)
            .DefaultIfEmpty(GetFirstCheckpoint())
            .First();
        nextCheckpoint.SetMaterial(targetMaterial);
    }

    private Checkpoint GetFirstCheckpoint() {
        if (firstCheckpoint == null) {
            firstCheckpoint = checkpoints.OrderBy(checkpoint => checkpoint.Index).First();
        }
        return firstCheckpoint;
    }
}
