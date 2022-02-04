using UnityEngine;

public class GlobalInputHandler : MonoBehaviour
{
    [SerializeField]
    private float resetCooldown = 5;

    private float resetTimer = 0;

    void Update()
    {
        HandleEscape();
        HandleReset();
    }

    void HandleEscape() {
        if (MenuManager.CurrentMap != null && Input.GetButtonDown("Cancel")) {
            MenuManager menu = GameObject.FindObjectOfType<MenuManager>();
            menu.LoadMenu();
        }
    }

    
    void HandleReset() {
        bool hardReset = Input.GetKey(KeyCode.LeftShift);
        if ((hardReset || resetTimer <= 0) && Input.GetButtonDown("Reset")) {
            PlayerController player = GameObject.FindObjectOfType<PlayerController>();
            MapController map = GameObject.FindObjectOfType<MapController>();

            Checkpoint resetCheckpoint;
            if (hardReset) {
                map.InitializeCheckpoints();
                resetCheckpoint = map.startingLine;
            } else {
                resetCheckpoint = map.lastCheckpoint;
            }

            player.TriggerReset(resetCheckpoint, hardReset);
            if (hardReset) {
                ScoreController score = GameObject.FindObjectOfType<ScoreController>();
                score.ResetLap();
                GhostController ghost = GameObject.FindObjectOfType<GhostController>();
                ghost.RestartPlayback();
            }

            resetTimer = resetCooldown;
        } else if (resetTimer > 0) {
            resetTimer -= Time.deltaTime;
        }
    }
}
