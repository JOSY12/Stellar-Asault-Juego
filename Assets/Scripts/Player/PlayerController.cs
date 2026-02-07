using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ControlType { Joysticks, TouchZones }

    [Header("Control Settings")]
    public ControlType controlType = ControlType.Joysticks;

    [Header("Joystick Controls")]
    public VirtualJoystick joystickMov;
    public VirtualJoystick joystickVis;

    [Header("Touch Zone Controls")]
    public TouchZoneController touchZoneMove;
    public TouchZoneController touchZoneAim;

    [Header("Ship Data")]
    public ShipData currentShip;
    public ShipData[] allShips;

    [Header("Fire Point")]
    public Transform firePoint;
    
    [Header("Health")]
    private float maxHealth;
    private float currentHealth;

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    
    [Header("Camera Shake")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;
    
    [Header("UI References")]
    public HealthUI healthUI;

    private float nextFireTime;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }
    
    void Start()
    {
        LoadShipData();
        ApplyCurrentPalette();

        Debug.Log($"=== SHIP LOADED ===");
        Debug.Log($"Ship: {currentShip.shipName}");
        Debug.Log($"Fire Rate: {currentShip.fireRate.currentValue}");
        Debug.Log($"Fire Rate Level: {currentShip.fireRate.currentLevel}/{currentShip.fireRate.maxLevel}");

        if (healthUI != null)
        {
            healthUI.Initialize(Mathf.RoundToInt(currentShip.health.currentValue));
            healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
        }

        if (SaveManager.Instance != null)
        {
            bool useTouchZones = SaveManager.Instance.UseTouchZones();
            SetControlType(useTouchZones ? ControlType.TouchZones : ControlType.Joysticks);
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
        
        string equippedShipName = SaveManager.Instance.GetEquippedShip();
        Debug.Log($"Equipped ship from SaveManager: {equippedShipName}");
        
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
            
            currentShip = allShips[0];
            Debug.LogWarning($"Using fallback ship: {currentShip.shipName}");
        }
        
        currentShip.LoadProgress();
        ApplyShipData();
    }

    void ApplyShipData()
    {
        Debug.Log("=== APPLYING SHIP DATA ===");
        Debug.Log($"Current ship: {currentShip.shipName}");
        
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
        
        if (currentShip.shipSprite == null)
        {
            Debug.LogError($"Ship {currentShip.shipName} has NULL sprite!");
            return;
        }
        
        Sprite oldSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = currentShip.shipSprite;
        
        Debug.Log($"Sprite changed:");
        Debug.Log($"  Old: {(oldSprite != null ? oldSprite.name : "NULL")}");
        Debug.Log($"  New: {spriteRenderer.sprite.name}");
        Debug.Log($"  Ship sprite: {currentShip.shipSprite.name}");
        Debug.Log($"  Match: {spriteRenderer.sprite == currentShip.shipSprite}");
        
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
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;
        
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;
        
        if (currentHealth <= 0)
            return;
        
        HandleAiming();
        HandleShooting();
    }
    
    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        if (currentHealth <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        HandleMovement();
    }
    
    void HandleMovement()
    {
        Vector2 moveInput = GetMoveInput();
        rb.linearVelocity = moveInput * currentShip.moveSpeed.currentValue;
    }
    
    // ✅ CORREGIDO: Usa lookInput en lugar de aimInput
    void HandleAiming()
    {
        Vector2 lookInput = GetAimInput();
        
        if (lookInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
            
            // Debug visual (solo en Scene view)
            Debug.DrawRay(transform.position, new Vector3(lookInput.x, lookInput.y, 0) * 3f, Color.red);
        }
    }

    void HandleShooting()
    {
        Vector2 lookInput = GetAimInput();
        
        if (lookInput.sqrMagnitude > 0.1f)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + currentShip.fireRate.currentValue;
            }
        }
    }
    
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("Missing bullet prefab or fire point!");
            return;
        }
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(
                currentShip.bulletSpeed.currentValue,
                Mathf.RoundToInt(currentShip.damage.currentValue)
            );
            
            if (currentShip.bulletData != null)
            {
                bulletScript.ApplyBulletData(currentShip.bulletData);
            }
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.shootSFX);
        
        if (CameraShake.Instance != null && SaveManager.Instance != null)
        {
            if (SaveManager.Instance.IsCameraShakeEnabled())
                CameraShake.Instance.Shake(shakeIntensity, shakeDuration);
        }
    }

    public void SetControlType(ControlType newType)
    {
        controlType = newType;
        
        Debug.Log($"=== CHANGING CONTROL TYPE TO: {newType} ===");
        
        if (joystickMov != null)
        {
            joystickMov.gameObject.SetActive(newType == ControlType.Joysticks);
            Debug.Log($"Joystick Move active: {newType == ControlType.Joysticks}");
        }
        
        if (joystickVis != null)
        {
            joystickVis.gameObject.SetActive(newType == ControlType.Joysticks);
            Debug.Log($"Joystick Aim active: {newType == ControlType.Joysticks}");
        }
        
        if (touchZoneMove != null)
        {
            touchZoneMove.gameObject.SetActive(newType == ControlType.TouchZones);
            Debug.Log($"TouchZone Move active: {newType == ControlType.TouchZones}");
        }
        
        if (touchZoneAim != null)
        {
            touchZoneAim.gameObject.SetActive(newType == ControlType.TouchZones);
            Debug.Log($"TouchZone Aim active: {newType == ControlType.TouchZones}");
        }
        
        Debug.Log($"=== CONTROL TYPE CHANGED ===");
    }

    public void StopInput()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        if (joystickMov != null)
            joystickMov.ResetJoystick();
        
        if (joystickVis != null)
            joystickVis.ResetJoystick();
        
        if (touchZoneMove != null)
            touchZoneMove.ResetZone();
        
        if (touchZoneAim != null)
            touchZoneAim.ResetZone();
        
        Debug.Log("Player input stopped");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        if (healthUI != null)
        {
            healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerHitSFX);
        
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        StopInput();
        
        Debug.Log("Player died!");
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        
        if (healthUI != null)
        {
            healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
        }
        
        StopInput();
        
        Debug.Log("Player revived!");
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

    // ✅ INPUT METHODS
    Vector2 GetMoveInput()
    {
        if (controlType == ControlType.Joysticks && joystickMov != null)
        {
            return new Vector2(joystickMov.Horizontal, joystickMov.Vertical);
        }
        else if (controlType == ControlType.TouchZones && touchZoneMove != null)
        {
            return new Vector2(touchZoneMove.Horizontal, touchZoneMove.Vertical);
        }
        
        return Vector2.zero;
    }

    Vector2 GetAimInput()
    {
        if (controlType == ControlType.Joysticks && joystickVis != null)
        {
            return new Vector2(joystickVis.Horizontal, joystickVis.Vertical);
        }
        else if (controlType == ControlType.TouchZones && touchZoneAim != null)
        {
            // ✅ Usar el InputVector directamente (funciona igual que Movement)
            return new Vector2(touchZoneAim.Horizontal, touchZoneAim.Vertical);
        }
        
        return Vector2.zero;
    }
}

 