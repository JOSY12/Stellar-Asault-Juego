using UnityEngine;

public class EnemySplitter : Enemy
{
    [Header("Splitter Settings")]
    public GameObject scoutPrefab;
    public int scoutsToSpawn = 2;

    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats del Splitter
        health = 4;
        speed = 3f;
        scrapValue = 6;
    }

    protected override void Die()
    {
        // Spawear scouts antes de morir
        if (scoutPrefab != null)
        {
            for (int i = 0; i < scoutsToSpawn; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
                Instantiate(scoutPrefab, (Vector2)transform.position + randomOffset, Quaternion.identity);
            }
        }
        
        // Morir normalmente
        base.Die();
    }
}