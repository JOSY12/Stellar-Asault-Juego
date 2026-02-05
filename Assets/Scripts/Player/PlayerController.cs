using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ship Data")]
    public ShipData currentShip;
    
    [Header("Joysticks")]
    public VirtualJoystick joystickMove;
    public VirtualJoystick joystickAim;
    
    [Header("Fire Point")]
    public Transform firePoint;
    
    [Header("Prefabs")]
    public GameObject bulletPrefab;
    
    [Header("Camera Shake")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;
    [Header("UI References")]
public HealthUI healthUI; // ← NUEVO

    // Runtime stats
    private float currentHealth;
    private float nextFireTime;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        LoadShipData();
            ApplyCurrentPalette(); // ← AGREGAR ESTA LÍNEA
// ← NUEVO: Inicializar UI de salud
    if (healthUI != null)
    {
        healthUI.Initialize(Mathf.RoundToInt(currentShip.health.currentValue));
        healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
    }
    }
    
    void LoadShipData()
    {
        if (currentShip == null)
        {
            Debug.LogError("No ShipData assigned to PlayerController!");
            return;
        }
        
        // Cargar progreso guardado
        currentShip.LoadProgress();
        
        // Aplicar sprite
        if (spriteRenderer != null && currentShip.shipSprite != null)
            spriteRenderer.sprite = currentShip.shipSprite;
        
        // Inicializar salud
        currentHealth = currentShip.health.currentValue;
        
        Debug.Log($"Ship loaded: {currentShip.shipName}");
        Debug.Log($"Damage: {currentShip.damage.currentValue}");
        Debug.Log($"Fire Rate: {currentShip.fireRate.currentValue}");
        Debug.Log($"Move Speed: {currentShip.moveSpeed.currentValue}");
    }
    
   void Update()
{
    // ← CAMBIO: Verificar game over PRIMERO
    if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        return;
    
    if (GameManager.Instance != null && GameManager.Instance.isPaused)
        return;
    
    HandleAiming();
    HandleShooting();
}
    
   void FixedUpdate()
{
    // ← CAMBIO: Verificar game over PRIMERO
    if (GameManager.Instance != null && GameManager.Instance.isGameOver)
    {
        rb.linearVelocity = Vector2.zero; // Detener movimiento
        return;
    }
    
    if (GameManager.Instance != null && GameManager.Instance.isPaused)
    {
        rb.linearVelocity = Vector2.zero; // Detener movimiento
        return;
    }
    
    HandleMovement();
}
    
    void HandleMovement()
    {
        if (joystickMove == null) return;
        
        Vector2 moveInput = new Vector2(joystickMove.Horizontal, joystickMove.Vertical);
        rb.linearVelocity = moveInput * currentShip.moveSpeed.currentValue;
    }
    
    void HandleAiming()
    {
        if (joystickAim == null) return;
        
        Vector2 aimInput = new Vector2(joystickAim.Horizontal, joystickAim.Vertical);
        
        if (aimInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }
    
    void HandleShooting()
    {
        if (joystickAim == null) return;
        
        Vector2 aimInput = new Vector2(joystickAim.Horizontal, joystickAim.Vertical);
        
        if (aimInput.sqrMagnitude > 0.1f && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + currentShip.fireRate.currentValue;
        }
    }
    
   void Shoot()
{
    if (bulletPrefab == null || firePoint == null)
    {
        Debug.LogError("Missing bullet prefab or fire point!");
        return;
    }
    
    // Instanciar bala
    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    
    // Configurar bala con stats de la nave
    Bullet bulletScript = bullet.GetComponent<Bullet>();
    if (bulletScript != null)
    {
        bulletScript.Initialize(
            currentShip.bulletSpeed.currentValue,
            Mathf.RoundToInt(currentShip.damage.currentValue)
        );
        
        // ← NUEVO: Aplicar visual de la bala
        if (currentShip.bulletData != null)
        {
            bulletScript.ApplyBulletData(currentShip.bulletData);
        }
    }
    
    // Reproducir sonido
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlaySFX(AudioManager.Instance.shootSFX);
    
    // Camera shake
    if (CameraShake.Instance != null && SaveManager.Instance != null)
    {
        if (SaveManager.Instance.IsCameraShakeEnabled())
            CameraShake.Instance.Shake(shakeIntensity, shakeDuration);
    }
}
    
    public void TakeDamage(int amount)
{
    currentHealth -= amount;
    
    // ← NUEVO: Actualizar UI
    if (healthUI != null)
    {
        healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
    }
    
    // Reproducir sonido de daño
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHitSFX);
    
    // Camera shake
    if (CameraShake.Instance != null && SaveManager.Instance != null)
    {
        if (SaveManager.Instance.IsCameraShakeEnabled())
            CameraShake.Instance.Shake(shakeIntensity * 2f, shakeDuration * 2f);
    }
    
    if (currentHealth <= 0)
    {
        Die();
    }
}
    
    void Die()
    {
        // Notificar al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
        
        // Desactivar controles
        enabled = false;
    }
    void ApplyCurrentPalette()
{
    if (PaletteManager.Instance != null)
    {
        PaletteData palette = PaletteManager.Instance.GetCurrentPalette();
        if (palette != null && spriteRenderer != null)
        {
            spriteRenderer.color = palette.playerColor;
        }
    }
}
}