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

    [Header("Feedback de Cámara")]
    public float shakeIntensity = 0.12f;
    public float shakeDuration = 0.08f;

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
    // Verifica que el prefab exista antes de instanciar
    if (bulletPrefab == null) 
    {
        Debug.LogError("¡Falta el Bullet Prefab en el PlayerController!");
        return;
    }

    float randomSpread = Random.Range(-spread, spread);
    Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);

    Instantiate(bulletPrefab, firePoint.position, bulletRotation);
    
    if (CameraShake.Instance != null)
        CameraShake.Instance.Shake(shakeIntensity, shakeDuration);

    nextFireTime = Time.time + fireRate;
}
    void FixedUpdate()
    {
        // Movimiento físico
        rb.linearVelocity = moveInput * moveSpeed;
    }
}