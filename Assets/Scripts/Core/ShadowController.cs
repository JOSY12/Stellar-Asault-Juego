using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [Range(0, 1)] public float shadowOpacity = 0.5f;
    public Vector2 lightDirection = new Vector2(1, -1); 
    public float shadowOffset = 0.3f;
    public SpriteRenderer shipRenderer;
    
    private Transform shadowTransform;
    private SpriteRenderer shadowRenderer;

    void Start()
    {
        // Crear el objeto sombra como hijo del objeto principal
        GameObject shadowObj = new GameObject("Shadow_Object");
        shadowTransform = shadowObj.transform;
        
        // ⚠️ CRÍTICO: Hacer que sea HIJO de este GameObject
        shadowTransform.SetParent(transform);
        
        shadowRenderer = shadowObj.AddComponent<SpriteRenderer>();
        shadowRenderer.color = new Color(0, 0, 0, shadowOpacity);
        shadowRenderer.sortingOrder = shipRenderer.sortingOrder - 1;
    }

    void LateUpdate()
    {
        if (shadowRenderer == null) return;
        
        // Sincronizar Sprite y Rotación
        shadowRenderer.sprite = shipRenderer.sprite;
        shadowTransform.rotation = shipRenderer.transform.rotation;

        // Calcular desplazamiento relativo a la dirección de luz
        Vector3 offset = (Vector3)lightDirection.normalized * shadowOffset;
        
        // Posicionar la sombra respecto a la nave (sin mover la nave)
        shadowTransform.position = shipRenderer.transform.position + offset;
    }
}



// ```

// **Cambios clave:**
// ✅ `shadowTransform.SetParent(transform)` - Ahora es hijo  
// ✅ Cuando destruyes el enemigo/player/bala → la sombra se destruye automáticamente  
// ✅ Verificación `if (shadowRenderer == null)` por seguridad

// ---

// ## ✅ APLICAR LOS CAMBIOS

// ### **PASO 1: Reemplazar scripts**

// 1. Abre `CameraShake.cs` y reemplaza TODO el código
// 2. Abre `ShadowController.cs` y reemplaza TODO el código
// 3. Guarda (Ctrl+S)
// 4. Vuelve a Unity (esperará a que compile)

// ### **PASO 2: Verificar en Unity**

// **NO necesitas cambiar nada en el Inspector**, los scripts son compatibles con la configuración actual.

// ### **PASO 3: Probar**

// Dale Play y verifica:

// ✅ **Camera Shake:**
// - Mueve el jugador lejos del centro
// - Dispara varias veces
// - La cámara NO debería saltar al (0,0,-10)
// - Solo debería temblar suavemente

// ✅ **Sombras:**
// - Mata enemigos
// - Las sombras deben desaparecer junto con los enemigos
// - NO deben quedar flotando

// ---

// ## 🔍 SI AÚN HAY PROBLEMAS

// ### **Debug para Camera:**

// Si la cámara TODAVÍA salta, verifica:

// 1. Selecciona `Main Camera` en Hierarchy
// 2. En el Inspector durante Play mode, observa:
// ```
//    Transform → Position
   
//    ¿Cambia bruscamente entre valores?
//    Ejemplo: (5, 3, -10) → (0, 0, -10) → (5, 3, -10)