using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PaletteSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI paletteNameText;
    public Image previewBackground;
    public Image previewPlayer;
    public Image previewEnemy;
    
    [Header("Buttons")]
    public Button prevButton;
    public Button nextButton;
    public Button selectButton;
    public Button backButton;
    
    [Header("Button Text")]
    public TextMeshProUGUI selectButtonText;
    public TextMeshProUGUI statusText;
    
    private int currentIndex = 0;
    
    void Start()
    {
        // Cargar paleta actual
        if (SaveManager.Instance != null)
        {
            currentIndex = SaveManager.Instance.GetCurrentPalette();
        }
        
        // Conectar botones
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousPalette);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPalette);
        
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectPalette);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        // Aplicar paleta actual
        if (PaletteManager.Instance != null)
            PaletteManager.Instance.ApplyCurrentPalette();
        
        UpdateUI();
    }
    
    void PreviousPalette()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = PaletteManager.Instance.allPalettes.Length - 1;
        
        UpdateUI();
    }
    
    void NextPalette()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentIndex++;
        if (currentIndex >= PaletteManager.Instance.allPalettes.Length)
            currentIndex = 0;
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (PaletteManager.Instance == null) return;
        
        PaletteData palette = PaletteManager.Instance.allPalettes[currentIndex];
        if (palette == null) return;
        
        // Nombre de paleta
        if (paletteNameText != null)
            paletteNameText.text = palette.paletteName;
        
        // Preview visual
        if (previewBackground != null)
            previewBackground.color = palette.backgroundColor;
        
        if (previewPlayer != null)
            previewPlayer.color = palette.playerColor;
        
        if (previewEnemy != null)
            previewEnemy.color = palette.enemyScoutColor;
        
        // Estado del botón
        bool isUnlocked = PaletteManager.Instance.IsPaletteUnlocked(currentIndex);
        bool isCurrent = (SaveManager.Instance != null && 
                         currentIndex == SaveManager.Instance.GetCurrentPalette());
        
        if (selectButton != null)
        {
            if (isCurrent)
            {
                if (selectButtonText != null)
                    selectButtonText.text = "SELECTED";
                selectButton.interactable = false;
            }
            else if (isUnlocked)
            {
                if (selectButtonText != null)
                    selectButtonText.text = "SELECT";
                selectButton.interactable = true;
            }
            else
            {
                if (selectButtonText != null)
                    selectButtonText.text = $"WATCH {palette.adCostToUnlock} AD(S)";
                selectButton.interactable = true;
            }
        }
        
        // Status text
        if (statusText != null)
        {
            if (palette.isFree)
            {
                statusText.text = "FREE";
                statusText.color = Color.green;
            }
            else if (isUnlocked)
            {
                statusText.text = "UNLOCKED";
                statusText.color = Color.cyan;
            }
            else
            {
                statusText.text = "LOCKED";
                statusText.color = Color.red;
            }
        }
    }
    
    void OnSelectPalette()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        bool isUnlocked = PaletteManager.Instance.IsPaletteUnlocked(currentIndex);
        
        if (isUnlocked)
        {
            // Ya desbloqueada, solo seleccionar
            if (PaletteManager.Instance != null)
            {
                PaletteManager.Instance.SetPalette(currentIndex);
            }
            
            UpdateUI();
        }
        else
        {
            // TODO: Mostrar ad (FASE 5)
            // Por ahora, desbloquear directamente para testing
            Debug.Log("TODO: Show ad to unlock palette");
            
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.UnlockPalette(currentIndex);
                PaletteManager.Instance.SetPalette(currentIndex);
            }
            
            UpdateUI();
        }
    }
    
    void GoBack()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("MainMenu");
    }
}
// ```

// ### **PASO 4: Crear Canvas UI**

// 1. Hierarchy → UI → Canvas
// 2. Nombrar: `PaletteSelectorUI`
// 3. Configurar igual (720x1280)

// ### **PASO 5: Crear estructura UI**

