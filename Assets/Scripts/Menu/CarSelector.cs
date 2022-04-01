using UnityEngine;
using System.Linq;

public class CarSelector : MonoBehaviour
{
    [field: SerializeField]
    public GameObject DisplayPosition { get; private set; }

    private CarOption[] carOptions;
    private GameObject[] displayCar;
    private int currentCarIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Resources.LoadAll("CarOptions");
        carOptions = Resources.FindObjectsOfTypeAll<CarOption>();
        displayCar = carOptions.Select(option  => option.Car)
            .Select(car => Instantiate(car, DisplayPosition.transform, false))
            .Select(car => SetupDisplayCar(car))
            .ToArray();
        SetCar();
    }

    GameObject SetupDisplayCar(GameObject car) {
        car.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(car.GetComponent<Rigidbody>());
        foreach (var ps in car.GetComponentsInChildren<ParticleSystem>()) {
         Destroy(ps);
        }
        Destroy(car.GetComponentInChildren<EngineAudioGenerator>());
        Destroy(car.GetComponentInChildren<EngineAudioLooper>());
        car.SetActive(false);
        return car;
    }

    public void ChangeCar(bool next) {
        displayCar[currentCarIndex].SetActive(false);
        if (next) {
            currentCarIndex += 1;
            if (currentCarIndex >= carOptions.Length) {
                currentCarIndex = 0;
            }
        } else {
            currentCarIndex -= 1;
            if (currentCarIndex < 0) {
                currentCarIndex = carOptions.Length - 1;
            }
        }
        SetCar();
    }

    public void SetCar() {
        displayCar[currentCarIndex].SetActive(true);
    }

    public CarOption GetSelectedCar()
    {
        return carOptions[currentCarIndex];
    }
}
