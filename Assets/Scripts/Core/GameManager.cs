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
    public float survivalTime = 0f; // ← NUEVO: Timer
public bool hasUsedContinue = false; // ← NUEVO: Solo 1 continue por run
    [Header("Player Reference")]
    public PlayerController player;
    
    [Header("UI References")]
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI killsText;
    public TMPro.TextMeshProUGUI scrapText;
    public DeathScreenUI deathScreen;
    public TMPro.TextMeshProUGUI timeText; // ← NUEVO
    
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
    
    // Timer de supervivencia
    survivalTime += Time.deltaTime;
    
    // Sistema de oleadas
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
    
    // ← AGREGAR: Detener input del player
    if (player != null)
    {
        player.StopInput();
    }
    
    // Reproducir sonido
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSFX);
    
    // Mostrar Death Screen
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
    else
    {
        Debug.LogError("DeathScreen reference is NULL!");
    }
}
    
public void ContinueRun()
{
    if (hasUsedContinue)
    {
        Debug.Log("Already used continue this run!");
        return;
    }
    
    Debug.Log("=== CONTINUE DEBUG ===");
    
    // Reactivar juego PRIMERO
    isGameOver = false;
    isPaused = false;
    Time.timeScale = 1f;
    
    hasUsedContinue = true;
    
    Debug.Log($"Game state reset: isGameOver={isGameOver}, isPaused={isPaused}, timeScale={Time.timeScale}");
    
    // Revivir jugador
    if (player != null)
    {
        player.Revive();
        Debug.Log("Player revived!");
    }
    else
    {
        Debug.LogError("Player reference is NULL!");
    }
    
    // ← MEJORAR: Destruir enemigos inmediatamente
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    Debug.Log($"Destroying {enemies.Length} enemies...");
    
    foreach (GameObject enemy in enemies)
    {
        if (enemy != null)
        {
            Destroy(enemy);
        }
    }
    
    // ← AGREGAR: Destruir balas enemigas también
    GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
    Debug.Log($"Destroying {enemyBullets.Length} enemy bullets...");
    
    foreach (GameObject bullet in enemyBullets)
    {
        if (bullet != null)
        {
            Destroy(bullet);
        }
    }
    
    Debug.Log("=== CONTINUE COMPLETE ===");
}

public void EndRun(bool watchedAd)
{
    if (SaveManager.Instance == null) return;
    
    int scrapToSave;
    
    if (watchedAd)
    {
        // Vio ad: guarda 100%
        scrapToSave = scrapThisRun;
    }
    else
    {
        // No vio ad: guarda 40%
        scrapToSave = Mathf.FloorToInt(scrapThisRun * 0.4f);
    }
    
    SaveManager.Instance.AddScrap(scrapToSave);
    SaveManager.Instance.AddKills(killsThisRun);
    SaveManager.Instance.SetHighScore(killsThisRun);
    
    // Guardar mejor tiempo
    if (SaveManager.Instance.GetBestTime() < survivalTime)
    {
        SaveManager.Instance.SetBestTime(survivalTime);
    }
    
    Debug.Log($"Run ended. Scrap saved: {scrapToSave} (Ad: {watchedAd})");
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
             // ← NUEVO: Actualizar timer
    if (timeText != null)
    {
        int minutes = Mathf.FloorToInt(survivalTime / 60f);
        int seconds = Mathf.FloorToInt(survivalTime % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }
    }
}