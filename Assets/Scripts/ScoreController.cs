using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private Text lapCounter;
    [SerializeField]
    private Text timer;
    [SerializeField]
    private Text bestLapDisplay;
    [SerializeField]
    private Text crashDisplay;
    [SerializeField]
    private Text speedDisplay;
    [SerializeField]
    private Text bestSpeedDisplay;

    private int completedLaps = -1;
    private float lapTime = 0;
    private float bestLap = float.MaxValue;

    private int crashes = 0;

    private float currentSpeed = 0;
    private float bestSpeed = 0;

    private void Start() {
        MapController.OnLap += HandleLap;
        CarData.OnCollision += HandleCrash;
        PlayerController.OnSpeedChange += HandleSpeed;
    }

    private void Update()
    {
        if (completedLaps >= 0) {
            lapTime += Time.deltaTime;
            timer.text = string.Format("{0:N2}", lapTime);
        } else {
            timer.text = "-";
        }

        if (completedLaps > 0) {
            lapCounter.text = completedLaps.ToString();
            bestLapDisplay.text = string.Format("{0:N2}", bestLap);
        } else {
            lapCounter.text = "-";
            bestLapDisplay.text = "-";
        }

        crashDisplay.text = crashes.ToString();
        speedDisplay.text = string.Format("{0:N2}", currentSpeed);
        bestSpeedDisplay.text = string.Format("{0:N2}", bestSpeed);
    }

    private void HandleLap()
    {
        completedLaps += 1;
        if (completedLaps > 0 && lapTime < bestLap) {
            bestLap = lapTime;
        }
        lapTime = 0;
        crashes = 0;
        bestSpeed = 0;
    }

    private void HandleCrash(Collision collision) {
        crashes += 1;
    }

    private void HandleSpeed(float speed) {
        currentSpeed = speed;
        if (speed > bestSpeed) {
            bestSpeed = speed;
        }
    }
}
