using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Ajustes")]
    public float speed = 3f;
    public int health = 2;
    public float shadowOffset = 0.3f;
    public Vector2 lightDirection = new Vector2(1, -1);
    [Range(0, 1)] public float shadowOpacity = 0.5f;
    
    [Header("Recompensas")]
    public int coinDropMin = 5;
    public int coinDropMax = 15;
    public float gemDropChance = 0.05f; // 5% chance de dropear gem
    
    private Transform player;
    private SpriteRenderer enemyRenderer;
    private SpriteRenderer shadowRenderer;
    private int maxHealth; // Para health bar

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
    }

    void Start()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

        maxHealth = health;

        // PARCHE: Desactivar sombra un instante para evitar el "flash" en el centro
        if (shadowRenderer != null) shadowRenderer.enabled = false;
        Invoke("EnableShadow", 0.1f); 
    }

    void EnableShadow() => shadowRenderer.enabled = true;

    void CreateShadow()
    {
        GameObject shadowObj = new GameObject("EnemyShadow");
        shadowObj.transform.parent = transform;
        shadowRenderer = shadowObj.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = enemyRenderer.sprite;
        shadowRenderer.color = new Color(0, 0, 0, shadowOpacity);
        shadowRenderer.sortingOrder = enemyRenderer.sortingOrder - 1;
        ActualizarPosicionSombra();
    }

    void LateUpdate()
    {
        if (player != null)
        {
            SeguirJugador();
            ActualizarPosicionSombra();
        }
    }

    void SeguirJugador()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Rotar para mirar al jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void ActualizarPosicionSombra()
    {
        if (shadowRenderer != null)
        {
            Vector3 offset = (Vector3)lightDirection.normalized * shadowOffset;
            shadowRenderer.transform.position = transform.position + offset;
            shadowRenderer.transform.rotation = transform.rotation;
            shadowRenderer.sprite = enemyRenderer.sprite;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        
        // Feedback visual de daño
        StartCoroutine(DamageFlash());
        
        // Mostrar número flotante de daño
        ShowDamageNumber(amount);
        
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Drop de recompensas
        DropRewards();
        
        // Efectos visuales de muerte
        SpawnDeathEffect();
        
        // Screen shake proporcional al enemigo
        if (CameraShake.Instance != null)
        {
            float shakeIntensity = 0.05f * (transform.localScale.x); // Boss shake más
            CameraShake.Instance.Shake(shakeIntensity, 0.1f);
        }
        
        Destroy(gameObject);
    }

    #region Rewards

    void DropRewards()
    {
        // Coins siempre
        int coinsDropped = Random.Range(coinDropMin, coinDropMax + 1);
        CurrencyManager.Instance.AddCoins(coinsDropped);
        
        // Chance de gem
        if (Random.value < gemDropChance)
        {
            CurrencyManager.Instance.AddGems(1);
            Debug.Log("💎 ¡Gem dropeada!");
        }
    }

    #endregion

    #region Visual Feedback

    System.Collections.IEnumerator DamageFlash()
    {
        if (enemyRenderer != null)
        {
            Color originalColor = enemyRenderer.color;
            enemyRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.color = originalColor;
        }
    }

    void SpawnDeathEffect()
    {
        // TODO: Instanciar particle system
        // Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        
        // Por ahora solo debug
        Debug.Log($"💀 Enemy murió en {transform.position}");
    }

    void ShowDamageNumber(int damage)
    {
        // TODO: Implementar floating damage numbers con pooling
        // FloatingTextPool.Instance.ShowNumber(transform.position, damage.ToString(), Color.yellow);
    }

    #endregion

    #region Collision con Player

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Damage al player
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            
            // El enemigo muere al tocar player (como Vampire Survivors)
            Die();
        }
    }

    #endregion
}