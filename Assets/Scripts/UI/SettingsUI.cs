using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Audio")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;
    
    [Header("Gameplay")]
    public Toggle shakeToggle;
    
    [Header("Toggle Labels")]
    public TextMeshProUGUI musicToggleLabel;
    public TextMeshProUGUI sfxToggleLabel;
    public TextMeshProUGUI shakeToggleLabel;
    
    [Header("Buttons")]
    public Button closeButton;
    [Header("Controls")]
public Toggle controlToggle;
public TextMeshProUGUI controlToggleLabel;

    void Start()
    {
        // Ocultar al inicio
        // gameObject.SetActive(false);
        
        SetupSettings();
    
    if (closeButton != null)
        closeButton.onClick.AddListener(Close);
    }
    
  public void Open()
{
    gameObject.SetActive(true);
    
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayButtonClick();
}
    
    public void Close()
    {
        gameObject.SetActive(false);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void SetupSettings()
    {
         if (controlToggle != null)
    {
        bool useTouchZones = SaveManager.Instance.UseTouchZones();
        controlToggle.isOn = useTouchZones;
        controlToggle.onValueChanged.AddListener(OnControlToggleChanged);
        UpdateControlToggleLabel(useTouchZones);
    }
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
void OnControlToggleChanged(bool value)
{
    if (SaveManager.Instance != null)
        SaveManager.Instance.SetUseTouchZones(value);
    
    UpdateControlToggleLabel(value);
    
    // ← MEJORAR: Buscar player y aplicar cambio
    PlayerController player = Object.FindFirstObjectByType<PlayerController>();
    if (player != null)
    {
        PlayerController.ControlType newType = value ? 
            PlayerController.ControlType.TouchZones : 
            PlayerController.ControlType.Joysticks;
        
        player.SetControlType(newType);
        
        Debug.Log($"Control type set to: {newType}");
    }
    else
    {
        Debug.LogWarning("PlayerController not found in scene!");
    }
}


void UpdateControlToggleLabel(bool useTouchZones)
{
    if (controlToggleLabel != null)
    {
        controlToggleLabel.text = useTouchZones ? "TOUCH ZONES" : "JOYSTICKS";
        controlToggleLabel.color = Color.cyan;
    }
}

}