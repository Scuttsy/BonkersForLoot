using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, .25f);
    }
}
public struct SpawnPointData
{
    public Transform SpawnPointName;
    public float FurthestDistanceToPlayer;
    public SpawnPointData(Transform spawnPoint, float furthestDistanceToPlayer )
    {
        SpawnPointName = spawnPoint;
        FurthestDistanceToPlayer = furthestDistanceToPlayer;
    }
}