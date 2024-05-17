using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    public int UnclaimedLoot = 0;
    public int Score = 0;
    public string PlayerName = "(Default Name)";

    [SerializeField] private List<Material> _playerColours;
    [SerializeField] private Renderer _playerModel;

    //private bool _positionSet = false;
    //private Vector3 _tempPos;
    //private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        // If this player hasn't been added yet to the PlayersInGame list add it.

        // !!! Moved to PlayerManager !!!

        //if (!GameSettings.PlayersInGame.Contains(this))
        //{
        //    GameSettings.PlayersInGame.Add(this);
        //}

        //_tempPos = this.gameObject.transform.position;

        //Debug.Log(_tempPos);

        //Debug.Log("Player Start");


        PlayerInput playerInput = GetComponent<PlayerInput>();
        int index = 0;
        for (int i = 0; i < GameSettings.PlayersInGame.Count; i++)
        {
            if (GameSettings.PlayersInGame[i] == playerInput)
            {
                index = i;
            }
        }

        if (index == 0)
        {
            _playerModel.material = _playerColours[0];
            PlayerName = "Blue";
        }
        else if (index == 1)
        {
            _playerModel.material = _playerColours[1];
            PlayerName = "Green";
        }
        else if (index == 2)
        {
            _playerModel.material = _playerColours[2];
            PlayerName = "Red";
        }
        else if (index == 3)
        {
            _playerModel.material = _playerColours[3];
            PlayerName = "Yellow";
        }

    }

    void Update()
    {
        //if (_positionSet == false && _timer > 0.1f)
        //{
        //    Debug.Log("Update " + _tempPos);
        //    transform.position = _tempPos;
        //    _positionSet = true;
        //}
        //else
        //{
        //    _timer += Time.deltaTime;
        //}

        //Debug.Log($"Position{this.transform.position}");
    }
}
