using UnityEngine;

public class Bala : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;
    public bool isCritical = false;
    
    [Header("Trail Effect")]
    public TrailRenderer trail;
    public Color normalTrailColor = Color.white;
    public Color criticalTrailColor = Color.yellow;
    
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Configurar trail si existe
        if (trail != null)
        {
            trail.startColor = isCritical ? criticalTrailColor : normalTrailColor;
        }
        
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar otros objetos que no sean enemigos
        if (!collision.CompareTag("Enemy")) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            
            // Efectos especiales para crítico
            if (isCritical)
            {
                SpawnCriticalEffect(collision.transform.position);
                
                // Screen shake más fuerte
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.Shake(0.15f, 0.1f);
                }
                
                // Slow-mo breve
                StartCoroutine(CriticalSlowMo());
            }
        }
        
        // Destruir bala después de impactar
        Destroy(gameObject);
    }

    void SpawnCriticalEffect(Vector3 position)
    {
        // TODO: Instanciar particle system de crítico
        // Instantiate(criticalParticlesPrefab, position, Quaternion.identity);
        
        Debug.Log("⭐ CRITICAL HIT!");
    }

    System.Collections.IEnumerator CriticalSlowMo()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = originalTimeScale;
    }
}