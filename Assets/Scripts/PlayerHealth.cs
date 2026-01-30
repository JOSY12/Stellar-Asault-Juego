using UnityEngine;
using System;

/// <summary>
/// Sistema de vida del jugador con opción de continuar con AD
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    
    [Header("Invulnerabilidad")]
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;
    
    [Header("Referencias")]
    public SpriteRenderer spriteRenderer;
    
    // Eventos
    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action OnPlayerDied;
    
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Feedback visual
        StartCoroutine(DamageEffect());
        
        // Screen shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.2f, 0.15f);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Iniciar invulnerabilidad temporal
            StartCoroutine(InvulnerabilityFrames());
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Efecto visual de curación
        StartCoroutine(HealEffect());
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        OnPlayerDied?.Invoke();
        
        Debug.Log("💀 Player murió!");
        
        // Pausar juego
        Time.timeScale = 0f;
        
        // Mostrar pantalla de Game Over con opción de continuar
        ShowGameOverScreen();
    }

    void ShowGameOverScreen()
    {
        // UIManager.Instance.ShowGameOver(WaveManager.Instance.GetCurrentWave());
        Debug.Log("Mostrando pantalla de Game Over...");
        
        // Ofrecer continuar con AD si está en oleada decente
        if (WaveManager.Instance != null && WaveManager.Instance.GetCurrentWave() >= 5)
        {
            Debug.Log("💎 Opción: Continuar con AD disponible");
            // UIManager.Instance.ShowContinueWithAdOption();
        }
    }

    /// <summary>
    /// Continuar jugando después de ver AD
    /// </summary>
    public void ContinueWithAd()
    {
        // AdManager.Instance.ShowRewardedAd(() => {
            currentHealth = maxHealth;
            isDead = false;
            Time.timeScale = 1f;
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            // Efecto visual de revivir
            StartCoroutine(ReviveEffect());
            
            Debug.Log("✨ ¡Revivido con AD!");
        // });
    }

    /// <summary>
    /// Restart sin AD (perder progreso de oleada)
    /// </summary>
    public void RestartGame()
    {
        isDead = false;
        Time.timeScale = 1f;
        
        // Reset oleada
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.currentWave = 1;
        }
        
        // Recargar escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    #region Visual Effects

    System.Collections.IEnumerator DamageEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            // Flash rojo
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    System.Collections.IEnumerator HealEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            // Flash verde
            spriteRenderer.color = Color.green;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = originalColor;
        }
    }

    System.Collections.IEnumerator ReviveEffect()
    {
        if (spriteRenderer != null)
        {
            // Efecto de parpadeo dorado
            Color originalColor = spriteRenderer.color;
            
            for (int i = 0; i < 5; i++)
            {
                spriteRenderer.color = Color.yellow;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // Invulnerabilidad post-revive
        StartCoroutine(InvulnerabilityFrames());
    }

    System.Collections.IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            float elapsed = 0f;
            
            while (elapsed < invulnerabilityDuration)
            {
                // Parpadeo
                spriteRenderer.color = new Color(
                    originalColor.r, 
                    originalColor.g, 
                    originalColor.b, 
                    Mathf.PingPong(elapsed * 10f, 1f)
                );
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityDuration);
        }
        
        isInvulnerable = false;
    }

    #endregion

    #region Public Methods

    public void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    #endregion
}