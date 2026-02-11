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
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        LoadShipData();

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
        if (SaveManager.Instance == null || allShips == null || allShips.Length == 0)
        {
            Debug.LogError("Cannot load ship data!");
            return;
        }
        
        string equippedShipName = SaveManager.Instance.GetEquippedShip();
        
        currentShip = null;
        foreach (ShipData ship in allShips)
        {
            if (ship != null && ship.shipName == equippedShipName)
            {
                currentShip = ship;
                break;
            }
        }
        
        if (currentShip == null)
        {
            currentShip = allShips[0];
            Debug.LogWarning($"Using fallback ship: {currentShip.shipName}");
        }
        
        currentShip.LoadProgress();
        ApplyShipData();
    }

    void ApplyShipData()
    {
        if (spriteRenderer != null && currentShip.shipSprite != null)
        {
            spriteRenderer.sprite = currentShip.shipSprite;
        }
        
        maxHealth = currentShip.health.currentValue;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.isGameOver || GameManager.Instance.isPaused))
            return;
        
        if (currentHealth <= 0)
            return;
        
        HandleAiming();
        HandleShooting();
    }
    
    void FixedUpdate()
    {
        if (GameManager.Instance != null && (GameManager.Instance.isGameOver || GameManager.Instance.isPaused))
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
    
    void HandleAiming()
    {
        Vector2 lookInput = GetAimInput();
        
        if (lookInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
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
        if (bulletPrefab == null || firePoint == null) return;
        
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
        
        if (joystickMov != null)
            joystickMov.gameObject.SetActive(newType == ControlType.Joysticks);
        
        if (joystickVis != null)
            joystickVis.gameObject.SetActive(newType == ControlType.Joysticks);
        
        if (touchZoneMove != null)
            touchZoneMove.gameObject.SetActive(newType == ControlType.TouchZones);
        
        if (touchZoneAim != null)
            touchZoneAim.gameObject.SetActive(newType == ControlType.TouchZones);
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
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        
        if (healthUI != null)
        {
            healthUI.UpdateHealth(Mathf.RoundToInt(currentHealth));
        }
        
        StopInput();
        
        // REACTIVAR controles
        if (SaveManager.Instance != null)
        {
            bool useTouchZones = SaveManager.Instance.UseTouchZones();
            SetControlType(useTouchZones ? ControlType.TouchZones : ControlType.Joysticks);
        }
    }

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
            return new Vector2(touchZoneAim.Horizontal, touchZoneAim.Vertical);
        }
        
        return Vector2.zero;
    }
}