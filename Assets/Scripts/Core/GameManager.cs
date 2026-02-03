using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public bool isGameOver = false;
    public bool isPaused = false;
    
    [Header("Wave System")]
    public int currentWave = 0;
    public float timeBetweenWaves = 5f;
    private float waveTimer = 0f;
    
    [Header("Economy")]
    public int scrapThisRun = 0;
    public int killsThisRun = 0;
    
    [Header("Player Reference")]
    public PlayerController player;
    
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
        StartNewGame();
    }
    
    void Update()
    {
        if (isGameOver || isPaused) return;
        
        // Wave timer
        waveTimer += Time.deltaTime;
        if (waveTimer >= timeBetweenWaves)
        {
            waveTimer = 0f;
            NextWave();
        }
    }
    
    public void StartNewGame()
    {
        isGameOver = false;
        isPaused = false;
        currentWave = 0;
        scrapThisRun = 0;
        killsThisRun = 0;
        waveTimer = 0f;
        
        Time.timeScale = 1f;
        
        // Reproducir música de gameplay
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
    }
    
    void NextWave()
    {
        currentWave++;
        Debug.Log($"Wave {currentWave} started!");
    }
    
    public void AddScrap(int amount)
    {
        scrapThisRun += amount;
    }
    
    public void AddKill()
    {
        killsThisRun++;
        
        // Dar scrap por kill (varía según oleada)
        int scrapReward = 1 + (currentWave / 10);
        AddScrap(scrapReward);
        
        Debug.Log($"Kills: {killsThisRun} | Scrap: {scrapThisRun}");
    }
    
    public void PlayerDied()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0f;
        
        // Reproducir sonido de muerte
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeathSFX);
        
        // Guardar stats
        SaveRunStats();
        
        Debug.Log($"GAME OVER! Kills: {killsThisRun} | Scrap earned: {scrapThisRun}");
        
        // Aquí se mostraría el DeathScreen (FASE 4)
        // Por ahora solo reiniciamos después de 3 segundos
        Invoke("RestartGame", 3f);
    }
    
    void SaveRunStats()
    {
        if (SaveManager.Instance == null) return;
        
        // Convertir 10% de scrap a permanente
        int permanentScrap = Mathf.FloorToInt(scrapThisRun * 0.1f);
        SaveManager.Instance.AddScrap(permanentScrap);
        
        // Guardar kills totales
        SaveManager.Instance.AddKills(killsThisRun);
        
        // Actualizar high score
        SaveManager.Instance.SetHighScore(killsThisRun);
        
        Debug.Log($"Saved {permanentScrap} permanent scrap");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}