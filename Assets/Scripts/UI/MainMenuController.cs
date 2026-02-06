using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Settings")]
public SettingsUI settingsUI; // ← AGREGAR


    void Start()
    {
        // Aplicar paleta guardada
        if (PaletteManager.Instance != null)
        {
            PaletteManager.Instance.ApplyCurrentPalette();
        }
        
        // Reproducir música de menú
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
        }
    }
    
    public void OnStartButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("Hangar");
    }
    
    public void OnPalettesButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("PaletteSelector");
    }
public void OnSettingsButton()
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayButtonClick();
    
    if (settingsUI != null)
        settingsUI.Open();
}
    
    public void OnExitButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Application.Quit();
        Debug.Log("Exit Game");
    }
}
 