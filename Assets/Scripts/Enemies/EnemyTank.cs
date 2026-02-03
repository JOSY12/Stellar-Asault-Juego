using UnityEngine;

public class EnemyTank : Enemy
{
    void Awake()
    {
        enemyRenderer = GetComponent<SpriteRenderer>();
        CreateShadow();
        
        // Stats del Tank
        health = 10;
        speed = 1.5f;
        scrapValue = 5;
    }
}