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

    private int completedLaps = -1;
    private float lapTime = 0;
    private float bestLap = float.MaxValue;

    private void Start() {
        MapController.OnLap += HandleLap;
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
    }

    private void HandleLap()
    {
        completedLaps += 1;
        if (completedLaps > 0 && lapTime < bestLap) {
            bestLap = lapTime;
        }
        lapTime = 0;
    }
}
