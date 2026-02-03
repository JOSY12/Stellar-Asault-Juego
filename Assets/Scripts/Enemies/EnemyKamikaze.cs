using UnityEngine;

public class EnemyKamikaze : Enemy
{
    [Header("Kamikaze Settings")]
    public int explosionDamage = 3;
    public float warningDistance = 3f;
    
    private SpriteRenderer spriteRenderer;
    private bool isWarning = false;

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer = enemyRenderer;
        CreateShadow();
        
        // Stats del Kamikaze
        health = 1;
        speed = 6f;
        scrapValue = 3;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            
            if (distance < warningDistance && !isWarning)
            {
                isWarning = true;
                StartCoroutine(FlashWarning());
            }
        }
    }

    System.Collections.IEnumerator FlashWarning()
    {
        Color originalColor = spriteRenderer.color;
        
        while (isWarning && player != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(explosionDamage);
            }
            
            // Explotar (morir)
            Die();
        }
    }
}