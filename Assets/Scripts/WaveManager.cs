using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema de oleadas con dificultad progresiva
/// Boss cada 5 oleadas para eventos especiales
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Configuración de Oleadas")]
    public int currentWave = 1;
    public float waveDuration = 30f;
    public float waveBreakDuration = 5f;
    
    [Header("Scaling de Dificultad")]
    public float enemyHealthMultiplier = 1.2f; // +20% HP por oleada
    public float enemySpeedMultiplier = 1.05f; // +5% velocidad por oleada
    public int baseEnemiesPerWave = 5;
    public float enemiesPerWaveIncrease = 1.5f;

    [Header("Recompensas")]
    public int baseCoinsPerWave = 50;
    public int bossWaveInterval = 5; // Boss cada 5 oleadas
    public float bossRewardMultiplier = 3f;

    [Header("Referencias")]
    public EnemySpawner spawner;

    // Estado
    private bool waveActive = false;
    private float waveTimer = 0f;
    private int enemiesSpawnedThisWave = 0;
    private int enemiesToSpawnThisWave = 0;

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
        LoadWaveProgress();
        StartCoroutine(WaveLoop());
    }

    #region Wave Loop

    IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return StartCoroutine(PrepareWave());
            yield return StartCoroutine(RunWave());
            yield return StartCoroutine(CompleteWave());
        }
    }

    IEnumerator PrepareWave()
    {
        waveActive = false;
        
        // UI: "Wave X starting in 5..."
        Debug.Log($"Preparando oleada {currentWave}...");
        
        // Calcular enemigos para esta oleada
        enemiesToSpawnThisWave = Mathf.RoundToInt(baseEnemiesPerWave * Mathf.Pow(enemiesPerWaveIncrease, currentWave - 1));
        enemiesSpawnedThisWave = 0;
        
        // Verificar si es oleada de boss
        bool isBossWave = (currentWave % bossWaveInterval == 0);
        
        if (isBossWave)
        {
            Debug.Log("⚠️ BOSS WAVE INCOMING!");
            // UIManager.Instance.ShowBossWarning();
        }
        
        yield return new WaitForSeconds(waveBreakDuration);
    }

    IEnumerator RunWave()
    {
        waveActive = true;
        waveTimer = 0f;
        
        Debug.Log($"🌊 OLEADA {currentWave} INICIADA!");
        
        bool isBossWave = (currentWave % bossWaveInterval == 0);
        
        if (isBossWave)
        {
            SpawnBoss();
        }
        else
        {
            // Spawn enemigos normales durante la oleada
            float spawnInterval = waveDuration / enemiesToSpawnThisWave;
            
            while (enemiesSpawnedThisWave < enemiesToSpawnThisWave)
            {
                SpawnScaledEnemy();
                enemiesSpawnedThisWave++;
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        // Esperar a que termine la oleada o mueran todos los enemigos
        while (waveTimer < waveDuration)
        {
            waveTimer += Time.deltaTime;
            
            // Si mataron a todos los enemigos antes, terminar oleada early
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && enemiesSpawnedThisWave >= enemiesToSpawnThisWave)
            {
                Debug.Log("¡Todos los enemigos eliminados!");
                break;
            }
            
            yield return null;
        }
    }

    IEnumerator CompleteWave()
    {
        waveActive = false;
        
        // Calcular recompensa
        bool isBossWave = (currentWave % bossWaveInterval == 0);
        int waveReward = CalculateWaveReward(isBossWave);
        
        // Dar recompensa
        CurrencyManager.Instance.AddCoins(waveReward);
        
        Debug.Log($"✅ Oleada {currentWave} completada! +{waveReward} coins");
        
        // UI: Mostrar popup de wave complete
        // UIManager.Instance.ShowWaveComplete(currentWave, waveReward, isBossWave);
        
        // Si fue boss, ofrecer AD para x3 reward
        if (isBossWave)
        {
            // UIManager.Instance.ShowBossRewardAdOption(waveReward);
            Debug.Log($"💎 ¡Boss derrotado! Ver AD para {waveReward * 3} coins total");
        }
        
        currentWave++;
        SaveWaveProgress();
        
        yield return new WaitForSeconds(2f);
    }

    #endregion

    #region Spawning

    void SpawnScaledEnemy()
    {
        if (spawner == null || spawner.enemyPrefab == null) return;

        // Spawn en posición aleatoria fuera de pantalla
        Vector2 spawnPos = GetRandomSpawnPosition();
        GameObject enemyObj = Instantiate(spawner.enemyPrefab, spawnPos, Quaternion.identity);
        
        // Aplicar scaling de oleada
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.health = Mathf.RoundToInt(enemy.health * Mathf.Pow(enemyHealthMultiplier, currentWave - 1));
            enemy.speed = enemy.speed * Mathf.Pow(enemySpeedMultiplier, currentWave - 1);
        }
    }

    void SpawnBoss()
    {
        if (spawner == null || spawner.enemyPrefab == null) return;

        Vector2 spawnPos = GetRandomSpawnPosition();
        GameObject bossObj = Instantiate(spawner.enemyPrefab, spawnPos, Quaternion.identity);
        
        // Boss es versión super-powered del enemigo normal
        Enemy boss = bossObj.GetComponent<Enemy>();
        if (boss != null)
        {
            boss.health = Mathf.RoundToInt(boss.health * Mathf.Pow(enemyHealthMultiplier, currentWave - 1) * 5);
            boss.speed = boss.speed * Mathf.Pow(enemySpeedMultiplier, currentWave - 1) * 0.8f; // Un poco más lento
            
            // Visual: hacer el boss más grande
            bossObj.transform.localScale = Vector3.one * 2f;
        }
        
        enemiesSpawnedThisWave = enemiesToSpawnThisWave; // Contar boss como todos los enemigos
    }

    Vector2 GetRandomSpawnPosition()
    {
        float distance = spawner.spawnRadius;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)spawner.transform.position + randomDirection * distance;
    }

    #endregion

    #region Rewards

    int CalculateWaveReward(bool isBossWave)
    {
        float reward = baseCoinsPerWave * currentWave;
        
        if (isBossWave)
        {
            reward *= bossRewardMultiplier;
        }
        
        return Mathf.RoundToInt(reward);
    }

    /// <summary>
    /// Claim boss reward con AD (x3)
    /// </summary>
    public void ClaimBossRewardWithAd()
    {
        int baseReward = CalculateWaveReward(true);
        
        // AdManager.Instance.ShowRewardedAd(() => {
            CurrencyManager.Instance.AddCoins(baseReward * 2); // Ya recibieron x1, esto da el x2 extra
            Debug.Log($"¡AD vista! +{baseReward * 2} coins extra");
        // });
    }

    #endregion

    #region Save/Load

    void SaveWaveProgress()
    {
        PlayerPrefs.SetInt("CurrentWave", currentWave);
        PlayerPrefs.Save();
    }

    void LoadWaveProgress()
    {
        currentWave = PlayerPrefs.GetInt("CurrentWave", 1);
    }

    #endregion

    #region Public Getters

    public bool IsWaveActive() => waveActive;
    public float GetWaveProgress() => waveTimer / waveDuration;
    public int GetCurrentWave() => currentWave;
    
    #endregion
}