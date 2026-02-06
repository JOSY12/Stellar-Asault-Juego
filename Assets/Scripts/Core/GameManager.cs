using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;
    
    [Header("Wave System")]
    public int currentWave = 1;
    public float waveDuration = 30f;
    private float waveTimer = 0f;
    
    [Header("Economy")]
    public int scrapThisRun = 0;
    public int killsThisRun = 0;
    
    [Header("Player Reference")]
    public PlayerController player;
    
    [Header("UI References")]
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI killsText;
    public TMPro.TextMeshProUGUI scrapText;
    public DeathScreenUI deathScreen;
    
    void Awake()
    {

       
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {


    //           PlayerPrefs.DeleteAll();
    //  Debug.Log("PlayerPrefs deleted!");
        // GameManager NO persiste entre escenas
        StartNewGame();
        
        // Aplicar paleta guardada
        if (PaletteManager.Instance != null)
        {
            PaletteManager.Instance.ApplyCurrentPalette();
        }
    }
    
    void Update()
    {
        if (isGameOver || isPaused) return;
        
        // Sistema de oleadas por tiempo
        waveTimer += Time.deltaTime;
        if (waveTimer >= waveDuration)
        {
            waveTimer = 0f;
            NextWave();
        }
        
        UpdateUI();
    }
    
    public void StartNewGame()
{
    isGameOver = false;
    isPaused = false;
    currentWave = 1;
    scrapThisRun = 0;
    killsThisRun = 0;
    waveTimer = 0f;
    
    Time.timeScale = 1f;
    
    // ← CAMBIO: Música random
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayRandomGameplayMusic();
    
    Debug.Log("Game Started!");
}
    
    void NextWave()
    {
        currentWave++;
        Debug.Log($"=== WAVE {currentWave} ===");
        
        if (currentWave % 10 == 0)
        {
            Debug.Log(">>> BOSS WAVE! <<<");
        }
    }
    
    public void AddScrap(int amount)
    {
        scrapThisRun += amount;
    }
    
    public void AddKill()
    {
        killsThisRun++;
        
        int scrapReward = 1 + (currentWave / 10);
        AddScrap(scrapReward);
    }
    
    public void PlayerDied()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        Debug.Log("=== PLAYER DIED ===");
        
        // NO pausar con Time.timeScale, solo marcar como game over
        // Time.timeScale = 0f; ← QUITAR ESTO
        
        // Reproducir sonido de muerte
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSFX);
        
        // Guardar stats
        int permanentScrap = SaveRunStats();
        
        Debug.Log($"Wave: {currentWave}, Kills: {killsThisRun}, Scrap: {scrapThisRun}, Saved: {permanentScrap}");
        
        // Mostrar Death Screen
        if (deathScreen != null)
        {
            Debug.Log("Showing DeathScreen...");
            deathScreen.ShowDeathScreen(currentWave, killsThisRun, scrapThisRun, permanentScrap);
        }
        else
        {
            Debug.LogError("DeathScreen reference is NULL!");
            Invoke("RestartGame", 3f);
        }
    }
    
    int SaveRunStats()
    {
        if (SaveManager.Instance == null) return 0;
        
        int permanentScrap = Mathf.FloorToInt(scrapThisRun * 0.1f);
        SaveManager.Instance.AddScrap(permanentScrap);
        SaveManager.Instance.AddKills(killsThisRun);
        SaveManager.Instance.SetHighScore(killsThisRun);
        
        return permanentScrap;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }
    
    void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"WAVE {currentWave}";
        
        if (killsText != null)
            killsText.text = $"KILLS: {killsThisRun}";
        
        if (scrapText != null)
            scrapText.text = $"SCRAP: {scrapThisRun}";
    }
}