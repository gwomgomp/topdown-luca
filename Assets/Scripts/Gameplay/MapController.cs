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
    private Checkpoint[] checkpoints;
    private Checkpoint nextCheckpoint;

    public delegate void Reached(Checkpoint checkpoint);
    public static event Reached OnCheckpoint;

    public delegate void Lapped();
    public static event Lapped OnLap;

    private void Start() {
        OnCheckpoint += FindNextCheckpoint;
        startingLine.SetHandler(OnCheckpoint);
        startingLine.SetMaterial(targetMaterial);
        nextCheckpoint = startingLine;

        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpoints = new Checkpoint[checkpointObjects.Length];
        for (int i = 0; i < checkpointObjects.Length; i++)
        {
            Checkpoint checkpoint = checkpointObjects[i].GetComponent<Checkpoint>();
            checkpoints[i] = checkpoint;
            checkpoint.SetHandler((checkpoint) => OnCheckpoint(checkpoint));
        }
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
