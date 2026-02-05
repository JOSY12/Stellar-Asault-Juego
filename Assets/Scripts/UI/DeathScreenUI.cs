using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathPanel;
    public TextMeshProUGUI waveReachedText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI scrapEarnedText;
    public TextMeshProUGUI permanentScrapText;
    
    [Header("Buttons")]
    public Button retryButton;
    public Button watchAdButton;
    public Button mainMenuButton;
    
    [Header("Ad Bonus")]
    public TextMeshProUGUI adBonusText;
    
    void Start()
    {
        // Ocultar al inicio
        if (deathPanel != null)
            deathPanel.SetActive(false);
        
        // Conectar botones
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);
        
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(OnWatchAd);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    
    public void ShowDeathScreen(int wave, int kills, int scrapEarned, int permanentScrap)
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);
        
        // Mostrar estadísticas
        if (waveReachedText != null)
            waveReachedText.text = $"WAVE {wave}";
        
        if (killsText != null)
            killsText.text = $"KILLS: {kills}";
        
        if (scrapEarnedText != null)
            scrapEarnedText.text = $"SCRAP EARNED: {scrapEarned}";
        
        if (permanentScrapText != null)
            permanentScrapText.text = $"SAVED: {permanentScrap}";
        
        // Calcular bonus de ad (50% más)
        int adBonus = Mathf.FloorToInt(scrapEarned * 0.5f);
        if (adBonusText != null)
            adBonusText.text = $"+{adBonus} BONUS";
    }
    
    void OnRetry()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
    
    void OnWatchAd()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        // TODO: Mostrar ad reward (FASE 5)
        // Por ahora solo dar el bonus
        
        if (GameManager.Instance != null && SaveManager.Instance != null)
        {
            int scrapBonus = Mathf.FloorToInt(GameManager.Instance.scrapThisRun * 0.5f);
            SaveManager.Instance.AddScrap(scrapBonus);
            
            Debug.Log($"Ad watched! Bonus scrap: {scrapBonus}");
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
    
    void OnMainMenu()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
// ```

// ---

// ### **PASO 2: Crear UI del Death Screen**

// 1. Abre la escena **Gameplay**
// 2. En el Canvas `GameplayUI`, crea:

// **Panel de Muerte:**
// ```
// GameplayUI
// └─ Panel (Nombrar: DeathPanel)
//     ├─ Anchor: Stretch (llenar todo)
//     ├─ Color: Negro (0, 0, 0, 220) ← Semi-transparente
//     └─ Active: ✗ (desactivado al inicio)
// ```

// **Contenedor central:**
// ```
// DeathPanel
// └─ Panel (Nombrar: ContentPanel)
//     ├─ Anchor: Center
//     ├─ Width: 500
//     ├─ Height: 700
//     ├─ Color: Gris oscuro (40, 40, 40, 255)
//     └─ Shadow: (5, -5) Negro
// ```

// **Textos de estadísticas:**
// ```
// ContentPanel
// ├─ Text (Nombrar: GameOverText)
// │   ├─ Pos Y: 280
// │   ├─ Text: "GAME OVER"
// │   ├─ Font Size: 64
// │   ├─ Color: Rojo (255, 100, 100)
// │   └─ Shadow: (3, -3) Negro
// │
// ├─ Text (Nombrar: WaveReachedText)
// │   ├─ Pos Y: 180
// │   ├─ Text: "WAVE 10"
// │   ├─ Font Size: 42
// │   ├─ Color: Amarillo
// │   └─ Shadow: (2, -2) Negro
// │
// ├─ Text (Nombrar: KillsText)
// │   ├─ Pos Y: 100
// │   ├─ Text: "KILLS: 50"
// │   ├─ Font Size: 36
// │   ├─ Color: Blanco
// │   └─ Shadow: (2, -2) Negro
// │
// ├─ Text (Nombrar: ScrapEarnedText)
// │   ├─ Pos Y: 40
// │   ├─ Text: "SCRAP EARNED: 25"
// │   ├─ Font Size: 32
// │   ├─ Color: Cyan
// │   └─ Shadow: (2, -2) Negro
// │
// └─ Text (Nombrar: PermanentScrapText)
//     ├─ Pos Y: -10
//     ├─ Text: "SAVED: 2"
//     ├─ Font Size: 28
//     ├─ Color: Verde (150, 255, 150)
//     └─ Shadow: (2, -2) Negro
// ```

// **Botones:**
// ```
// ContentPanel
// ├─ Button (Nombrar: RetryButton)
// │   ├─ Pos Y: -120
// │   ├─ Width: 400
// │   ├─ Height: 70
// │   ├─ Color: Verde (100, 255, 100, 200)
// │   └─ Text: "RETRY"
// │       ├─ Font Size: 36
// │       └─ Shadow: (2, -2) Negro
// │
// ├─ Button (Nombrar: WatchAdButton)
// │   ├─ Pos Y: -210
// │   ├─ Width: 400
// │   ├─ Height: 70
// │   ├─ Color: Amarillo (255, 220, 100, 200)
// │   └─ Panel interior con:
// │       ├─ Text: "WATCH AD"
// │       │   ├─ Font Size: 32
// │       │   └─ Color: Negro
// │       └─ Text (Nombrar: AdBonusText)
// │           ├─ Pos Y: -25
// │           ├─ Text: "+12 BONUS"
// │           ├─ Font Size: 24
// │           └─ Color: Verde
// │
// └─ Button (Nombrar: MainMenuButton)
//     ├─ Pos Y: -300
//     ├─ Width: 400
//     ├─ Height: 60
//     ├─ Color: Gris (150, 150, 150, 200)
//     └─ Text: "MAIN MENU"
//         ├─ Font Size: 28
//         └─ Shadow: (2, -2) Negro
// ```

// ---

// ### **PASO 3: Conectar DeathScreenUI**

// 1. Crea GameObject vacío en Hierarchy: `DeathScreenController`
// 2. Add Component → DeathScreenUI
// 3. Asignar referencias:
// ```
// UI References:
// ├─ Death Panel: [Arrastra DeathPanel]
// ├─ Wave Reached Text: [Arrastra WaveReachedText]
// ├─ Kills Text: [Arrastra KillsText]
// ├─ Scrap Earned Text: [Arrastra ScrapEarnedText]
// └─ Permanent Scrap Text: [Arrastra PermanentScrapText]

// Buttons:
// ├─ Retry Button: [Arrastra RetryButton]
// ├─ Watch Ad Button: [Arrastra WatchAdButton]
// └─ Main Menu Button: [Arrastra MainMenuButton]

// Ad Bonus:
// └─ Ad Bonus Text: [Arrastra AdBonusText]