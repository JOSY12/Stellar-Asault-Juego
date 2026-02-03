using UnityEngine;

public class EnemySniper : Enemy
{
    [Header("Sniper Settings")]
    public GameObject bulletPrefab;
    public float fireRate = 3f;
    public float fireRange = 8f;
    public float keepDistance = 6f;
    
    private float nextFireTime;

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats del Sniper
        health = 2;
        speed = 2f;
        scrapValue = 4;
    }

    protected override void FollowPlayer()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Si está muy cerca, huir
        if (distance < keepDistance)
        {
            direction = -direction; // Invertir dirección
        }
        // Si está muy lejos, acercarse
        else if (distance > fireRange)
        {
            // Mantener dirección normal
        }
        // Si está en rango óptimo, no moverse
        else
        {
            direction = Vector2.zero;
        }
        
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        
        // Siempre mirar al jugador
        Vector2 lookDirection = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Disparar si está en rango
        if (distance <= fireRange && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        
        // Configurar bala enemiga
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(8f, 1);
        }
        
        // Cambiar tag para que no dañe a otros enemigos
        bullet.tag = "EnemyBullet";
        
        nextFireTime = Time.time + fireRate;
    }
}