using UnityEngine;

public class PanelToggler : MonoBehaviour
{
    public void Toggle() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
