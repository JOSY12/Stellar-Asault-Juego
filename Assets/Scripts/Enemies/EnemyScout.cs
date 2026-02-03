using UnityEngine;

public class EnemyScout : Enemy
{
    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats del Scout
        health = 1;
        speed = 5f;
        scrapValue = 1;
    }
}