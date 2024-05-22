using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip lobbyMusic;
    public AudioClip playMusic;

    private AudioSource audioSource;
    private string currentSceneName;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0.2f;

        PlayMusic(SceneManager.GetActiveScene().name);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic(scene.name);
    }

    void PlayMusic(string sceneName)
    {
        if (sceneName == currentSceneName)
            return;

        currentSceneName = sceneName;

        switch (sceneName)
        {
            case "Lobby":
                audioSource.clip = lobbyMusic;
                break;
            case "WorkingDemoMap":
                audioSource.clip = playMusic;
                break;
            default:
                return;
        }

        if (audioSource.clip != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
