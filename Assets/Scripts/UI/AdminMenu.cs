using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdminMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject adminPanel;
    public Button openButton; // Botón invisible en esquina
    
    [Header("Admin Buttons")]
    public Button add1000ScrapButton;
    public Button add10000ScrapButton;
    public Button unlockAllShipsButton;
    public Button maxAllStatsButton;
    public Button resetAllDataButton;
    public Button closeButton;
    
    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    
    private int clickCount = 0;
    private float lastClickTime = 0f;
    
    void Start()
    {
        if (adminPanel != null)
            adminPanel.SetActive(false);
        
        // Conectar botones
        if (openButton != null)
            openButton.onClick.AddListener(OnOpenButtonClicked);
        
        if (add1000ScrapButton != null)
            add1000ScrapButton.onClick.AddListener(() => AddScrap(1000));
        
        if (add10000ScrapButton != null)
            add10000ScrapButton.onClick.AddListener(() => AddScrap(10000));
        
        if (unlockAllShipsButton != null)
            unlockAllShipsButton.onClick.AddListener(UnlockAllShips);
        
       
        
        if (maxAllStatsButton != null)
            maxAllStatsButton.onClick.AddListener(MaxAllStats);
        
        if (resetAllDataButton != null)
            resetAllDataButton.onClick.AddListener(ResetAllData);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseAdminPanel);
    }
    
    void OnOpenButtonClicked()
    {
        // Requiere 5 clicks rápidos (en 2 segundos)
        if (Time.time - lastClickTime > 2f)
        {
            clickCount = 0;
        }
        
        clickCount++;
        lastClickTime = Time.time;
        
        if (clickCount >= 5)
        {
            OpenAdminPanel();
            clickCount = 0;
        }
    }
    
    void OpenAdminPanel()
    {
        if (adminPanel != null)
        {
            adminPanel.SetActive(true);
            ShowFeedback("Admin Menu Opened", Color.cyan);
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void CloseAdminPanel()
    {
        if (adminPanel != null)
            adminPanel.SetActive(false);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void AddScrap(int amount)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.AddScrap(amount);
            ShowFeedback($"+{amount} Scrap Added!", Color.green);
            Debug.Log($"[ADMIN] Added {amount} scrap");
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void UnlockAllShips()
    {
        if (SaveManager.Instance != null)
        {
            // Lista de todas las naves
            string[] shipNames = { "Starter", "Scout", "Tank", "Burst", "Omega" };
            
            foreach (string shipName in shipNames)
            {
                SaveManager.Instance.UnlockShip(shipName);
            }
            
            ShowFeedback("All Ships Unlocked!", Color.green);
            Debug.Log("[ADMIN] All ships unlocked");
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
     
    
    void MaxAllStats()
    {
        if (SaveManager.Instance != null)
        {
            string[] shipNames = { "Starter", "Scout", "Tank", "Burst", "Omega" };
            string[] statNames = { "Damage", "Fire Rate", "Move Speed", "Health", "Bullet Speed" };
            
            foreach (string shipName in shipNames)
            {
                foreach (string statName in statNames)
                {
                    // Subir a nivel 5 (máximo)
                    for (int i = 0; i < 5; i++)
                    {
                        SaveManager.Instance.UpgradeShipStat(shipName, statName);
                    }
                }
            }
            
            ShowFeedback("All Stats Maxed!", Color.green);
            Debug.Log("[ADMIN] All stats maxed");
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void ResetAllData()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetAllData();
            ShowFeedback("All Data Reset!", Color.red);
            Debug.Log("[ADMIN] All data reset");
        }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }
    
    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            CancelInvoke("ClearFeedback");
            Invoke("ClearFeedback", 2f);
        }
    }
    
    void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }
}