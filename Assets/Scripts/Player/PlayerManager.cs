using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void AddPlayer(PlayerInput player)
    {
        GameSettings.PlayersInGame.Add(player);
        var playerindex = GameSettings.PlayersInGame.Count - 1;
        player.gameObject.transform.position = GameSettings.SpawnPointList[playerindex].transform.position;
        //Debug.Log("" + GameSettings.PlayersInGame[playerindex].gameObject + ", " + playerindex + ", " + GameSettings.SpawnPointList[playerindex].transform.position); 
        //player.gameObject.GetComponent<PlayerMovementController>().Respawn();
        
        Debug.Log("Player Added");
    }
}
