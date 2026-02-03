using UnityEngine;

public class ShadowController : MonoBehaviour
{[Range(0, 1)] public float shadowOpacity = 0.5f;
    public Vector2 lightDirection = new Vector2(1, -1); 
    public float shadowOffset = 0.3f;
    public SpriteRenderer shipRenderer;
    
    private Transform shadowTransform;
    private SpriteRenderer shadowRenderer;

    void Start()
    {
        // Crear el objeto sombra como hijo
        GameObject shadowObj = new GameObject("Shadow_Object");
        shadowTransform = shadowObj.transform;
        shadowRenderer = shadowObj.AddComponent<SpriteRenderer>();

      shadowRenderer.color = new Color(0, 0, 0, shadowOpacity);
        shadowRenderer.sortingOrder = shipRenderer.sortingOrder - 1;
    }

    void LateUpdate()
    {
        // Sincronizar Sprite y Rotación
        shadowRenderer.sprite = shipRenderer.sprite;
        shadowTransform.rotation = shipRenderer.transform.rotation;

        // Calcular desplazamiento relativo a la dirección de luz
        Vector3 offset = (Vector3)lightDirection.normalized * shadowOffset;
        
        // Posicionar la sombra respecto a la nave (sin mover la nave)
        shadowTransform.position = shipRenderer.transform.position + offset;
    }
}