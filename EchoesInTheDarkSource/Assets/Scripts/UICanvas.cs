using UnityEngine;
using UnityEngine.InputSystem;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame) _panel.SetActive(!_panel.activeSelf);
    }
}
