using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento (Joystick Izquierdo)")]
    public VirtualJoystick joystickMov; 
    public float moveSpeed = 8f;
    public Rigidbody2D rb;

    [Header("Disparo (Joystick Derecho)")]
    public VirtualJoystick joystickVis; 
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.1f;
    public float spread = 12f;
    
    [Header("Multi-Shot")]
    public int bulletCount = 1; // Se actualiza desde UpgradeManager
    public float multiShotAngle = 15f; // Ángulo entre balas

    [Header("Feedback de Cámara")]
    public float shakeIntensity = 0.12f;
    public float shakeDuration = 0.08f;

    [Header("Critical Hit System")]
    public float criticalChance = 0f; // Se actualiza desde UpgradeManager
    public float criticalDamageMultiplier = 3f;
    
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float nextFireTime;

    void Update()
    {
        // Capturar datos de ambos joysticks
        moveInput = new Vector2(joystickMov.Horizontal, joystickMov.Vertical);
        lookInput = new Vector2(joystickVis.Horizontal, joystickVis.Vertical);

        // Lógica de rotación y disparo con el joystick derecho
        if (lookInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;

            if (Time.time >= nextFireTime)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) 
        {
            Debug.LogError("¡Falta el Bullet Prefab en el PlayerController!");
            return;
        }

        // Disparar múltiples balas según upgrade
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = CalculateBulletAngle(i);
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            
            // Aplicar crítico si corresponde
            if (Random.value < criticalChance)
            {
                ApplyCriticalToBullet(bullet);
            }
        }
        
        // Camera shake proporcional a cantidad de balas
        if (CameraShake.Instance != null)
        {
            float finalShake = shakeIntensity * Mathf.Sqrt(bulletCount);
            CameraShake.Instance.Shake(finalShake, shakeDuration);
        }

        nextFireTime = Time.time + fireRate;
    }

    float CalculateBulletAngle(int bulletIndex)
    {
        // Spread aleatorio base
        float randomSpread = Random.Range(-spread, spread);
        
        if (bulletCount == 1)
        {
            return randomSpread;
        }
        
        // Multi-shot pattern
        if (bulletCount == 2)
        {
            // Dos balas: ±45°
            return (bulletIndex == 0 ? -45f : 45f) + randomSpread;
        }
        else if (bulletCount == 3)
        {
            // Tres balas: -45°, 0°, 45°
            return (bulletIndex - 1) * 45f + randomSpread;
        }
        else if (bulletCount == 5)
        {
            // Fan de 180°: -90°, -45°, 0°, 45°, 90°
            return (bulletIndex - 2) * 45f + randomSpread;
        }
        else if (bulletCount >= 8)
        {
            // Círculo 360°
            return (360f / bulletCount) * bulletIndex + randomSpread;
        }
        
        // Fallback: distribuir uniformemente
        float totalAngle = multiShotAngle * (bulletCount - 1);
        float startAngle = -totalAngle / 2f;
        return startAngle + (multiShotAngle * bulletIndex) + randomSpread;
    }

    void ApplyCriticalToBullet(GameObject bullet)
    {
        Bala bala = bullet.GetComponent<Bala>();
        if (bala != null)
        {
            bala.damage = Mathf.RoundToInt(bala.damage * criticalDamageMultiplier);
            bala.isCritical = true;
            
            // Efecto visual de crítico
            MakeBulletCritical(bullet);
        }
    }

    void MakeBulletCritical(GameObject bullet)
    {
        // Hacer la bala dorada y más grande
        SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.yellow;
            bullet.transform.localScale *= 1.5f;
        }
        
        // TODO: Agregar trail effect
        // TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        // if (trail != null) trail.startColor = Color.yellow;
    }

    void FixedUpdate()
    {
        // Movimiento físico
        rb.linearVelocity = moveInput * moveSpeed;
    }

    #region Public Setters (llamados por UpgradeManager)

    public void SetBulletCount(int count)
    {
        bulletCount = Mathf.Max(1, count);
    }

    public void SetCriticalChance(float chance)
    {
        criticalChance = Mathf.Clamp01(chance);
    }

    #endregion
}
