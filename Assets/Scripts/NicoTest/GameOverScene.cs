
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// !!! There should be an empty object in the GameOverScene with this script attached. !!!
// !!! There should be an empty object in the GameOverScene with this script attached. !!!
// !!! There should be an empty object in the GameOverScene with this script attached. !!!
public class GameOverScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winnerText;

    [Header("References")]
    [SerializeField] private Transform _winnerPos;
    [SerializeField] private Transform _secondPos;
    [SerializeField] private Transform _thirdPos;


    [SerializeField] private GameObject _LeaderboardPrefab;
    [SerializeField] private Transform _leaderboardLayoutGroup;

    [SerializeField] private TMP_Text _quitGame;
    [SerializeField] private Image _quitGamePanel;
    [SerializeField] private TMP_Text _restartGame;
    [SerializeField] private Image _restartGamePanel;
    [SerializeField] private Vector3 _minSize = Vector3.one / 1000;
    [SerializeField] private float _lerpSpeed = 50f;
    private Vector3 _maxSize;
    private Color _endColor;
    private Color _startColor;

    private AudioSource audioSource;

    private bool _lerpTextSizeAndOpacity;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        GameSettings.GameIsInProgress = false;
        SetPlayerPositions();
        SetPlayerLeaderboard();
        // Set Winning player in UI
        if ( _winnerText != null && GameSettings.FirstPlace != null)
        {
            _winnerText.text = $"The winner is: {GameSettings.FirstPlace.gameObject.GetComponent<Player>().PlayerName}";
        }

        else
        {
            Debug.Log("Something is Null in winner text");
        }

        _lerpTextSizeAndOpacity = false;
        _maxSize = _restartGame.transform.localScale;
        _endColor = _restartGamePanel.color;
        _startColor = _endColor;
        _startColor.a = 0;
        _restartGamePanel.color = _startColor;
        _quitGamePanel.color = _startColor;
        _restartGame.transform.localScale = _minSize;
        _quitGame.transform.localScale = _minSize;
        Invoke(nameof(StartTextVisibilityLerp), 3);
    }

    private void SetPlayerLeaderboard()
    {
        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            GameObject dog = Instantiate(_LeaderboardPrefab, _leaderboardLayoutGroup);
            TMP_Text[] texties = dog.GetComponentsInChildren<TMP_Text>();
            texties[0].text = WinnerDecider.Leaderboard[i].PlayerName;
            texties[1].text = WinnerDecider.Leaderboard[i].Score.ToString();
        }
    }

    private void SetPlayerPositions()
    {
        if (GameSettings.FirstPlace != null)
        {
            GameSettings.FirstPlace.transform.rotation = new Quaternion(0, 0, 0, 0);
            GameSettings.FirstPlace.transform.position = _winnerPos.position;
        }

        if (GameSettings.SecondPlace != null)
        {
            GameSettings.SecondPlace.transform.rotation = new Quaternion(0, 0, 0, 0);
            GameSettings.SecondPlace.transform.position = _secondPos.position;
        }

        if (GameSettings.ThirdPlace != null)
        {
            GameSettings.ThirdPlace.transform.rotation = new Quaternion(0, 0, 0, 0);
            GameSettings.ThirdPlace.transform.position = _thirdPos.position;
        }
    }


    void Update()
    {
        if (!_lerpTextSizeAndOpacity) return;
        _restartGame.transform.localScale = Vector3.Lerp(_restartGame.transform.localScale, _maxSize, _lerpSpeed * Time.deltaTime);
        _quitGame.transform.localScale = Vector3.Lerp(_quitGame.transform.localScale, _maxSize, _lerpSpeed * Time.deltaTime);
        Color newAlpha = _restartGamePanel.color;
        newAlpha.a = Mathf.Lerp(newAlpha.a, _endColor.a, _lerpSpeed * Time.deltaTime);
        _restartGamePanel.color = newAlpha;
        _quitGamePanel.color = newAlpha;

        if (!(Vector3.Distance(_restartGame.transform.localScale, _maxSize) < 0.01f)) return;
        _lerpTextSizeAndOpacity = false;
        _restartGame.transform.localScale = _maxSize;
        _quitGame.transform.localScale = _maxSize;
        GameSettings.CanEndGame();
    }

    private void StartTextVisibilityLerp()
    {
        _lerpTextSizeAndOpacity = true;
    }

}
