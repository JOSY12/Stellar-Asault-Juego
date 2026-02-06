using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ship Data")]
    public ShipData currentShip;
    
    [Header("Joysticks")]
    public VirtualJoystick joystickMove;
    public VirtualJoystick joystickAim;
    [Header("Ship Data")]
public ShipData[] allShips; // ← Debe estar asignado en Inspector

    [Header("Fire Point")]
    public Transform firePoint;
    [Header("Health")]
private float maxHealth; // ← AGREGAR ESTA LÍNEA
    private float currentHealth;

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    [Header("Camera Shake")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;
    [Header("UI References")]
public HealthUI healthUI; // ← NUEVO

    // Runtime stats
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
    ApplyCurrentPalette();
// ← NUEVO: Inicializar UI de salud
    if (healthUI != null)
    {
        healthUI.Initialize(Mathf.RoundToInt(currentShip.health.currentValue));
        healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
    }
    }
    
  void LoadShipData()
{
    Debug.Log("=== LOADING SHIP DATA ===");
    
    if (SaveManager.Instance == null)
    {
        Debug.LogError("SaveManager is NULL!");
        return;
    }
    
    if (allShips == null || allShips.Length == 0)
    {
        Debug.LogError("allShips array is empty or null!");
        return;
    }
    
    // Obtener nave equipada
    string equippedShipName = SaveManager.Instance.GetEquippedShip();
    Debug.Log($"Equipped ship from SaveManager: {equippedShipName}");
    
    // Buscar la ShipData
    currentShip = null;
    foreach (ShipData ship in allShips)
    {
        if (ship != null && ship.shipName == equippedShipName)
        {
            currentShip = ship;
            Debug.Log($"✓ Found ship: {ship.shipName}");
            break;
        }
    }
    
    if (currentShip == null)
    {
        Debug.LogError($"Ship '{equippedShipName}' not found in allShips array!");
        Debug.Log("Available ships:");
        foreach (ShipData ship in allShips)
        {
            if (ship != null)
                Debug.Log($"  - {ship.shipName}");
        }
        
        // Usar primera nave como fallback
        currentShip = allShips[0];
        Debug.LogWarning($"Using fallback ship: {currentShip.shipName}");
    }
    
    // Cargar progreso de la nave
    currentShip.LoadProgress();
    
    // Aplicar datos de la nave
    ApplyShipData();
}
    
void ApplyShipData()
{
    Debug.Log("=== APPLYING SHIP DATA ===");
    Debug.Log($"Current ship: {currentShip.shipName}");
    
    // Verificar SpriteRenderer
    if (spriteRenderer == null)
    {
        Debug.LogError("SpriteRenderer is NULL!");
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Could not find SpriteRenderer component!");
            return;
        }
    }
    
    // Verificar sprite de la nave
    if (currentShip.shipSprite == null)
    {
        Debug.LogError($"Ship {currentShip.shipName} has NULL sprite!");
        return;
    }
    
    // APLICAR SPRITE
    Sprite oldSprite = spriteRenderer.sprite;
    spriteRenderer.sprite = currentShip.shipSprite;
    
    Debug.Log($"Sprite changed:");
    Debug.Log($"  Old: {(oldSprite != null ? oldSprite.name : "NULL")}");
    Debug.Log($"  New: {spriteRenderer.sprite.name}");
    Debug.Log($"  Ship sprite: {currentShip.shipSprite.name}");
    Debug.Log($"  Match: {spriteRenderer.sprite == currentShip.shipSprite}");
    
    // Aplicar stats
    maxHealth = currentShip.health.currentValue;
    currentHealth = maxHealth;
    
    Debug.Log($"Stats applied:");
    Debug.Log($"  Health: {currentHealth}");
    Debug.Log($"  Damage: {currentShip.damage.currentValue}");
    Debug.Log($"  Fire Rate: {currentShip.fireRate.currentValue}");
    Debug.Log($"  Move Speed: {currentShip.moveSpeed.currentValue}");
    
    Debug.Log("=== SHIP DATA APPLIED SUCCESSFULLY ===");
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