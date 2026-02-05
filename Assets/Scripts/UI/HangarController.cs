using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HangarController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI shipNameText;
    public TextMeshProUGUI scrapText;
    public Button launchButton;
    public Button backButton;
    
    void Start()
    {
        // Aplicar paleta
        if (PaletteManager.Instance != null)
            PaletteManager.Instance.ApplyCurrentPalette();
        
        // Mostrar scrap
        UpdateScrapDisplay();
        
        // Conectar botones
        if (launchButton != null)
            launchButton.onClick.AddListener(LaunchGame);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        // Mostrar nave equipada
        if (shipNameText != null && SaveManager.Instance != null)
        {
            string shipName = SaveManager.Instance.GetEquippedShip();
            shipNameText.text = shipName.ToUpper();
        }
    }
    
    void UpdateScrapDisplay()
    {
        if (scrapText != null && SaveManager.Instance != null)
        {
            int scrap = SaveManager.Instance.GetScrap();
            scrapText.text = $"SCRAP: {scrap}";
        }
    }
    
    void LaunchGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("Gameplay");
    }
    
    void GoBack()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("MainMenu");
    }
}
// ```

// ### **PASO 2: Crear UI del Hangar**

// Crea una UI simple:
// ```
// Canvas
// └─ Panel
//     ├─ TitleText: "HANGAR"
//     ├─ ShipNameText: "STARTER"
//     ├─ ScrapText: "SCRAP: 0"
//     ├─ LaunchButton: "LAUNCH" → Gameplay
//     └─ BackButton: "BACK" → MainMenu
// ```

// *(Los detalles completos los haremos en el siguiente mensaje para no saturar)*

// ---

// ## 🎯 BUILD SETTINGS

// **IMPORTANTE:** Agregar escenas al Build:

// 1. File → Build Settings
// 2. Click **Add Open Scenes** para cada una:
//    - MainMenu
//    - PaletteSelector  
//    - Hangar
//    - Gameplay

// **Orden:**
// ```
// 0: MainMenu
// 1: PaletteSelector
// 2: Hangar
// 3: Gameplay