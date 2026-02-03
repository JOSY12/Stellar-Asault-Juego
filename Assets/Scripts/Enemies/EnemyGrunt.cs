using UnityEngine;

public class EnemyGrunt : Enemy
{
    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats del Grunt
        health = 3;
        speed = 2.5f;
        scrapValue = 2;
    }
}