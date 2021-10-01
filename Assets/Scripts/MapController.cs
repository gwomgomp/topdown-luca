using System;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private Material reachedMaterial;
    [SerializeField]
    private Material targetMaterial;
    [SerializeField]
    private Material inactiveMaterial;

    public Checkpoint startingLine;
    [SerializeField]
    private Checkpoint[] checkpoints;

    public Checkpoint lastCheckpoint { get; private set; }
    private Checkpoint nextCheckpoint = null;

    public delegate void Reached(Checkpoint checkpoint);
    public static event Reached OnCheckpoint;

    public delegate void Lapped();
    public static event Lapped OnLap;

    private void Start() {
        OnCheckpoint += FindNextCheckpoint;
        startingLine.SetHandler(OnCheckpoint);
        startingLine.SetMaterial(targetMaterial);
        nextCheckpoint = startingLine;
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetHandler(OnCheckpoint);
        }
    }

    private void FindNextCheckpoint(Checkpoint reachedCheckpoint)
    {
        if (reachedCheckpoint.Equals(nextCheckpoint)) {
            reachedCheckpoint.SetMaterial(reachedMaterial);

            if (reachedCheckpoint.Equals(startingLine)) {
                FinishLap();
            } else {
                GetNextCheckpoint(reachedCheckpoint);
            }
            lastCheckpoint = reachedCheckpoint;
            nextCheckpoint.SetMaterial(targetMaterial);
        }
    }

    private void FinishLap() {
        OnLap();
        nextCheckpoint = checkpoints[0];
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetMaterial(inactiveMaterial);
        }
    }

    private void GetNextCheckpoint(Checkpoint reachedCheckpoint) {
        int index = Array.IndexOf(checkpoints, reachedCheckpoint);
        if (index + 1 >= checkpoints.Length) {
            nextCheckpoint = startingLine;
        } else {
            nextCheckpoint = checkpoints[index + 1];
        }
    }
}
