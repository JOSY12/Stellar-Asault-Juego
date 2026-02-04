using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject scoutPrefab;
    public GameObject gruntPrefab;
    public GameObject kamikazePrefab;
    public GameObject tankPrefab;
    public GameObject sniperPrefab;
    public GameObject splitterPrefab;
    public GameObject zigzagPrefab;
    public GameObject bossPrefab;
    
    [Header("Spawn Settings")]
    public float spawnRadius = 12f;
    public float baseSpawnRate = 2f;
    public float minSpawnRate = 0.3f;
    public float spawnRateDecrease = 0.05f;
    
    [Header("Enemy Count Settings")]
    public int baseEnemiesPerWave = 3;
    public int maxEnemiesPerWave = 15;
    
    private Transform player;
    private float currentSpawnRate;
    private float nextSpawnTime;
    private int currentWave = 0;
    private bool bossSpawned = false;
    
    void Start()
    {
        FindPlayer();
        currentSpawnRate = baseSpawnRate;
        nextSpawnTime = Time.time + 1f;
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
    
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;
        
        // Actualizar oleada actual
        if (GameManager.Instance != null)
        {
            currentWave = GameManager.Instance.currentWave;
        }
        
        // Spawn de Boss cada 10 oleadas
        if (currentWave > 0 && currentWave % 10 == 0 && !bossSpawned)
        {
            SpawnBoss();
            bossSpawned = true;
            return; // No spawear enemigos normales en oleada de boss
        }
        
        // Resetear flag de boss cuando pase la oleada
        if (currentWave % 10 != 0)
        {
            bossSpawned = false;
        }
        
        // Sistema de spawn continuo
        if (Time.time >= nextSpawnTime)
        {
            SpawnWave();
            UpdateSpawnRate();
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }
    
    void SpawnWave()
    {
        // Calcular cuántos enemigos spawear según oleada
        int enemiesToSpawn = Mathf.Min(
            baseEnemiesPerWave + (currentWave / 5),
            maxEnemiesPerWave
        );
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnRandomEnemy();
        }
    }
    
    void SpawnRandomEnemy()
    {
        GameObject enemyToSpawn = ChooseEnemyByWave();
        
        if (enemyToSpawn == null) return;
        
        Vector2 spawnPosition = GetRandomSpawnPosition();
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }
    
    GameObject ChooseEnemyByWave()
    {
        List<GameObject> availableEnemies = new List<GameObject>();
        
        // Oleadas 1-5: Solo básicos
        if (currentWave <= 5)
        {
            if (scoutPrefab != null) availableEnemies.Add(scoutPrefab);
            if (gruntPrefab != null) availableEnemies.Add(gruntPrefab);
        }
        // Oleadas 6-10: Agregar Kamikaze y Tank
        else if (currentWave <= 10)
        {
            if (scoutPrefab != null) availableEnemies.Add(scoutPrefab);
            if (gruntPrefab != null) availableEnemies.Add(gruntPrefab);
            if (kamikazePrefab != null) availableEnemies.Add(kamikazePrefab);
            if (tankPrefab != null) availableEnemies.Add(tankPrefab);
        }
        // Oleadas 11-20: Agregar especiales
        else if (currentWave <= 20)
        {
            if (scoutPrefab != null) availableEnemies.Add(scoutPrefab);
            if (gruntPrefab != null) availableEnemies.Add(gruntPrefab);
            if (kamikazePrefab != null) availableEnemies.Add(kamikazePrefab);
            if (tankPrefab != null) availableEnemies.Add(tankPrefab);
            if (sniperPrefab != null) availableEnemies.Add(sniperPrefab);
            if (zigzagPrefab != null) availableEnemies.Add(zigzagPrefab);
            if (splitterPrefab != null) availableEnemies.Add(splitterPrefab);
        }
        // Oleadas 21+: Todos mezclados
        else
        {
            if (scoutPrefab != null) availableEnemies.Add(scoutPrefab);
            if (gruntPrefab != null) availableEnemies.Add(gruntPrefab);
            if (kamikazePrefab != null) availableEnemies.Add(kamikazePrefab);
            if (tankPrefab != null) availableEnemies.Add(tankPrefab);
            if (sniperPrefab != null) availableEnemies.Add(sniperPrefab);
            if (zigzagPrefab != null) availableEnemies.Add(zigzagPrefab);
            if (splitterPrefab != null) availableEnemies.Add(splitterPrefab);
            
            // Más enemigos fuertes en oleadas altas
            if (tankPrefab != null) availableEnemies.Add(tankPrefab);
            if (sniperPrefab != null) availableEnemies.Add(sniperPrefab);
        }
        
        if (availableEnemies.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return null;
        }
        
        return availableEnemies[Random.Range(0, availableEnemies.Count)];
    }
    
    void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab not assigned!");
            return;
        }
        
        Vector2 spawnPosition = GetRandomSpawnPosition();
        Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        
        Debug.Log($"BOSS spawned at wave {currentWave}!");
    }
    
    Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPosition;
        
        if (player != null)
        {
            // Spawn fuera de la pantalla, alrededor del jugador
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            spawnPosition = (Vector2)player.position + randomDirection * spawnRadius;
        }
        else
        {
            // Fallback: spawn alrededor del centro
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            spawnPosition = randomDirection * spawnRadius;
        }
        
        return spawnPosition;
    }
    
    void UpdateSpawnRate()
    {
        // Hacer spawn más rápido conforme avanzan las oleadas
        currentSpawnRate = baseSpawnRate - (currentWave * spawnRateDecrease);
        currentSpawnRate = Mathf.Max(currentSpawnRate, minSpawnRate);
    }
}
// ```

