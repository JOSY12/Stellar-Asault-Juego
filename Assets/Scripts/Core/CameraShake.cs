using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Esto es lo que falta para que el Instance funcione
    public static CameraShake Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Shake(float intensity, float time)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(intensity, time));
    }

    private System.Collections.IEnumerator DoShake(float intensity, float time)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < time)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}