using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField]
    private GameObject ghostCarPrefab;

    [SerializeField]
    private float recordingPointWait;

    private PlayerController playerController;
    private GameObject ghostCar;

    private List<(Vector3, Quaternion)> playbackGhostPositions = new List<(Vector3, Quaternion)>();

    private List<(Vector3, Quaternion)> lastRecordedGhostPositions = new List<(Vector3, Quaternion)>();
    private List<(Vector3, Quaternion)> recordingGhostPositions = new List<(Vector3, Quaternion)>();

    private bool recording = false;
    private float lastRecordTime;

    private float playbackStartTime;

    private void Start() {
        playerController = GameObject.FindObjectOfType<PlayerController>();
        MapController.OnLap += StartGhostRecording;
        ghostCar = Instantiate(ghostCarPrefab);
    }

    private void OnDestroy() {
        MapController.OnLap -= StartGhostRecording;
    }

    private void StartGhostRecording()
    {
        recording = false;
        lastRecordedGhostPositions = recordingGhostPositions;
        recordingGhostPositions = new List<(Vector3, Quaternion)>();
        recording = true;
        RecordCurrentTimestamp();
        lastRecordTime = Time.time;
    }

    void Update()
    {
        if (recording && lastRecordTime + recordingPointWait < Time.time) {
            RecordCurrentTimestamp();
        }
        if (playbackGhostPositions.Count > 0) {
            ghostCar.SetActive(true);
            MoveGhost();
        } else {
            ghostCar.SetActive(false);
        }
    }

    internal void RestartPlayback()
    {
        playbackStartTime = Time.time;
    }

    internal void PlayGhost(List<(Vector3, Quaternion)> ghostValues)
    {
        playbackGhostPositions = ghostValues;
        playbackStartTime = Time.time;
    }

    public List<(Vector3, Quaternion)> GetLastRecording() {
        return lastRecordedGhostPositions;
    }

    private void RecordCurrentTimestamp()
    {
        recordingGhostPositions.Add(playerController.GetCurrentPosition());
        lastRecordTime = Time.time;
    }

    private void MoveGhost()
    {
        float currentReplayTime = Time.time - playbackStartTime;
        int currentReplayPoint = ((int) (currentReplayTime / recordingPointWait)) % (playbackGhostPositions.Count - 1);
        int nextReplayPoint = currentReplayPoint + 1 >= playbackGhostPositions.Count - 1 ? 0 : currentReplayPoint + 1;
        float currentLerp = ((currentReplayTime - recordingPointWait * currentReplayPoint) / recordingPointWait) % 1;
        (Vector3 currentPosition, Quaternion currentRotation) = playbackGhostPositions[currentReplayPoint];
        (Vector3 nextPosition, Quaternion nextRotation) = playbackGhostPositions[nextReplayPoint];
        Vector3 newPosition = Vector3.Lerp(currentPosition, nextPosition, currentLerp);
        Quaternion newRotation = Quaternion.Lerp(currentRotation, nextRotation, currentLerp);
        ghostCar.transform.SetPositionAndRotation(newPosition, newRotation);
    }
}
