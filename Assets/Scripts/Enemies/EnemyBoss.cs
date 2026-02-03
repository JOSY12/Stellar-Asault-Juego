using UnityEngine;

public class EnemyBoss : Enemy
{
    [Header("Boss Settings")]
    public GameObject bulletPrefab;
    public int bulletsPerBurst = 5;
    public float burstFireRate = 0.2f;
    public float timeBetweenBursts = 3f;
    
    private float nextBurstTime;
    private int currentWave;

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats base del Boss (escalan con oleada)
        if (GameManager.Instance != null)
        {
            currentWave = GameManager.Instance.currentWave;
        }
        
        health = 50 + (currentWave * 10);
        speed = 1f;
        scrapValue = 50;
    }

    void Start()
    {
        base.Start();
        
        // Reproducir sonido de boss
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bossSpawnSFX);
    }

    void Update()
    {
        if (player != null && Time.time >= nextBurstTime)
        {
            StartCoroutine(FireBurst());
            nextBurstTime = Time.time + timeBetweenBursts;
        }
    }

    System.Collections.IEnumerator FireBurst()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            if (bulletPrefab != null && player != null)
            {
                // Calcular ángulo hacia el jugador con spread
                Vector2 direction = (player.position - transform.position).normalized;
                float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                float spread = (i - bulletsPerBurst / 2) * 15f; // 15 grados entre balas
                float finalAngle = baseAngle + spread;
                
                Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);
                GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
                
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(6f, 2);
                }
                
                bullet.tag = "EnemyBullet";
            }
            
            yield return new WaitForSeconds(burstFireRate);
        }
    }
}