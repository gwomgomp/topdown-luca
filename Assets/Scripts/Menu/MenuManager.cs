using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    [Serializable]
    public struct MapDefinition {
        public string name;
        public string mapId;
    }
    public MapDefinition[] mapDefinitions;

    public GameObject buttonPrefab;
    public float horizontalButtonDistance;
    public float verticalButtonDistance;

    private CarSelector carSelector;

    public static string CurrentMap { get; private set; }

    void Update() {
        if (CurrentMap != null && Input.GetButtonDown("Cancel")) {
            LoadMenu();
        }
    }

    [SerializeField]
    private GameObject gameLogic;

    public void LoadMenu() {
        CurrentMap = null;
        StartCoroutine(LoadMenuAsync());
    }

    IEnumerator LoadMenuAsync()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentScene);

        Scene menuScene = SceneManager.GetActiveScene();
        Canvas canvas = FindComponent<Canvas>(menuScene);
        createMapButtons(canvas);

        carSelector = FindComponent<CarSelector>(menuScene);
    }

    public void createMapButtons(Canvas canvas) {
        int xPosition = 0;
        int yPosition = 0;
        foreach (var mapDefinition in mapDefinitions) {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(canvas.transform, false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = mapDefinition.name;
            button.GetComponent<Button>().onClick.AddListener(() => LoadMap(mapDefinition.mapId));

            RectTransform rectangle = button.GetComponent<RectTransform>();
            button.transform.Translate(new Vector3(
                xPosition * (rectangle.sizeDelta.x + horizontalButtonDistance),
                yPosition * -(rectangle.sizeDelta.y + verticalButtonDistance),
                0
            ));
            
            // the position of the rectangle is equal to its center, so we need space for 1 1/2 rectangles plus spacing to the right
            if (rectangle.sizeDelta.x * 1.5 + horizontalButtonDistance + rectangle.position.x > Screen.width) {
                xPosition = 0;
                yPosition++;
            } else {
                xPosition++;
            }
        }
    }

    public void LoadMap(string mapId) {
        CurrentMap = mapId;
        StartCoroutine(LoadMapAsync(mapId));
    }

    IEnumerator LoadMapAsync(string mapId)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapId, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
                
        SceneManager.UnloadSceneAsync(currentScene);

        Scene mapScene = SceneManager.GetSceneByName(mapId);
        GameObject logic = Instantiate(gameLogic, Vector3.zero, Quaternion.identity);
        PlayerController player = logic.GetComponentInChildren<PlayerController>();
        MapController map = FindComponent<MapController>(mapScene);
        CarOption carOption = carSelector.GetSelectedCar();

        if (map != null && player != null) {
            player.SpawnCar(carOption, map);
        } else {
            Debug.LogError("Couldn't load map or player");
        }
    }

    private T FindComponent<T>(Scene scene) {
        return scene.GetRootGameObjects()
            .Select(rootItem => rootItem.GetComponentInChildren<T>())
            .Where(component => component != null)
            .First();
    }
}
