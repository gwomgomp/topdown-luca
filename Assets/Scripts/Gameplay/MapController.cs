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

    [field: SerializeField]
    public Checkpoint startingLine { get; private set; }
    public Checkpoint lastCheckpoint { get; private set; }
    public Checkpoint nextCheckpoint { get; private set; }
    private Checkpoint[] checkpoints;

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
        for (int i = 0; i < checkpoints.Length; i++)
        {
            Checkpoint checkpoint = checkpoints[i];
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
                GetNextCheckpoint(reachedCheckpoint);
            }
        }
    }

    private void FinishLap() {
        OnLap();
        GetNextCheckpoint(startingLine);
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint != nextCheckpoint) {
                checkpoint.SetMaterial(inactiveMaterial);
            }
        }
    }

    private void GetNextCheckpoint(Checkpoint reachedCheckpoint) {
        nextCheckpoint = Array.Find(checkpoints, c => c.Index == reachedCheckpoint.Index + 1);
        if (nextCheckpoint == null) {
            int lowestIndex = int.MaxValue;
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.Index < lowestIndex) {
                    lowestIndex = checkpoint.Index;
                    nextCheckpoint = checkpoint;
                }
            }
        }
        nextCheckpoint.SetMaterial(targetMaterial);
    }
}
