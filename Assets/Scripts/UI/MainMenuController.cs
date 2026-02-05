using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        // Aplicar paleta guardada
        if (PaletteManager.Instance != null)
        {
            PaletteManager.Instance.ApplyCurrentPalette();
        }
        
        // Reproducir música de menú
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
        }
    }
    
    public void OnStartButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("Hangar");
    }
    
    public void OnPalettesButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("PaletteSelector");
    }
    
    public void OnSettingsButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        // TODO: Abrir panel de settings (lo haremos después)
        Debug.Log("Settings clicked");
    }
    
    public void OnExitButton()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Application.Quit();
        Debug.Log("Exit Game");
    }
}
// ```

// ### **PASO 5: Crear Canvas UI**

// 1. Hierarchy → UI → Canvas
// 2. Nombrar: `MainMenuUI`

// **Configurar Canvas:**
// ```
// Canvas:
// └─ Render Mode: Screen Space - Overlay

// Canvas Scaler:
// ├─ UI Scale Mode: Scale With Screen Size
// ├─ Reference Resolution: 720 x 1280
// ├─ Screen Match Mode: Match Width Or Height
// └─ Match: 0.5
// ```

// ### **PASO 6: Crear Background (opcional)**

// 1. Click derecho en Canvas → UI → Image
// 2. Nombrar: `Background`
// ```
// Rect Transform:
// └─ Anchor Presets: Stretch (ambas direcciones)

// Image:
// ├─ Color: Negro con alpha 200 (0, 0, 0, 200)
// └─ Raycast Target: ✗ (desactivar)
// ```

// ### **PASO 7: Crear Logo del juego**

// 1. Click derecho en Canvas → UI → Image
// 2. Nombrar: `Logo`
// ```
// Rect Transform:
// ├─ Anchor: Top Center
// ├─ Pos X: 0
// ├─ Pos Y: -200
// ├─ Width: 600
// └─ Height: 200

// Image:
// ├─ Source Image: [Tu sprite de logo si tienes]
// ├─ Color: Blanco
// └─ Preserve Aspect: ✓
// ```

// **Si no tienes logo, crea un Text:**
// ```
// Text - TextMeshPro:
// ├─ Text: "ENDLESS PURSUIT"
// ├─ Font Size: 72
// ├─ Alignment: Center
// ├─ Color: Blanco
// └─ Shadow: (2, -2) Negro
// ```

// ### **PASO 8: Crear Panel de Botones**

// 1. Click derecho en Canvas → UI → Panel
// 2. Nombrar: `ButtonPanel`
// ```
// Rect Transform:
// ├─ Anchor: Center
// ├─ Pos X: 0
// ├─ Pos Y: -100
// ├─ Width: 400
// └─ Height: 500

// Image:
// ├─ Color: Transparente (0, 0, 0, 0)
// └─ Raycast Target: ✗
// ```

// ### **PASO 9: Crear Botones**

// **Botón START:**

// 1. Click derecho en `ButtonPanel` → UI → Button - TextMeshPro
// 2. Nombrar: `StartButton`
// ```
// Rect Transform:
// ├─ Anchor: Top Center
// ├─ Pos X: 0
// ├─ Pos Y: -50
// ├─ Width: 350
// └─ Height: 80

// Button:
// ├─ Normal Color: Blanco con alpha 150
// ├─ Highlighted Color: Cyan claro
// ├─ Pressed Color: Cyan oscuro
// └─ Transition: Color Tint

// Image (del botón):
// ├─ Color: El de arriba
// └─ Add Component → Shadow
//     ├─ Effect Distance: (4, -4)
//     └─ Effect Color: Negro

// Text (hijo):
// ├─ Text: "START"
// ├─ Font Size: 42
// ├─ Color: Blanco
// ├─ Alignment: Center
// └─ Shadow: (2, -2) Negro
// ```

// **Duplicar botones (Ctrl+D) 3 veces más y ajustar:**

// **Botón PALETTES:**
// ```
// Pos Y: -150
// Text: "PALETTES"
// ```

// **Botón SETTINGS:**
// ```
// Pos Y: -250
// Text: "SETTINGS"
// ```

// **Botón EXIT:**
// ```
// Pos Y: -350
// Text: "EXIT"
// Color Normal: Rojo claro (255, 150, 150, 150)
// ```

// ### **PASO 10: Conectar botones**

// 1. Crea un GameObject vacío en Hierarchy
// 2. Nombrar: `MenuController`
// 3. Add Component → MainMenuController

// Ahora conecta cada botón:

// **StartButton:**
// ```
// Button → On Click():
// ├─ Click en +
// ├─ Arrastra MenuController
// └─ Function: MainMenuController → OnStartButton
// ```

// **PalettesButton:**
// ```
// On Click():
// └─ MainMenuController → OnPalettesButton
// ```

// **SettingsButton:**
// ```
// On Click():
// └─ MainMenuController → OnSettingsButton
// ```

// **ExitButton:**
// ```
// On Click():
// └─ MainMenuController → OnExitButton