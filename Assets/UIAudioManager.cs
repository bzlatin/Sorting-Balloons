using System.Collections;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("Audio")]
    public AudioSource audioSource;
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Menu Music")]
    public AudioSource menuMusicSource;
    public AudioClip menuLoopClip;
    [Range(0f, 1f)] public float menuMusicVolume = 0.5f;

    


    [Header("Clips")]
    public AudioClip buttonClickClip;
    public AudioClip switchLevelClickClip;

    public AudioClip correctSwapClip;
    public AudioClip wrongSwapClip;
    public AudioClip correctSkipClip;
    public AudioClip winGameClip;
    public AudioClip loseGameClip;
    public AudioClip backgroundMusicClip;



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
        if (sfxSource && buttonClickClip)
            sfxSource.PlayOneShot(buttonClickClip);
    }

public void SwitchLevelButtonClick()
{
    if (sfxSource && switchLevelClickClip)
        sfxSource.PlayOneShot(switchLevelClickClip);
}

public void PlayCorrectSwap()
{
    if (sfxSource && correctSwapClip)
        sfxSource.PlayOneShot(correctSwapClip);
}

public void PlayWrongSwap()
{
    if (sfxSource && wrongSwapClip)
        sfxSource.PlayOneShot(wrongSwapClip);
}

public void PlayCorrectSkip()
{
    if (sfxSource && correctSkipClip)
        sfxSource.PlayOneShot(correctSkipClip);
}

public void PlayWinSound()
{
    if (sfxSource && winGameClip)
        sfxSource.PlayOneShot(winGameClip);
}

public void PlayLoseSound()
{
    if (sfxSource != null && loseGameClip != null)
    {
        sfxSource.PlayOneShot(loseGameClip);
    }
}



    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusicClip == null)
            return;

        // Force stop to reset state
        musicSource.Stop();
        musicSource.clip = backgroundMusicClip;
        musicSource.loop = true;
        musicSource.volume = 0.4f; // or whatever value you want for game music
        musicSource.Play();

        Debug.Log("UIAudioManager: Playing Background Music");
    }



        public void PauseBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.UnPause();
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    private Coroutine musicFadeCoroutine;
private Coroutine menuFadeCoroutine;

    public void FadeOutBackgroundMusic(float fadeDuration = 2f)
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        if (musicSource != null && musicSource.isPlaying)
        {
            musicFadeCoroutine = StartCoroutine(FadeOutCoroutine(musicSource, fadeDuration));
        }
    }

    public void FadeOutMenuMusic(float fadeDuration = 2f)
    {
        if (menuFadeCoroutine != null)
        {
            StopCoroutine(menuFadeCoroutine);
        }

        if (menuMusicSource != null && menuMusicSource.isPlaying)
        {
            menuFadeCoroutine = StartCoroutine(FadeOutCoroutine(menuMusicSource, fadeDuration));
        }
    }

    private IEnumerator FadeOutCoroutine(AudioSource targetSource, float duration)
    {
        float startVolume = targetSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            targetSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        targetSource.Stop();
        targetSource.volume = startVolume;

        // ✅ Reset the handle if it's menuMusicSource
        if (targetSource == menuMusicSource)
        {
            menuFadeCoroutine = null;
        }
        else if (targetSource == musicSource)
        {
            musicFadeCoroutine = null;
        }
    }


    public void PlayMenuMusic()
    {
        if (menuFadeCoroutine != null)
        {
            StopCoroutine(menuFadeCoroutine);
            menuFadeCoroutine = null;
        }

        if (menuMusicSource == null || menuLoopClip == null)
        {
            Debug.LogWarning("UIAudioManager: menuMusicSource or menuLoopClip is NULL");
            return;
        }

        // FORCE stop to reset state
        menuMusicSource.Stop();
        menuMusicSource.clip = menuLoopClip;
        menuMusicSource.loop = true;
        menuMusicSource.volume = menuMusicVolume;
        menuMusicSource.Play();

        Debug.Log("UIAudioManager: Restarting Menu Music cleanly.");
    }

    public void StopMenuMusic()
    {
        if (menuMusicSource != null && menuMusicSource.isPlaying)
        {
            Debug.Log("UIAudioManager: Stopping Menu Music");

            menuMusicSource.Stop();
        }
    }

    public void SetMenuMusicVolume(float volume)
    {
        menuMusicVolume = Mathf.Clamp01(volume);
        if (menuMusicSource != null)
        {
            menuMusicSource.volume = menuMusicVolume;
        }
    }


}