// ---

// ## 🔧 CONFIGURAR ENEMYSPAWNER EN UNITY

// ### **PASO 1: Seleccionar EnemySpawner en Hierarchy**

// 1. Click en `EnemySpawner` en Hierarchy
// 2. En Inspector, verás todos los campos nuevos

// ### **PASO 2: Asignar todos los prefabs**
// ```
// Enemy Prefabs:
// ├─ Scout Prefab: [Arrastra Enemy_Scout desde Prefabs/Enemies/Basic/]
// ├─ Grunt Prefab: [Arrastra Enemy_Grunt]
// ├─ Kamikaze Prefab: [Arrastra Enemy_Kamikaze desde Special/]
// ├─ Tank Prefab: [Arrastra Enemy_Tank]
// ├─ Sniper Prefab: [Arrastra Enemy_Sniper]
// ├─ Splitter Prefab: [Arrastra Enemy_Splitter]
// ├─ Zigzag Prefab: [Arrastra Enemy_Zigzag]
// └─ Boss Prefab: [Arrastra Enemy_Boss desde Boss/]

// Spawn Settings:
// ├─ Spawn Radius: 12
// ├─ Base Spawn Rate: 2
// ├─ Min Spawn Rate: 0.3
// └─ Spawn Rate Decrease: 0.05

// Enemy Count Settings:
// ├─ Base Enemies Per Wave: 3
// └─ Max Enemies Per Wave: 15


// ```

// ---

// ## ✅ MANTENER ESTOS SCRIPTS (Ya los tienes)

// **NO BORRES:**
// - `ShadowController.cs` - Funciona perfecto ✅
// - `VirtualJoystick.cs` - Lo usaremos ✅
// - `CameraShake.cs` - Funciona perfecto ✅

// **PUEDES BORRAR:**
// - `Bala.cs` - Reemplazado por `Bullet.cs`
// - `Enemy.cs` viejo - Reemplazado por el nuevo
// - `PlayerController.cs` viejo - Reemplazado
// - `EnemySpawner.cs` viejo - Reemplazado

// ---

// ## 🎨 PALETAS DE COLOR (Para FASE 2)
// ```
// PALETA 1 - NEON (Gratis, Default)
// ├─ Background: #000000 (Negro)
// ├─ Player: #00FFFF (Cyan)
// ├─ Bullet: #FFFFFF (Blanco)
// ├─ Enemy Scout: #00FF00 (Verde)
// ├─ Enemy Grunt: #FFFF00 (Amarillo)
// └─ Enemy Kamikaze: #FF0000 (Rojo)

// PALETA 2 - RETRO (Gratis)
// ├─ Background: #0F380F (Verde Game Boy oscuro)
// ├─ Player: #9BBC0F (Verde Game Boy claro)
// ├─ Bullet: #8BAC0F
// ├─ Enemies: #306230 (Verde medio)

// PALETA 3 - BLOOD (1 Ad)
// ├─ Background: #1A0000 (Negro rojizo)
// ├─ Player: #FF3333 (Rojo brillante)
// ├─ Bullet: #FF0000
// ├─ Enemies: #800000 (Rojo oscuro)

// PALETA 4 - OCEAN (1 Ad)
// ├─ Background: #001A33 (Azul profundo)
// ├─ Player: #00BFFF (Azul cielo)
// ├─ Bullet: #FFFFFF
// ├─ Enemies: #004D99 (Azul medio)

// PALETA 5 - CYBERPUNK (2 Ads)
// ├─ Background: #0D0221 (Púrpura oscuro)
// ├─ Player: #FF006E (Magenta)
// ├─ Bullet: #FFBE0B (Naranja)
// ├─ Enemies: #8338EC (Púrpura)

// PALETA 6 - MONOCHROME (3 Ads)
// ├─ Background: #000000 (Negro)
// ├─ Player: #FFFFFF (Blanco)
// ├─ Bullet: #CCCCCC (Gris claro)
// ├─ Enemies: #666666 (Gris medio)