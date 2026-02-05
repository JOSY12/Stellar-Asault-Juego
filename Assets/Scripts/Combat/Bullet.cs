using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    
    [Header("Knockback")]
    public float knockbackForce = 3f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trail;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
    }
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
        
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Si es bala enemiga, solo daña al jugador
        if (gameObject.CompareTag("EnemyBullet"))
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
            return;
        }
        
        // Si es bala del jugador, solo daña enemigos
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                
                // Aplicar knockback
                ApplyKnockback(collision);
                
                // Sonido de hit
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHitSFX);
            }
            
            Destroy(gameObject);
        }
    }
    
    void ApplyKnockback(Collider2D enemyCollider)
    {
        Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 knockbackDir = transform.up;
            
            // ← CAMBIO: Usar bodyType en lugar de isKinematic
            bool wasKinematic = (enemyRb.bodyType == RigidbodyType2D.Kinematic);
            
            if (wasKinematic)
            {
                enemyRb.bodyType = RigidbodyType2D.Dynamic; // ← CAMBIO
            }
            
            enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            
            if (wasKinematic && enemyCollider != null)
            {
                StartCoroutine(ResetKinematic(enemyRb, 0.1f));
            }
        }
    }
    
    System.Collections.IEnumerator ResetKinematic(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // ← CAMBIO
        }
    }
    
    public void Initialize(float speed, int damage)
    {
        this.speed = speed;
        this.damage = damage;
    }
    
    public void ApplyBulletData(BulletData data)
    {
        if (data == null) return;
        
        if (spriteRenderer != null && data.bulletSprite != null)
        {
            spriteRenderer.sprite = data.bulletSprite;
            
            Color finalColor = data.bulletColor;
            if (PaletteManager.Instance != null)
            {
                PaletteData palette = PaletteManager.Instance.GetCurrentPalette();
                if (palette != null)
                {
                    finalColor = palette.playerBulletColor;
                }
            }
            
            spriteRenderer.color = finalColor;
            transform.localScale = new Vector3(data.bulletScale.x, data.bulletScale.y, 1);
        }
        
        if (data.hasTrail)
        {
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
            }
            
            trail.time = data.trailTime;
            trail.startWidth = data.trailWidth;
            trail.endWidth = 0f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            
            Color trailColor = data.trailColor;
            if (PaletteManager.Instance != null)
            {
                PaletteData palette = PaletteManager.Instance.GetCurrentPalette();
                if (palette != null)
                {
                    trailColor = palette.playerBulletColor;
                }
            }
            
            trail.startColor = trailColor;
            trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0);
        }
    }
}