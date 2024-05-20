using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager _playerInputManager;


    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerLeft -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        //if (GameSettings.PlayersInGame.Count >= GameSettings.PlayersRequiredToStart)
        //{
        //    return;
        //}

        //GameSettings.PlayersInGame.Add(player);
        GameSettings.PlayersRequiredToStart = 4;
        // GameSettings.PlayersRequiredToStart = Mathf.Max(GameSettings.MinimumPlayersRequiredToStart, GameSettings.PlayersInGame.Count);
        //player.gameObject.GetComponent<PlayerMovementController>().SetPlayerStartingPosition(GameSettings.PlayersInGame.Count - 1);
        //var playerindex = GameSettings.PlayersInGame.Count - 1;
        //player.gameObject.transform.position += Vector3.forward * GameSettings.PlayersInGame.Count * 2;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        Debug.Log("Player Added");
    }
}
