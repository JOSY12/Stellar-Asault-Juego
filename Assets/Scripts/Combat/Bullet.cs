using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;
    
    private Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}