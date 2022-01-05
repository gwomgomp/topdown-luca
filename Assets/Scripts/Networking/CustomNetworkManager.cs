using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCarMessage>(OnCreateCar);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        CreateCarMessage carMessage = new CreateCarMessage{};

        conn.Send(carMessage);
    }

    void OnCreateCar(NetworkConnection conn, CreateCarMessage message)
    {
        MapController map = FindMap();
        if (map != null) {
            GameObject car = Instantiate(playerPrefab, map.startingLine.transform.position, map.startingLine.transform.rotation);
            NetworkServer.AddPlayerForConnection(conn, car);
        } else {
            Debug.LogError("Couldn't load map");
        }
    }

    private MapController FindMap() {
        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects()) {
            MapController map = rootGameObject.GetComponentInChildren(typeof(MapController)) as MapController;
            if (map != null) {
                return map;
            }
        }
        return null;
    }
}

public struct CreateCarMessage : NetworkMessage
{
}