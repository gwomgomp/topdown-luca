using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager;

    void Start()
    {
        DontDestroyOnLoad(menuManager);
        menuManager.LoadMenu();
    }
}
