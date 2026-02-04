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
    public float waveDuration = 30f; // Duración de cada oleada en segundos
    private float waveTimer = 0f;
    
    [Header("Economy")]
    public int scrapThisRun = 0;
    public int killsThisRun = 0;
    
    [Header("Player Reference")]
    public PlayerController player;
    
    [Header("UI References (Optional)")]
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI killsText;
    public TMPro.TextMeshProUGUI scrapText;
    
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
        
        // Reproducir música de gameplay
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
        
        Debug.Log("Game Started!");
    }
    
    void NextWave()
    {
        currentWave++;
        Debug.Log($"=== WAVE {currentWave} ===");
        
        // Mensaje especial para oleadas de boss
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
        
        // Dar scrap por kill (escala con oleada)
        int scrapReward = 1 + (currentWave / 10);
        AddScrap(scrapReward);
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
        
        Debug.Log("============================");
        Debug.Log($"GAME OVER!");
        Debug.Log($"Wave Reached: {currentWave}");
        Debug.Log($"Kills: {killsThisRun}");
        Debug.Log($"Scrap Earned: {scrapThisRun}");
        Debug.Log($"Permanent Scrap Saved: {Mathf.FloorToInt(scrapThisRun * 0.1f)}");
        Debug.Log("============================");
        
        // Reiniciar después de 3 segundos (FASE 4 mostrará DeathScreen)
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
    
    void UpdateUI()
    {
        // Actualizar textos si existen
        if (waveText != null)
            waveText.text = $"WAVE {currentWave}";
        
        if (killsText != null)
            killsText.text = $"KILLS: {killsThisRun}";
        
        if (scrapText != null)
            scrapText.text = $"SCRAP: {scrapThisRun}";
    }
}
// ```

// ---

// ## 🎮 CREAR HUD SIMPLE (OPCIONAL - FASE 4 lo mejoraremos)

// Si quieres ver las stats en pantalla:

// ### **PASO 1: Crear textos en Canvas**

// 1. Selecciona `GameplayUI` (tu Canvas)
// 2. Click derecho → UI → Text - TextMeshPro

// 3. Crear 3 textos:
//    - `WaveText` (arriba centro)
//    - `KillsText` (arriba izquierda)
//    - `ScrapText` (arriba derecha)

// ### **PASO 2: Configurar textos**

// **WaveText:**
// ```
// Rect Transform:
// ├─ Anchor: Top Center
// ├─ Pos X: 0, Pos Y: -50
// ├─ Width: 300, Height: 60

// Text:
// ├─ Text: "WAVE 1"
// ├─ Font Size: 36
// ├─ Alignment: Center
// ├─ Color: Blanco

// Shadow Component:
// ├─ Effect Distance: (2, -2)
// └─ Effect Color: Negro
// ```

// **KillsText:**
// ```
// Rect Transform:
// ├─ Anchor: Top Left
// ├─ Pos X: 20, Pos Y: -20
// ├─ Width: 200, Height: 40

// Text:
// ├─ Text: "KILLS: 0"
// ├─ Font Size: 24
// ├─ Alignment: Left
// └─ Color: Blanco
// ```

// **ScrapText:**
// ```
// Rect Transform:
// ├─ Anchor: Top Right
// ├─ Pos X: -20, Pos Y: -20
// ├─ Width: 200, Height: 40

// Text:
// ├─ Text: "SCRAP: 0"
// ├─ Font Size: 24
// ├─ Alignment: Right
// └─ Color: Amarillo (255, 220, 100)
// ```

// ### **PASO 3: Asignar al GameManager**

// 1. Selecciona `_Managers`
// 2. GameManager:
// ```
//    UI References:
//    ├─ Wave Text: [Arrastra WaveText]
//    ├─ Kills Text: [Arrastra KillsText]
//    └─ Scrap Text: [Arrastra ScrapText]
// ```

// ---

// ## 🎯 PROBAR EL SISTEMA COMPLETO

// ### **Dale Play y verifica:**

// ✅ **Oleadas 1-5:**
// - Solo aparecen Scout y Grunt
// - Spawn lento

// ✅ **Oleadas 6-10:**
// - Aparecen Kamikaze y Tank
// - Spawn más rápido

// ✅ **Oleada 10:**
// - Aparece un BOSS
// - No aparecen enemigos normales durante la oleada

// ✅ **Oleadas 11+:**
// - Todos los tipos mezclados
// - Mucha acción

// ✅ **HUD:**
// - Se actualiza correctamente
// - Muestra oleada, kills, scrap

// ---

// ## 📊 BALANCEO ACTUAL
// ```
// PROGRESIÓN DE DIFICULTAD:
// ├─ Oleada 1: 3 enemigos cada 2s
// ├─ Oleada 10: 5 enemigos cada 1.5s + BOSS
// ├─ Oleada 20: 7 enemigos cada 1s + BOSS
// ├─ Oleada 30: 9 enemigos cada 0.65s + BOSS
// └─ Oleada 50+: 15 enemigos cada 0.3s

// SCRAP POR KILL:
// ├─ Oleadas 1-9: 1 scrap
// ├─ Oleadas 10-19: 2 scrap
// ├─ Oleadas 20-29: 3 scrap
// └─ Etc...