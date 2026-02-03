using UnityEngine;

public class EnemyZigzag : Enemy
{
    [Header("Zigzag Settings")]
    public float zigzagFrequency = 2f;
    public float zigzagAmplitude = 2f;
    
    private Vector2 perpendicularDirection;
    private float zigzagTimer;

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats del Zigzag
        health = 2;
        speed = 4f;
        scrapValue = 3;
    }

    protected override void FollowPlayer()
    {
        if (player == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Calcular dirección perpendicular
        perpendicularDirection = new Vector2(-direction.y, direction.x);
        
        // Zigzag
        zigzagTimer += Time.deltaTime * zigzagFrequency;
        float zigzagOffset = Mathf.Sin(zigzagTimer) * zigzagAmplitude;
        
        Vector2 movement = direction + perpendicularDirection * zigzagOffset * 0.5f;
        movement = movement.normalized;
        
        transform.position += (Vector3)movement * speed * Time.deltaTime;
        
        // Rotar hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}