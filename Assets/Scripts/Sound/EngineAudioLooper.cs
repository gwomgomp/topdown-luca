using UnityEngine;

public class EngineAudioLooper : MonoBehaviour
{
    public float minimumPitch = 0.5f;

    public float distanceMultiplier = 45;
    public float speedToPitchRatio = 7.5f;

    public bool isListener = false;
    public float listenerVolume = 0.05f;
    public float nonListenerVolume = 0.15f;

    private AudioSource audioSource;
    private float pitch;
    private float newPitch;

    private Vector3 lastPosition;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        if (isListener) {
            audioSource.volume = listenerVolume;
        } else {
            audioSource.volume = nonListenerVolume;
        }
        pitch = minimumPitch;
        newPitch = minimumPitch;
    }

    void Update() {
        Vector3 currentPosition = GetComponentInParent<Transform>().position;
        if (lastPosition != null) {
            HandleSpeed(Vector3.Distance(lastPosition, currentPosition) * distanceMultiplier);
        }
        lastPosition = currentPosition;

        float lerpMultiplier = pitch > newPitch ? 2 : 1;
        pitch = Mathf.Lerp(pitch, newPitch, Time.deltaTime * lerpMultiplier);
        audioSource.pitch = pitch;
    }

    private void HandleSpeed(float speed) {
        if (speed > 0 && speed < 100) { // any speed that is strangely high is probably an artifact from moving cars by code, ignore them
            newPitch = speed / speedToPitchRatio;
            newPitch = newPitch > minimumPitch ? newPitch : minimumPitch;
        }
    }
}
