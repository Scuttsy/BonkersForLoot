using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    public int UnclaimedLoot = 0;
    public int Score = 0;
    public string PlayerName = "(Default Name)";

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
