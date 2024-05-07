using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class LootSpawning : MonoBehaviour
{
    [SerializeField] private List<LootSpawnPoint> _spawnPositions; // Add all spawnpoints in the inspector.
    [SerializeField] private int _maxLootOnMap; // Maximum number of loot allowed to be on the map at te same time.
    [SerializeField] private float _timeToSpawn;
    private float _timer;

    [SerializeField] private GameObject _loot;

    private System.Random random = new System.Random();

    private void Awake()
    {
        // Add all spawn points to List
        GameSettings.LootSpawnPoints.AddRange(_spawnPositions);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
        Instantiate(_loot, validSpawnPoint[index].transform.position, 
            Quaternion.identity, validSpawnPoint[index].transform);

        validSpawnPoint.Clear();
    }
}
