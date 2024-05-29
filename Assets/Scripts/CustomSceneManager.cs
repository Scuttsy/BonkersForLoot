using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public string NextSceneName;

    private InputSystemUIInputModule _inputSystemUIInputModule;

    private void Start()
    {
        _inputSystemUIInputModule = GetComponent<InputSystemUIInputModule>();

        StartingGameScript.OnCountdownFinished += SwitchScene;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        StartingGameScript.OnCountdownFinished -= SwitchScene;
    }

    private void SwitchScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(NextSceneName);
        PlayerMovementController.StartGame();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}