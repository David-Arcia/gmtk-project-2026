using UnityEngine;

public enum AudioEffects
    {
        DASH
    }

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField]
    AudioClip dashSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayAudio(AudioEffects effectType)
    {
        if (!audioSource)
        {
            return;
        }
        AudioClip clipToUse = null;
        switch (effectType)
        {
            case AudioEffects.DASH:
                clipToUse = dashSound;
                break;
            default:
                Debug.LogError("Unknown audio effect requested");
                return;
        }
        audioSource.PlayOneShot(clipToUse);
    }
}
