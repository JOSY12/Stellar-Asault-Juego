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
    public Button selectButton;
    public Button backButton;
    
    [Header("Button Text")]
    public TextMeshProUGUI selectButtonText;
    public TextMeshProUGUI statusText;
    
    private int currentIndex = 0;
    
    void Start()
    {
        if (SaveManager.Instance != null && ZoneManager.Instance != null)
        {
            ZoneData currentZone = ZoneManager.Instance.GetCurrentZone();
            if (currentZone != null)
            {
                currentIndex = currentZone.zoneIndex;
            }
        }
        
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousZone);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextZone);
        
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectZone);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        UpdateUI();
    }
    
    void PreviousZone()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = ZoneManager.Instance.allZones.Length - 1;
        
        UpdateUI();
    }
    
    void NextZone()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentIndex++;
        if (currentIndex >= ZoneManager.Instance.allZones.Length)
            currentIndex = 0;
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (ZoneManager.Instance == null || ZoneManager.Instance.allZones == null) return;
        
        ZoneData zone = ZoneManager.Instance.allZones[currentIndex];
        if (zone == null) return;
        
        if (zoneNameText != null)
            zoneNameText.text = zone.zoneName;
        
        if (scrapMultiplierText != null)
            scrapMultiplierText.text = $"SCRAP: x{zone.scrapMultiplier}";
        
        if (previewBackground != null)
            previewBackground.color = zone.ambientColor;
        
        bool isUnlocked = zone.IsUnlocked();
        bool isCurrent = (SaveManager.Instance != null && 
                         currentIndex == SaveManager.Instance.GetCurrentZone());
        
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
                    selectButtonText.text = $"LOCKED ({zone.unlockCost} SCRAP)";
                
                int currentScrap = SaveManager.Instance != null ? SaveManager.Instance.GetScrap() : 0;
                selectButton.interactable = currentScrap >= zone.unlockCost;
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
    }
    
    void OnSelectZone()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        ZoneData zone = ZoneManager.Instance.allZones[currentIndex];
        bool isUnlocked = zone.IsUnlocked();
        
        if (isUnlocked)
        {
            ZoneManager.Instance.SetCurrentZone(currentIndex);
            LaunchGame();
        }
        else
        {
            if (SaveManager.Instance != null && SaveManager.Instance.SpendScrap(zone.unlockCost))
            {
                zone.Unlock();
                ZoneManager.Instance.SetCurrentZone(currentIndex);
                LaunchGame();
            }
            else
            {
                Debug.Log("Not enough scrap!");
            }
        }
    }
    
    void LaunchGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
    
    void GoBack()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        SceneManager.LoadScene("Hangar");
    }
}