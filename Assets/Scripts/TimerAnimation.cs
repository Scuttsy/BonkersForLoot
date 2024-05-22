using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerAnimation : MonoBehaviour
{
    private Animation _animation;
    [SerializeField] private GameplayScene _gameplayScene;
    // Start is called before the first frame update
    void Start()
    {
        _animation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        // O7, rest in piece impact frames. :,(

        if (_gameplayScene.TimeRemaining < 30f)
        {
            _animation.Play();
        }
    }
}
