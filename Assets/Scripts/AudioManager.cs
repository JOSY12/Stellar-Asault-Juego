using UnityEngine;

/// <summary>
/// Gestor de audio del juego
/// Maneja efectos de sonido y música
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Sound Effects")]
    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip criticalSound;
    public AudioClip upgradeSound;
    public AudioClip gameOverSound;
    public AudioClip enemyDeathSound;
    public AudioClip coinSound;
    
    [Header("Settings")]
    [Range(0f, 1f)] public float sfxVolume = 0.7f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    
    private AudioSource audioSource;
    private AudioSource musicSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupAudioSources()
    {
        // Audio Source para SFX
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = sfxVolume;

        // Audio Source separado para música
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }
    
    /// <summary>
    /// Reproduce un efecto de sonido
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, sfxVolume);
        }
    }

    /// <summary>
    /// Reproduce un sonido con volumen específico
    /// </summary>
    public void PlaySound(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }

    // Métodos de conveniencia para sonidos específicos
    public void PlayShoot() => PlaySound(shootSound, 0.3f);
    public void PlayHit() => PlaySound(hitSound);
    public void PlayCritical() => PlaySound(criticalSound, 1.2f);
    public void PlayUpgrade() => PlaySound(upgradeSound);
    public void PlayGameOver() => PlaySound(gameOverSound);
    public void PlayEnemyDeath() => PlaySound(enemyDeathSound, 0.5f);
    public void PlayCoin() => PlaySound(coinSound, 0.4f);

    /// <summary>
    /// Reproduce música de fondo
    /// </summary>
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null && musicSource != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Para la música
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Ajusta volumen de SFX
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Ajusta volumen de música
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
}