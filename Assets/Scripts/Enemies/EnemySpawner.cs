using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnRadius = 10f;
    public float timeBetweenSpawns = 2f;
    
    [Header("Wave Scaling")]
    public float spawnRateDecrease = 0.05f; // Se hace más rápido cada oleada
    public float minSpawnRate = 0.5f;
    
    private Transform player;
    private float currentSpawnRate;
    
    void Start()
    {
        FindPlayer();
        currentSpawnRate = timeBetweenSpawns;
        InvokeRepeating("SpawnEnemy", 1f, currentSpawnRate);
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
    
    void Update()
    {
        // Ajustar spawn rate según oleada
        if (GameManager.Instance != null)
        {
            float newRate = timeBetweenSpawns - (GameManager.Instance.currentWave * spawnRateDecrease);
            newRate = Mathf.Max(newRate, minSpawnRate);
            
            if (newRate != currentSpawnRate)
            {
                currentSpawnRate = newRate;
                CancelInvoke("SpawnEnemy");
                InvokeRepeating("SpawnEnemy", 0f, currentSpawnRate);
            }
        }
    }
    
    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        
        Vector2 spawnPosition;
        
        if (player != null)
        {
            // Spawn alrededor del jugador
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            spawnPosition = (Vector2)player.position + randomDirection * spawnRadius;
        }
        else
        {
            // Spawn alrededor del centro
            spawnPosition = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;
        }
        
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
```

---

## ✅ MANTENER ESTOS SCRIPTS (Ya los tienes)

**NO BORRES:**
- `ShadowController.cs` - Funciona perfecto ✅
- `VirtualJoystick.cs` - Lo usaremos ✅
- `CameraShake.cs` - Funciona perfecto ✅

**PUEDES BORRAR:**
- `Bala.cs` - Reemplazado por `Bullet.cs`
- `Enemy.cs` viejo - Reemplazado por el nuevo
- `PlayerController.cs` viejo - Reemplazado
- `EnemySpawner.cs` viejo - Reemplazado

---

## 🎨 PALETAS DE COLOR (Para FASE 2)
```
PALETA 1 - NEON (Gratis, Default)
├─ Background: #000000 (Negro)
├─ Player: #00FFFF (Cyan)
├─ Bullet: #FFFFFF (Blanco)
├─ Enemy Scout: #00FF00 (Verde)
├─ Enemy Grunt: #FFFF00 (Amarillo)
└─ Enemy Kamikaze: #FF0000 (Rojo)

PALETA 2 - RETRO (Gratis)
├─ Background: #0F380F (Verde Game Boy oscuro)
├─ Player: #9BBC0F (Verde Game Boy claro)
├─ Bullet: #8BAC0F
├─ Enemies: #306230 (Verde medio)

PALETA 3 - BLOOD (1 Ad)
├─ Background: #1A0000 (Negro rojizo)
├─ Player: #FF3333 (Rojo brillante)
├─ Bullet: #FF0000
├─ Enemies: #800000 (Rojo oscuro)

PALETA 4 - OCEAN (1 Ad)
├─ Background: #001A33 (Azul profundo)
├─ Player: #00BFFF (Azul cielo)
├─ Bullet: #FFFFFF
├─ Enemies: #004D99 (Azul medio)

PALETA 5 - CYBERPUNK (2 Ads)
├─ Background: #0D0221 (Púrpura oscuro)
├─ Player: #FF006E (Magenta)
├─ Bullet: #FFBE0B (Naranja)
├─ Enemies: #8338EC (Púrpura)

PALETA 6 - MONOCHROME (3 Ads)
├─ Background: #000000 (Negro)
├─ Player: #FFFFFF (Blanco)
├─ Bullet: #CCCCCC (Gris claro)
├─ Enemies: #666666 (Gris medio)