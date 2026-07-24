using UnityEngine;

public enum AudioEffects
{
    DASH,
    ENEMY_DEATH,
    HURT
}

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField]
    AudioClip dashSound;
    [SerializeField]
    AudioClip enemyDeathSound;
    [SerializeField]
    AudioClip hurtSound;
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
            case AudioEffects.ENEMY_DEATH:
                clipToUse = enemyDeathSound;
                break;
            case AudioEffects.HURT:
                clipToUse = hurtSound;
                break;
            default:
                Debug.LogError("Unknown audio effect requested");
                return;
        }
        audioSource.PlayOneShot(clipToUse);
    }
}
