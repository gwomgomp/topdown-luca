using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    void Start()
    {
        MenuManager menuManager = GameObject.FindObjectOfType<MenuManager>();
        DontDestroyOnLoad(menuManager);
        menuManager.LoadMenu();
    }
}
