using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootSpawning : MonoBehaviour
{
    [SerializeField] private List<LootSpawnPoint> _spawnPositions; // Add all spawnpoints in the inspector.
    [SerializeField] private int _maxLootOnMap; // Maximum number of loot allowed to be on the map at te same time.
    [SerializeField] private float _timeToSpawn;
    private float _timer;

    [SerializeField] private GameObject _loot;
    [SerializeField] private GameObject _audioSourceManager;

    private System.Random random = new System.Random();

    private void Awake()
    {
        // Add all spawn points to List
        if (GameSettings.LootSpawnPoints != null)
        {
            GameSettings.LootSpawnPoints.Clear();
            GameSettings.LootSpawnPoints.AddRange(_spawnPositions);
        }
    }

    void Update()
    {
        if (_timer > _timeToSpawn) 
        {
            _timer = 0;

            if (GameSettings.LootOnMap >= _maxLootOnMap)
            {
                return;
            }

            SpawnLoot();
            
        }
        else
        {
            _timer += Time.deltaTime;
        }

    }

    private void SpawnLoot()
    {
        List<LootSpawnPoint> validSpawnPoint = new List<LootSpawnPoint>();
        int index;

        // Check if SpawnPoint is available and if so add it to list.
        foreach (LootSpawnPoint lsp in GameSettings.LootSpawnPoints)
        {
            if (lsp.HasSpawnedLoot == false)
            {
                validSpawnPoint.Add(lsp);
            }
        }

        // Instantiate Loot on random valid potition and parent it to the spawnpotition
        index = random.Next(0, validSpawnPoint.Count);
        float randomYRotation = Random.Range(0, 360);
        GameObject lootInstance = Instantiate(_loot, validSpawnPoint[index].transform.position, 
            Quaternion.Euler(0,randomYRotation,0), validSpawnPoint[index].transform);

        Loot lootScript = lootInstance.GetComponent<Loot>();
        if (lootScript != null && _audioSourceManager != null)
        {
            Debug.Log("Set audiosource");
            AudioSource audioSource = _audioSourceManager.GetComponent<AudioSource>();
            lootScript.SetAudioSource(audioSource);
        }

        validSpawnPoint.Clear();
    }
}
