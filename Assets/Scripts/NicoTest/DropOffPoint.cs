using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffPoint : MonoBehaviour
{
    [SerializeField]
    private AudioClip _dropOffClip;
    private AudioSource _source;

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // On trigger Check if collider is Player
        // and if so add UnclaimedLoot to score and UnclaimedLoot = 0.
        if (other.gameObject.tag != "Player") return;
        GameObject otherParent = other.gameObject.transform.parent.gameObject;

        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.Score += (int)player.UnclaimedLoot;
            if (player.UnclaimedLoot != 0)
            {
                _source.PlayOneShot(_dropOffClip);
            }
            player.UnclaimedLoot = 0;
        }

        if (otherParent.TryGetComponent(out Player parentPlayer))
        {
            parentPlayer.Score += (int)parentPlayer.UnclaimedLoot;
            if (parentPlayer.UnclaimedLoot != 0)
            {
                _source.PlayOneShot(_dropOffClip);
            }
            parentPlayer.UnclaimedLoot = 0;
        }
    }
}
