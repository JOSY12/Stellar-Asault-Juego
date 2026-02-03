using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    
    [Header("SFX Clips")]
    public AudioClip shootSFX;
    public AudioClip enemyHitSFX;
    public AudioClip enemyDeathSFX;
    public AudioClip playerHitSFX;
    public AudioClip playerDeathSFX;
    public AudioClip upgradeSFX;
    public AudioClip buttonClickSFX;
    public AudioClip itemPickupSFX;
    public AudioClip bossSpawnSFX;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        // Cargar configuración guardada
        if (SaveManager.Instance != null)
        {
            musicSource.volume = SaveManager.Instance.GetMusicVolume();
            sfxSource.volume = SaveManager.Instance.GetSFXVolume();
            
            musicSource.mute = !SaveManager.Instance.IsMusicEnabled();
            sfxSource.mute = !SaveManager.Instance.IsSFXEnabled();
        }
    }
    
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        if (SaveManager.Instance != null)
            SaveManager.Instance.SetMusicVolume(volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        if (SaveManager.Instance != null)
            SaveManager.Instance.SetSFXVolume(volume);
    }
    
    public void ToggleMusic(bool enabled)
    {
        musicSource.mute = !enabled;
        if (SaveManager.Instance != null)
            SaveManager.Instance.SetMusicEnabled(enabled);
    }
    
    public void ToggleSFX(bool enabled)
    {
        sfxSource.mute = !enabled;
        if (SaveManager.Instance != null)
            SaveManager.Instance.SetSFXEnabled(enabled);
    }
    
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }
}