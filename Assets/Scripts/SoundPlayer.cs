using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioClip soundClip;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = soundClip;

        audioSource.Play();
    }
}