// **Panel Principal:**
// ```
// Canvas
// └─ Panel (Nombrar: MainPanel)
//     ├─ Anchor: Center
//     ├─ Width: 600
//     ├─ Height: 900
//     ├─ Color: Negro (0, 0, 0, 200)
//     └─ Shadow: (5, -5) Negro
// ```

// **Título:**
// ```
// MainPanel
// └─ Text - TextMeshPro (Nombrar: TitleText)
//     ├─ Anchor: Top Center
//     ├─ Pos Y: -50
//     ├─ Text: "SELECT PALETTE"
//     ├─ Font Size: 48
//     ├─ Color: Blanco
//     └─ Shadow: (2, -2) Negro
// ```

// **Nombre de paleta:**
// ```
// MainPanel
// └─ Text - TextMeshPro (Nombrar: PaletteNameText)
//     ├─ Anchor: Top Center
//     ├─ Pos Y: -150
//     ├─ Text: "NEON"
//     ├─ Font Size: 36
//     ├─ Color: Cyan
//     └─ Shadow: (2, -2) Negro
// ```

// **Preview Box:**
// ```
// MainPanel
// └─ Panel (Nombrar: PreviewBox)
//     ├─ Anchor: Center
//     ├─ Pos Y: 50
//     ├─ Width: 500
//     ├─ Height: 300
//     └─ Color: Gris oscuro (50, 50, 50)
    
//     └─ Image (Nombrar: PreviewBackground)
//         ├─ Anchor: Stretch
//         ├─ Color: Negro (dinámico)
        
//         └─ Image (hijo, Nombrar: PreviewPlayer)
//             ├─ Pos: (0, 0)
//             ├─ Width/Height: 80
//             ├─ Sprite: [Un sprite de nave]
//             ├─ Color: Cyan (dinámico)
            
//         └─ Image (hijo, Nombrar: PreviewEnemy)
//             ├─ Pos: (150, 0)
//             ├─ Width/Height: 60
//             ├─ Sprite: [Un sprite de enemigo]
//             ├─ Color: Verde (dinámico)
// ```

// **Botones de navegación:**
// ```
// MainPanel
// └─ Button < (Nombrar: PrevButton)
//     ├─ Pos: (-200, 50)
//     ├─ Width/Height: 80
//     ├─ Text: "<"
//     └─ Font Size: 48

// └─ Button > (Nombrar: NextButton)
//     ├─ Pos: (200, 50)
//     ├─ Width/Height: 80
//     ├─ Text: ">"
//     └─ Font Size: 48
// ```

// **Status Text:**
// ```
// MainPanel
// └─ Text (Nombrar: StatusText)
//     ├─ Pos Y: -250
//     ├─ Text: "FREE"
//     ├─ Font Size: 28
//     ├─ Color: Verde
// ```

// **Botón Select:**
// ```
// MainPanel
// └─ Button (Nombrar: SelectButton)
//     ├─ Pos Y: -350
//     ├─ Width: 400
//     ├─ Height: 80
//     └─ Text: "SELECT"
// ```

// **Botón Back:**
// ```
// MainPanel
// └─ Button (Nombrar: BackButton)
//     ├─ Anchor: Bottom Center
//     ├─ Pos Y: 50
//     ├─ Width: 300
//     ├─ Height: 60
//     └─ Text: "BACK"
// ```

// ### **PASO 6: Conectar script**

// 1. Crea GameObject vacío: `PaletteSelectorController`
// 2. Add Component → PaletteSelectorUI
// 3. Asignar todas las referencias:
// ```
// UI References:
// ├─ Palette Name Text: [Arrastra PaletteNameText]
// ├─ Preview Background: [Arrastra PreviewBackground]
// ├─ Preview Player: [Arrastra PreviewPlayer]
// └─ Preview Enemy: [Arrastra PreviewEnemy]

// Buttons:
// ├─ Prev Button: [Arrastra PrevButton]
// ├─ Next Button: [Arrastra NextButton]
// ├─ Select Button: [Arrastra SelectButton]
// └─ Back Button: [Arrastra BackButton]

// Button Text:
// ├─ Select Button Text: [Texto del SelectButton]
// └─ Status Text: [Arrastra StatusText]