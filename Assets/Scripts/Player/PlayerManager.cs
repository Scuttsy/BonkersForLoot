using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager _playerInputManager;

    public static PlayerInput[] PlayersInGame;


    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += AddPlayer;

    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerLeft -= AddPlayer;
    }

    public static void AddPlayer(PlayerInput player)
    {
        if (GameSettings.PlayersInGame.Count >= GameSettings.PlayersRequiredToStart)
        {
            return;
        }

        GameSettings.PlayersInGame.Add(player);
        var playerindex = GameSettings.PlayersInGame.Count - 1;
        //player.gameObject.transform.position += Vector3.forward * GameSettings.PlayersInGame.Count * 2;


        Debug.Log("Player Added");
    }
}
