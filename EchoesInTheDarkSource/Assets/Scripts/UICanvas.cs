using Unity.Behavior;
using UnityEngine;
using UnityEngine.InputSystem;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private BehaviorGraphAgent[] guards;

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame) _panel.SetActive(!_panel.activeSelf);
    }

    public void SetGuardCircleDetection(bool value)
    {
        foreach (BehaviorGraphAgent guard in guards)
        {
            guard.SetVariableValue("DetectionCircleEnabled", value);
        }
    }
}
