using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    
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
        // Destruir después del lifetime
        Destroy(gameObject, lifeTime);
        
        // Mover la bala hacia adelante
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
                
                // Sonido de hit
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHitSFX);
            }
            
            Destroy(gameObject);
        }
    }
    
    public void Initialize(float speed, int damage)
    {
        this.speed = speed;
        this.damage = damage;
    }
    
    // ← NUEVO: Método para aplicar visual de la bala
    public void ApplyBulletData(BulletData data)
{
    if (data == null) return;
    
    // Aplicar sprite
    if (spriteRenderer != null && data.bulletSprite != null)
    {
        spriteRenderer.sprite = data.bulletSprite;
        
        // Aplicar color de la paleta actual en lugar del color del BulletData
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
    
    // Aplicar trail si existe
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
        
        // Color del trail también usa la paleta
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