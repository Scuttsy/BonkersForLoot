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
    // Start is called before the first frame update
    void Start()
    {
        // Set Winning player in UI
        if ( _winnerText != null && GameSettings.FirstPlayer != null)
        {
            _winnerText.text = $"The winner is: {GameSettings.FirstPlayer.name}";
        }

        else
        {
            Debug.Log("Something is Null in winner text");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
