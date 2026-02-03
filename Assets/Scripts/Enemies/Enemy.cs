using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 2;
    public float speed = 3f;
    public int scrapValue = 1;
    
    [Header("Shadow Settings")]
    public float shadowOffset = 0.3f;
    public Vector2 lightDirection = new Vector2(1, -1);
    [Range(0, 1)] public float shadowOpacity = 0.5f;
    
    protected Transform player;
    protected SpriteRenderer enemyRenderer;
    protected SpriteRenderer shadowRenderer;
    
    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
    }
    
    void Start()
    {
        FindPlayer();
        
        // Desactivar sombra temporalmente para evitar flash
        if (shadowRenderer != null) shadowRenderer.enabled = false;
        Invoke("EnableShadow", 0.1f);
    }
    
    void FindPlayer()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }
    
    void EnableShadow()
    {
        if (shadowRenderer != null)
            shadowRenderer.enabled = true;
    }
    
    void CreateShadow()
    {
        GameObject shadowObj = new GameObject("EnemyShadow");
        shadowObj.transform.parent = transform;
        shadowRenderer = shadowObj.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = enemyRenderer.sprite;
        shadowRenderer.color = new Color(0, 0, 0, shadowOpacity);
        shadowRenderer.sortingOrder = enemyRenderer.sortingOrder - 1;
        UpdateShadowPosition();
    }
    
    void LateUpdate()
    {
        if (player != null)
        {
            FollowPlayer();
            UpdateShadowPosition();
        }
    }
    
    protected virtual void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        
        // Rotar hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void UpdateShadowPosition()
    {
        if (shadowRenderer != null)
        {
            Vector3 offset = (Vector3)lightDirection.normalized * shadowOffset;
            shadowRenderer.transform.position = transform.position + offset;
            shadowRenderer.transform.rotation = transform.rotation;
            shadowRenderer.sprite = enemyRenderer.sprite;
        }
    }
    
    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        // Notificar al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.AddKill();
        
        // Reproducir sonido de muerte
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeathSFX);
        
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(1);
            }
        }
    }
}