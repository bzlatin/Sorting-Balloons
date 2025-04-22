using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip buttonClickClip;
    public AudioClip switchLevelClickClip;

    public AudioClip correctSwapClip;
    public AudioClip wrongSwapClip;
    public AudioClip correctSkipClip;
    public AudioClip winGameClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        EnsureAudioSourceIsValid();
    }

    void OnEnable()
    {
        EnsureAudioSourceIsValid();
    }

    private void EnsureAudioSourceIsValid()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("UIAudioManager: Missing AudioSource! Attach one or assign in Inspector.");
            }
        }
    }



    public void PlayButtonClick()
    {
        if (audioSource && buttonClickClip)
            audioSource.PlayOneShot(buttonClickClip);
    }
    public void SwitchLevelButtonClick()
    {
        if (audioSource && switchLevelClickClip)
            audioSource.PlayOneShot(switchLevelClickClip);
    }

    public void PlayCorrectSwap()
    {
        if (audioSource && correctSwapClip)
            audioSource.PlayOneShot(correctSwapClip);
    }

    public void PlayWrongSwap()
    {
        if (audioSource && wrongSwapClip)
            audioSource.PlayOneShot(wrongSwapClip);
    }

    public void PlayCorrectSkip()
    {
        if (audioSource && correctSkipClip)
            audioSource.PlayOneShot(correctSkipClip);
    }

    public void PlayWinSound()
    {
        if (audioSource && winGameClip)
            audioSource.PlayOneShot(winGameClip);
    }
}
