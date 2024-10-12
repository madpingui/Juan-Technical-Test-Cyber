using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip missClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip flipClip;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if it already exists
            return;
        }

        Instance = this; // Assign the singleton instance
        DontDestroyOnLoad(gameObject); // Persist through scene changes
    }

    public void PlayCardMiss()
    {
        audioSource.clip = missClip;
        audioSource.Play();
    }

    public void PlayCardMatch()
    {
        audioSource.clip = matchClip;
        audioSource.Play();
    }

    public void PlayCardFlip()
    {
        audioSource.clip = flipClip;
        audioSource.Play();
    }
}
