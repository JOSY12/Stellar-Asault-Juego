using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public CanvasGroup pauseCanvasGroup; // ← NUEVO
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    [Header("Pause Button")]
public Button pauseButton; // ← NUEVO
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle shakeToggle;
    public Button settingsBackButton;
    
    [Header("Toggle Labels")]
    public TextMeshProUGUI musicToggleLabel;
    public TextMeshProUGUI sfxToggleLabel;
    public TextMeshProUGUI shakeToggleLabel;
    
    private bool isPaused = false;
    
    void Start()
    {
        // Ocultar al inicio
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        // ← AGREGAR
    if (pauseButton != null)
        pauseButton.onClick.AddListener(Pause);
        // Si no hay CanvasGroup, agregarlo
        if (pausePanel != null && pauseCanvasGroup == null)
        {
            pauseCanvasGroup = pausePanel.GetComponent<CanvasGroup>();
            if (pauseCanvasGroup == null)
            {
                pauseCanvasGroup = pausePanel.AddComponent<CanvasGroup>();
            }
        }
        
        // Conectar botones
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(CloseSettings);
        
        // Configurar sliders y toggles
        SetupSettings();
    }
    
    void Update()
    {
        // Detectar tecla ESC o botón de pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    public void Pause()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return; // No pausar si ya murió
        
        isPaused = true;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
        
        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    public void Resume()
    {
        isPaused = false;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void GoToMainMenu()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    void SetupSettings()
    {
        if (SaveManager.Instance == null) return;
        
        // Music slider
        if (musicSlider != null)
        {
            musicSlider.value = SaveManager.Instance.GetMusicVolume();
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        // SFX slider
        if (sfxSlider != null)
        {
            sfxSlider.value = SaveManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // Music toggle
        if (musicToggle != null)
        {
            musicToggle.isOn = SaveManager.Instance.IsMusicEnabled();
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            UpdateMusicToggleLabel(musicToggle.isOn);
        }
        
        // SFX toggle
        if (sfxToggle != null)
        {
            sfxToggle.isOn = SaveManager.Instance.IsSFXEnabled();
            sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
            UpdateSFXToggleLabel(sfxToggle.isOn);
        }
        
        // Camera shake toggle
        if (shakeToggle != null)
        {
            shakeToggle.isOn = SaveManager.Instance.IsCameraShakeEnabled();
            shakeToggle.onValueChanged.AddListener(OnShakeToggleChanged);
            UpdateShakeToggleLabel(shakeToggle.isOn);
        }
    }
    
    void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }
    
    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }
    
    void OnMusicToggleChanged(bool value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleMusic(value);
        
        UpdateMusicToggleLabel(value);
    }
    
    void OnSFXToggleChanged(bool value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleSFX(value);
        
        UpdateSFXToggleLabel(value);
    }
    
    void OnShakeToggleChanged(bool value)
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SetCameraShakeEnabled(value);
        
        UpdateShakeToggleLabel(value);
    }
    
    void UpdateMusicToggleLabel(bool isOn)
    {
        if (musicToggleLabel != null)
        {
            musicToggleLabel.text = isOn ? "ON" : "OFF";
            musicToggleLabel.color = isOn ? Color.green : Color.red;
        }
    }
    
    void UpdateSFXToggleLabel(bool isOn)
    {
        if (sfxToggleLabel != null)
        {
            sfxToggleLabel.text = isOn ? "ON" : "OFF";
            sfxToggleLabel.color = isOn ? Color.green : Color.red;
        }
    }
    
    void UpdateShakeToggleLabel(bool isOn)
    {
        if (shakeToggleLabel != null)
        {
            shakeToggleLabel.text = isOn ? "ON" : "OFF";
            shakeToggleLabel.color = isOn ? Color.green : Color.red;
        }
    }
}
 