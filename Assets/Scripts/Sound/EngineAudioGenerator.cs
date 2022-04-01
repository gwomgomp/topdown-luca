using UnityEngine;

public class EngineAudioGenerator : MonoBehaviour
{
    public float baseFrequency = 50;
    public float minimumPitch = 0.5f;

    public float distanceMultiplier = 45;

    public bool isListener = false;
    public float listenerVolume = 0.005f;
    public float nonListenerVolume = 0.05f;

    private AudioSource audioSource;
    private float pitch;
    private float newPitch;
    private int time = 0;

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

        pitch = Mathf.Lerp(pitch, newPitch, Time.deltaTime);
        audioSource.pitch = pitch;
    }

    void OnAudioFilterRead(float[] data, int channels) {
        for (int i = 0; i < data.Length; i += channels) {
            data[i] = CreateWave(time, 44100);
           
            if(channels == 2) {
                data[i+1] = CreateWave(time, 44100);
            }

            time++;
        }
    }

    private void HandleSpeed(float speed) {
        if (speed > 0 && speed < 100) { // any speed that is strangely high is probably an artifact from moving cars by code, ignore them
            newPitch = speed;
            newPitch = newPitch > minimumPitch ? newPitch : minimumPitch;
        }
    }

    private float CreateWave(int timeIndex, float sampleRate)
    {
        float sin = Mathf.Sin(2 * Mathf.PI * timeIndex * baseFrequency / sampleRate);
        return sin > 0 ? 1 : 0;
    }
}
