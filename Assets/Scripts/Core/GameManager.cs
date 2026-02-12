using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameObject pauseMenu;
    
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
    public float survivalTime = 0f;
    public bool hasUsedContinue = false;
    
    [Header("Player Reference")]
    public PlayerController player;
    
    [Header("UI References")]
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI killsText;
    public TMPro.TextMeshProUGUI scrapText;
    public DeathScreenUI deathScreen;
    public TMPro.TextMeshProUGUI timeText;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        StartNewGame();
    }
    
    void Update()
    {
        if (isGameOver || isPaused) return;
        
        survivalTime += Time.deltaTime;
        
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
        survivalTime = 0f;
        hasUsedContinue = false;
        
        Time.timeScale = 1f;
        
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
        if (ZoneManager.Instance != null)
        {
            float multiplier = ZoneManager.Instance.GetScrapMultiplier();
            amount = Mathf.RoundToInt(amount * multiplier);
        }
        
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
        
        // ═══ INTEGRACIÓN ADS: Notificar para interstitial ═══
        if (AdManager.Instance != null)
        {
            AdManager.Instance.OnPlayerDeath();
        }
        // ════════════════════════════════════════════════════
        
        if (player != null)
        {
            player.StopInput();
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSFX);
        
        if (deathScreen != null)
        {
            deathScreen.ShowDeathScreen(
                currentWave, 
                killsThisRun, 
                scrapThisRun, 
                Mathf.FloorToInt(scrapThisRun * 0.4f),
                survivalTime,
                hasUsedContinue
            );
        }
    }
    
    public void ContinueRun()
    {
        if (hasUsedContinue) return;
        
        Debug.Log("=== CONTINUE ===");
        
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;
        hasUsedContinue = true;
        
        if (player != null)
        {
            player.Revive();
        }
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        
        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in enemyBullets)
        {
            if (bullet != null)
                Destroy(bullet);
        }
    }

    public void EndRun(bool watchedAd)
    {
        if (SaveManager.Instance == null) return;
        
        int scrapToSave = watchedAd ? scrapThisRun : Mathf.FloorToInt(scrapThisRun * 0.4f);
        
        SaveManager.Instance.AddScrap(scrapToSave);
        SaveManager.Instance.AddKills(killsThisRun);
        SaveManager.Instance.SetHighScore(killsThisRun);
        
        if (SaveManager.Instance.GetBestTime() < survivalTime)
        {
            SaveManager.Instance.SetBestTime(survivalTime);
        }
        
        Debug.Log($"Run ended. Scrap saved: {scrapToSave}");
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
        
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }
    
    void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"{currentWave}";
        
        if (killsText != null)
            killsText.text = $"{killsThisRun}";
        
        if (scrapText != null)
            scrapText.text = $"{scrapThisRun}";
        
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(survivalTime / 60f);
            int seconds = Mathf.FloorToInt(survivalTime % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}