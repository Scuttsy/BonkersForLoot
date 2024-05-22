using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private GameObject _parent;
    [SerializeField] private float _floatSpeed = 1.0f; //the time in seconds that the coin should take to bounce up and down
    [SerializeField] private float _explodeSpeed = 1.0f;
    public Vector3 _ExplodeTarget = Vector3.zero; //The place that the coin tries to move to

    [SerializeField]
    private AudioClip _audioClip;

    [SerializeField] private GameObject _lootPickupIcon;

    private void Awake()
    {
        GameSettings.LootOnMap++; // Add 1 to loot on map when created
        _parent = this.transform.parent.gameObject;
        _parent.GetComponent<LootSpawnPoint>().HasSpawnedLoot = true;
    }

    private void OnDestroy()
    {
        GameSettings.LootOnMap--; // Substract 1 from loot on map when Destroyed
        _parent.GetComponent<LootSpawnPoint>().HasSpawnedLoot = false;                                                                                                                                                                                                                                                       
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger Check if collider is Player
        // and if so add 1 UnclaimedLoot of that player and destroy loot.

        GameObject otherParent = other.gameObject.transform.parent.gameObject;

        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.UnclaimedLoot++;
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            Debug.Log("Is " + player.UnclaimedLoot);
            Destroy(this.gameObject);
        }

        if (otherParent.TryGetComponent(out Player parentPlayer))
        {
            parentPlayer.UnclaimedLoot++;
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            Debug.Log("Is " + parentPlayer.UnclaimedLoot);
            Instantiate(_lootPickupIcon, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_ExplodeTarget != Vector3.zero)
        {
            Vector3 velocity = (_ExplodeTarget - transform.position).normalized;
            float speed = Mathf.Min(_explodeSpeed, (_ExplodeTarget - transform.position).magnitude);
            transform.position += velocity * speed * Time.deltaTime;

        }
    }
}
