using UnityEngine;
using TMPro;

public class MuteController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI muteText;
    private AudioSource[] activeSources;
    private bool muted;
    private BackgroundMusic bgMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bgMusic = BackgroundMusic.Instance;
        muted = bgMusic.GlobalMuteState;
        UpdateMutingElements(muted);
    }

    public void ToggleMute()
    {
        muted = !muted;
        UpdateMutingElements(muted);
        bgMusic.GlobalMuteState = muted;
    }

    private void UpdateMutingElements(bool muted)
    {
        activeSources = FindObjectsByType<AudioSource>();
        if (activeSources != null && activeSources.Length > 0)
        {
            foreach (AudioSource audioSource in activeSources)
            {
                audioSource.mute = muted;
            }

            string buttonText = "";
            if (muted)
            {
                buttonText = "Unmute";
            }
            else
            {
                buttonText = "Mute";
            }
            muteText.text = buttonText;
        }
    }
}
