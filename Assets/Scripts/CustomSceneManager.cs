using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public string NextSceneName = "MatthiasTestScene";

    private void Start()
    {
        StartingGameScript.OnCountdownFinished += SwitchScene;
    }

    private void OnDestroy()
    {
        StartingGameScript.OnCountdownFinished -= SwitchScene;
    }

    private void SwitchScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(NextSceneName);
    }
}