using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCamOnPlayerJoin : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager _playerInputManager;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Light _noPlayerLight;

    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += ToggleCamera;
    }
    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= ToggleCamera;
    }

    private void ToggleCamera(PlayerInput player)
    {
        _camera.gameObject.SetActive(false);
        _noPlayerLight.gameObject.SetActive(false);
    }

}
