using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HangarUI : MonoBehaviour
{
    [Header("Ship Data")]
    public ShipData[] allShips;
    private int currentShipIndex = 0;
    
    [Header("UI - Top Bar")]
    public TextMeshProUGUI scrapText;
    public Button backButton;
    
    [Header("UI - Ship Selection")]
    public TextMeshProUGUI shipNameText;
    public Image shipPreviewImage;
    public Button prevShipButton;
    public Button nextShipButton;
    public TextMeshProUGUI shipStatusText;
    
    [Header("UI - Stats Container")]
    public Transform statsContainer;
    public GameObject statRowPrefab;
    
    [Header("UI - Action Buttons")]
    public Button purchaseButton;
    public Button equipButton;
    public Button launchButton;
    public TextMeshProUGUI purchaseButtonText;
    
    void Start()
    {
        // Aplicar paleta
        if (PaletteManager.Instance != null)
            PaletteManager.Instance.ApplyCurrentPalette();
        
        // Cargar nave equipada
        LoadEquippedShip();
        
        // Conectar botones
        if (prevShipButton != null)
            prevShipButton.onClick.AddListener(PreviousShip);
        
        if (nextShipButton != null)
            nextShipButton.onClick.AddListener(NextShip);
        
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(PurchaseShip);
        
        if (equipButton != null)
            equipButton.onClick.AddListener(EquipShip);
        
        if (launchButton != null)
            launchButton.onClick.AddListener(LaunchGame);
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
        
        // Mostrar nave actual
        UpdateUI();
    }
    
    void LoadEquippedShip()
    {
        if (SaveManager.Instance == null) return;
        
        string equippedShipName = SaveManager.Instance.GetEquippedShip();
        
        for (int i = 0; i < allShips.Length; i++)
        {
            if (allShips[i].shipName == equippedShipName)
            {
                currentShipIndex = i;
                break;
            }
        }
    }
    
    void PreviousShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentShipIndex--;
        if (currentShipIndex < 0)
            currentShipIndex = allShips.Length - 1;
        
        UpdateUI();
    }
    
    void NextShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentShipIndex++;
        if (currentShipIndex >= allShips.Length)
            currentShipIndex = 0;
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        ShipData ship = allShips[currentShipIndex];
        ship.LoadProgress();
        
        // Actualizar scrap
        if (scrapText != null && SaveManager.Instance != null)
        {
            int scrap = SaveManager.Instance.GetScrap();
            scrapText.text = $"SCRAP: {scrap}";
        }
        
        // Nombre de nave
        if (shipNameText != null)
            shipNameText.text = ship.shipName.ToUpper();
        
        // Preview sprite
        if (shipPreviewImage != null && ship.shipSprite != null)
            shipPreviewImage.sprite = ship.shipSprite;
        
        // Status de la nave
        UpdateShipStatus(ship);
        
        // Stats de la nave
        UpdateStats(ship);
        
        // Botones de acción
        UpdateActionButtons(ship);
    }
    
    void UpdateShipStatus(ShipData ship)
    {
        bool isOwned = ship.IsOwned();
        bool isEquipped = ship.IsEquipped();
        
        if (shipStatusText != null)
        {
            if (isEquipped)
            {
                shipStatusText.text = "EQUIPPED";
                shipStatusText.color = Color.green;
            }
            else if (isOwned)
            {
                shipStatusText.text = "OWNED";
                shipStatusText.color = Color.cyan;
            }
            else
            {
                shipStatusText.text = $"LOCKED - {ship.purchaseCost} SCRAP";
                shipStatusText.color = Color.red;
            }
        }
    }
    
    void UpdateStats(ShipData ship)
    {
        if (statsContainer == null) return;
        
        // Limpiar stats anteriores
        foreach (Transform child in statsContainer)
            Destroy(child.gameObject);
        
        // Crear row por cada stat
        CreateStatRow(ship.damage, ship);
        CreateStatRow(ship.fireRate, ship);
        CreateStatRow(ship.moveSpeed, ship);
        CreateStatRow(ship.health, ship);
        CreateStatRow(ship.bulletSpeed, ship);
    }
    
    void CreateStatRow(ShipStat stat, ShipData ship)
    {
        if (statRowPrefab == null)
        {
            Debug.LogError("StatRow Prefab is NULL!");
            return;
        }
        
        GameObject row = Instantiate(statRowPrefab, statsContainer);
        
        // Buscar componentes
        Transform nameTransform = row.transform.Find("StatName");
        if (nameTransform == null)
        {
            Debug.LogError("StatRow missing 'StatName'");
            Destroy(row);
            return;
        }
        TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
        
        Transform valueTransform = row.transform.Find("ValueText");
        if (valueTransform == null)
        {
            Debug.LogError("StatRow missing 'ValueText'");
            Destroy(row);
            return;
        }
        TextMeshProUGUI valueText = valueTransform.GetComponent<TextMeshProUGUI>();
        
        Transform buttonTransform = row.transform.Find("UpgradeButton");
        if (buttonTransform == null)
        {
            Debug.LogError("StatRow missing 'UpgradeButton'");
            Destroy(row);
            return;
        }
        Button upgradeBtn = buttonTransform.GetComponent<Button>();
        TextMeshProUGUI costText = upgradeBtn.GetComponentInChildren<TextMeshProUGUI>();
        
        // Configurar textos
        if (nameText != null)
            nameText.text = stat.statName + ":";
        
        if (valueText != null)
            valueText.text = $"{stat.currentLevel}/{stat.maxLevel}";
        
        // Configurar botón
        int upgradeCost = stat.GetUpgradeCost();
        bool canUpgrade = stat.CanUpgrade() && ship.IsOwned();
        int currentScrap = SaveManager.Instance != null ? SaveManager.Instance.GetScrap() : 0;
        bool canAfford = currentScrap >= upgradeCost;
        
        if (upgradeBtn != null)
        {
            if (canUpgrade && canAfford)
            {
                upgradeBtn.interactable = true;
                if (costText != null)
                    costText.text = $"{upgradeCost}";
            }
            else if (!canUpgrade)
            {
                upgradeBtn.interactable = false;
                if (costText != null)
                    costText.text = "MAX";
            }
            else
            {
                upgradeBtn.interactable = false;
                if (costText != null)
                    costText.text = $"{upgradeCost}";
            }
            
            upgradeBtn.onClick.AddListener(() => OnUpgradeStat(stat, ship, upgradeCost));
        }
    }
    
    void OnUpgradeStat(ShipStat stat, ShipData ship, int cost)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (SaveManager.Instance == null) return;
        
        if (SaveManager.Instance.GetScrap() < cost)
        {
            Debug.Log("Not enough scrap!");
            return;
        }
        
        if (!SaveManager.Instance.SpendScrap(cost))
            return;
        
        stat.Upgrade();
        SaveManager.Instance.UpgradeShipStat(ship.shipName, stat.statName);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.upgradeSFX);
        
        UpdateUI();
    }
    
    void UpdateActionButtons(ShipData ship)
    {
        bool isOwned = ship.IsOwned();
        bool isEquipped = ship.IsEquipped();
        
        // Botón Purchase
        if (purchaseButton != null)
        {
            purchaseButton.gameObject.SetActive(!isOwned);
            
            int currentScrap = SaveManager.Instance != null ? SaveManager.Instance.GetScrap() : 0;
            bool canAfford = currentScrap >= ship.purchaseCost;
            purchaseButton.interactable = canAfford;
            
            if (purchaseButtonText != null)
                purchaseButtonText.text = $"PURCHASE ({ship.purchaseCost})";
        }
        
        // Botón Equip
        if (equipButton != null)
        {
            equipButton.gameObject.SetActive(isOwned && !isEquipped);
        }
        
        // Botón Launch siempre visible
        if (launchButton != null)
        {
            launchButton.interactable = true;
        }
    }
    
    void PurchaseShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        ShipData ship = allShips[currentShipIndex];
        
        if (SaveManager.Instance == null) return;
        
        if (SaveManager.Instance.GetScrap() < ship.purchaseCost)
        {
            Debug.Log("Not enough scrap!");
            return;
        }
        
        if (SaveManager.Instance.SpendScrap(ship.purchaseCost))
        {
            ship.Unlock();
            ship.Equip();
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.upgradeSFX);
            
            UpdateUI();
        }
    }
    
    void EquipShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        ShipData ship = allShips[currentShipIndex];
        ship.Equip();
        
        UpdateUI();
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
 