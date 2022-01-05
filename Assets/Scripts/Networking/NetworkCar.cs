using Mirror;
using UnityEngine.SceneManagement;

public class NetworkCar : NetworkBehaviour
{
    public override void OnStartLocalPlayer() {
        FindPlayerController().SetCar(gameObject);
    }

    private PlayerController FindPlayerController() {
        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects()) {
            PlayerController player = rootGameObject.GetComponentInChildren(typeof(PlayerController)) as PlayerController;
            if (player != null) {
                return player;
            }
        }
        return null;
    }
}
