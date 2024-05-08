using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// !!! There should be an empty object in the GameOverScene with this script attached. !!!
// !!! There should be an empty object in the GameOverScene with this script attached. !!!
// !!! There should be an empty object in the GameOverScene with this script attached. !!!
public class GameOverScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winnerText;

    [SerializeField] private Transform _winnerPos;
    [SerializeField] private Transform _secondPos;
    [SerializeField] private Transform _thirdPos;
    // Start is called before the first frame update
    void Start()
    {
        GameSettings.GameIsInProgress = false;
        SetPlayerPositions();

        // Set Winning player in UI
        if ( _winnerText != null && GameSettings.FirstPlace != null)
        {
            _winnerText.text = $"The winner is: {GameSettings.FirstPlace.name}";
        }

        else
        {
            Debug.Log("Something is Null in winner text");
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
