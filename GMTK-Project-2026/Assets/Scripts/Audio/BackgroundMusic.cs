using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Static reference to the single instance allowed in the game
    public static BackgroundMusic Instance { get; private set; }

    public bool GlobalMuteState = false;

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy it
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set this object as the unique instance
        Instance = this;

        // Tell Unity not to destroy this object when loading new scenes
        DontDestroyOnLoad(gameObject);
    }
}
