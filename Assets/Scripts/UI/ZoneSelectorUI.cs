using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ZoneSelectorUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI zoneNameText;
    public TextMeshProUGUI scrapMultiplierText;
    public Image previewBackground;
    
    [Header("Buttons")]
    public Button prevButton;
    public Button nextButton;
    public Button launchButton;
    public Button backButton;
    
    [Header("Button Text")]
    public TextMeshProUGUI launchButtonText;
    public TextMeshProUGUI statusText;
    
    private int currentIndex = 0;
    
    void Start()
    {
        // Siempre empieza en zona 0
        currentIndex = 0;
        
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousZone);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextZone);
        
        if (launchButton != null)
            launchButton.onClick.AddListener(OnLaunch);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        UpdateUI();
    }
    
    void OnEnable()
    {
        // Refrescar UI al volver a esta escena
        if (ZoneManager.Instance != null)
        {
            UpdateUI();
        }
    }
    
    void PreviousZone()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (ZoneManager.Instance == null || ZoneManager.Instance.allZones == null)
        {
            Debug.LogError("ZoneManager or zones not found!");
            return;
        }
        
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = ZoneManager.Instance.allZones.Length - 1;
        
        Debug.Log($"Previous zone: {currentIndex}");
        UpdateUI();
    }
    
    void NextZone()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (ZoneManager.Instance == null || ZoneManager.Instance.allZones == null)
        {
            Debug.LogError("ZoneManager or zones not found!");
            return;
        }
        
        currentIndex++;
        if (currentIndex >= ZoneManager.Instance.allZones.Length)
            currentIndex = 0;
        
        Debug.Log($"Next zone: {currentIndex}");
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (ZoneManager.Instance == null)
        {
            Debug.LogError("ZoneManager not found!");
            return;
        }
        
        if (ZoneManager.Instance.allZones == null || ZoneManager.Instance.allZones.Length == 0)
        {
            Debug.LogError("No zones assigned to ZoneManager!");
            return;
        }
        
        // Validar índice
        if (currentIndex < 0 || currentIndex >= ZoneManager.Instance.allZones.Length)
        {
            Debug.LogWarning($"Invalid zone index {currentIndex}, resetting to 0");
            currentIndex = 0;
        }
        
        ZoneData zone = ZoneManager.Instance.allZones[currentIndex];
        if (zone == null)
        {
            Debug.LogError($"Zone at index {currentIndex} is null!");
            return;
        }
        
        if (zoneNameText != null)
            zoneNameText.text = zone.zoneName;
        
        if (scrapMultiplierText != null)
            scrapMultiplierText.text = $"SCRAP: x{zone.scrapMultiplier}";
        
        if (previewBackground != null)
            previewBackground.color = zone.ambientColor;
        
        bool isUnlocked = zone.IsUnlocked();
        
        if (launchButton != null)
        {
            if (isUnlocked)
            {
                launchButton.interactable = true;
                if (launchButtonText != null)
                    launchButtonText.text = "LAUNCH";
            }
            else
            {
                int currentScrap = SaveManager.Instance != null ? SaveManager.Instance.GetScrap() : 0;
                bool canAfford = currentScrap >= zone.unlockCost;
                
                launchButton.interactable = canAfford;
                if (launchButtonText != null)
                    launchButtonText.text = canAfford ? $"UNLOCK ({zone.unlockCost})" : $"LOCKED ({zone.unlockCost})";
            }
        }
        
        if (statusText != null)
        {
            if (zone.unlockCost == 0 || isUnlocked)
            {
                statusText.text = "UNLOCKED";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "LOCKED";
                statusText.color = Color.red;
            }
        }
        
        Debug.Log($"UI Updated: Zone {currentIndex} - {zone.zoneName}");
    }
    
    void OnLaunch()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (ZoneManager.Instance == null || ZoneManager.Instance.allZones == null)
        {
            Debug.LogError("Cannot launch - ZoneManager not ready!");
            return;
        }
        
        ZoneData zone = ZoneManager.Instance.allZones[currentIndex];
        bool isUnlocked = zone.IsUnlocked();
        
        if (!isUnlocked)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.SpendScrap(zone.unlockCost))
            {
                zone.Unlock();
                Debug.Log($"Zone {zone.zoneName} unlocked!");
            }
            else
            {
                Debug.Log("Not enough scrap!");
                return;
            }
        }
        
        // Guardar zona seleccionada
        ZoneManager.Instance.SetCurrentZone(currentIndex);
        Debug.Log($"Launching zone: {zone.zoneName}");
        
        // Ir a Gameplay
        SceneManager.LoadScene("Gameplay");
    }
    
    void GoBack()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("Hangar");
    }
}