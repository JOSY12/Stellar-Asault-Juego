using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    
    [Header("Settings Panel Reference")]
    public SettingsUI settingsUI; // ← CAMBIO: Referenciar el script
    
    [Header("Pause Button")]
    public Button pauseButton;
    
    private bool isPaused = false;
    
    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (settingsUI != null)
            settingsUI.gameObject.SetActive(false);
        
        // Conectar botones
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        if (pauseButton != null)
            pauseButton.onClick.AddListener(Pause);
    }
    
    void Update()
    {
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
            return;
        
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
        
        if (settingsUI != null)
            settingsUI.Close();
        
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void OpenSettings()
    {
        if (settingsUI != null)
            settingsUI.Open();
        
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
}
 