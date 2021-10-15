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

    private int completedLaps = -1;
    private float lapTime = 0;
    private float bestLap = float.MaxValue;

    private int crashes = 0;

    private void Start() {
        MapController.OnLap += HandleLap;
        CarData.OnCrash += HandleCrash;
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
    }

    private void HandleLap()
    {
        completedLaps += 1;
        if (completedLaps > 0 && lapTime < bestLap) {
            bestLap = lapTime;
        }
        lapTime = 0;
        crashes = 0;
    }

    private void HandleCrash() {
        crashes += 1;
    }
}
