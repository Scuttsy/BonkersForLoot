using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : MonoBehaviour 
{
    public int UnclaimedLoot = 0;
    public int Score = 0;
    public string PlayerName = "(Default Name)";

    [SerializeField] private List<Material> _playerColours;
    [SerializeField] private List<Color> _playerDeviceColours;
    [SerializeField] private List<Color> _lootPickupColours;
    [SerializeField] private Renderer _playerModel;

    [SerializeField] private ScreenShake _screenShake;

    [SerializeField] private TMP_Text _unclaimedScoreUI;
    private bool _showUnclaimedIcon;

    [HideInInspector] public bool HasLostPoints;

    [Header("ArrowToCentre")]
    [SerializeField] private Transform _arrowToCentrePivot;
    [Range(0.1f, 3f)]
    [SerializeField] private float _arrowScalingMultiplier;
    [SerializeField] private int _arrowToCentreThreshold = 1;
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
        if (device.GetType().ToString() == "UnityEngine.InputSystem.DualShock.DualShock4GamepadHID")
        {
            DualShockGamepad ds4 = (DualShockGamepad)device;
            ds4.SetLightBarColor(_playerDeviceColours[index]);
        }
        if (device.GetType().ToString() == "UnityEngine.InputSystem.DualShock.DualSenseGamepadHID")
        {
            var dualSenseGamepad = (DualSenseGamepadHID)device;
            dualSenseGamepad.SetLightBarColor(_playerDeviceColours[index]);
        }

        _unclaimedScoreUI.enabled = false;
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
        if (!_unclaimedScoreUI.enabled) return;
        _unclaimedScoreUI.text = UnclaimedLoot.ToString();
    }

    private void FixedUpdate()
    {
        if (UnclaimedLoot > _arrowToCentreThreshold)
        {
            _arrowToCentrePivot.GetChild(0).gameObject.SetActive(true);
            _arrowToCentrePivot.LookAt(new Vector3(0, _arrowToCentrePivot.transform.position.y, 0));
        }
        else
            _arrowToCentrePivot.GetChild(0).gameObject.SetActive(false);
    }

    public int LoseUnclaimedLoot()
    {
        if (HasLostPoints) return 0;
        HasLostPoints = true;
        // Lose 20% of the unclaimedLoot
        int current = UnclaimedLoot;

        if ((UnclaimedLoot / 5f) < 1f && (UnclaimedLoot / 5f) > 0)
        {
            UnclaimedLoot -= 1;
        }

        else
        {
            this.UnclaimedLoot -= this.UnclaimedLoot / 5;
            UnclaimedLoot = (int)Mathf.Round(UnclaimedLoot);
        }

        return current - UnclaimedLoot;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Collision");
            //_screenShake.StartShaking();
        }
    }

    public void EnableUnclaimedLootUI()
    {
        _unclaimedScoreUI.enabled = true;
    }
}
