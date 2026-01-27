using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Ajustes")]
    public float speed = 3f;
    public int health = 2;
    public float shadowOffset = 0.3f;
    public Vector2 lightDirection = new Vector2(1, -1);
[Range(0, 1)] public float shadowOpacity = 0.5f; // 0 es invisible, 1 es negro total
    private Transform player;
    private SpriteRenderer enemyRenderer;
    private SpriteRenderer shadowRenderer;

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
    }

    void Start()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

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
        // Aplicamos la opacidad aquí
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
        if (health <= 0) Destroy(gameObject);
    }
}