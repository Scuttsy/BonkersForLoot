using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private Transform _pointerPivot;
    [SerializeField] private Transform _playerGFX;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void Update()
    {
        Vector3 centeredMousePos = Input.mousePosition - new Vector3(Screen.width/2f,Screen.height/2f,0);
        Vector3 pointerRotation = new Vector3(0,Mathf.Atan2(centeredMousePos.x, centeredMousePos.y) * Mathf.Rad2Deg, 0);
        _pointerPivot.eulerAngles = pointerRotation;

        if (Input.GetMouseButtonDown(0))
        {
            _playerGFX.forward = _pointerPivot.forward;

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}
