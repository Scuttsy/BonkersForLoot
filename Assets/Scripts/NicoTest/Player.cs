using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour 
{
    public float UnclaimedLoot = 0;
    public int Score = 0;
    public string PlayerName = "(Default Name)";

    [SerializeField] private List<Material> _playerColours;
    [SerializeField] private List<Color> _playerDeviceColours;
    [SerializeField] private Renderer _playerModel;

    [SerializeField] private ScreenShake _screenShake;

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

        switch (index)
        {
            case 0:
                _playerModel.material = _playerColours[0];
                PlayerName = "Blue";
                break;
            case 1:
                _playerModel.material = _playerColours[1];
                PlayerName = "Green";
                break;
            case 2:
                _playerModel.material = _playerColours[2];
                PlayerName = "Red";
                break;
            case 3:
                _playerModel.material = _playerColours[3];
                PlayerName = "Yellow";
                break;
        }


        var device = playerInput.devices[0];
        //TODO: set color for all controllers
        if (device.GetType().ToString() == "UnityEngine.InputSystem.DualShock.DualShock4GamepadHID")
        {
            DualShockGamepad ds4 = (DualShockGamepad)device;
            ds4.SetLightBarColor(_playerDeviceColours[index]);
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

    public void LoseUnclaimedLoot()
    {
        // Lose 20% of the unclaimedLoot

        if ((UnclaimedLoot / 5f) < 1f)
        {
            UnclaimedLoot -= 1f;
        }

        else
        {
            this.UnclaimedLoot -= this.UnclaimedLoot / 5;
            UnclaimedLoot = (int)Mathf.Round(UnclaimedLoot);
        }        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Collision");
            _screenShake.StartShaking();
        }
    }
}
