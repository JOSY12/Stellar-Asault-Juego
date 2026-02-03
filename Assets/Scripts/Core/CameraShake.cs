using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    
    private Vector3 originalPosition;
    private bool isShaking = false;

    void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }

    public void Shake(float intensity, float time)
    {
        if (!isShaking)
        {
            StopAllCoroutines();
            StartCoroutine(DoShake(intensity, time));
        }
    }

    private System.Collections.IEnumerator DoShake(float intensity, float time)
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < time)
        {
            // Guardar la posición "objetivo" que CameraFollow está tratando de alcanzar
            CameraFollow cameraFollow = GetComponent<CameraFollow>();
            if (cameraFollow != null && cameraFollow.player != null)
            {
                originalPosition = cameraFollow.player.position + cameraFollow.offset;
            }
            else
            {
                originalPosition = transform.position;
            }

            // Aplicar shake como OFFSET de la posición objetivo
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.position = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isShaking = false;
        // No restauramos posición - CameraFollow se encargará
    }
}