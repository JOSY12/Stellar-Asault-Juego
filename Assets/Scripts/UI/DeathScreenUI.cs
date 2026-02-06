using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathPanel;
    
    [Header("Tabs")]
    public GameObject statsTab;
    public GameObject upgradesTab;
    public Button statsTabButton;
    public Button upgradesTabButton;
    
    [Header("Stats Tab Content")]
    public TextMeshProUGUI waveReachedText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI scrapEarnedText;
    public TextMeshProUGUI permanentScrapText;
    
    [Header("Upgrades Tab - Ship Selection")]
    public TextMeshProUGUI scrapDisplayText;
    public TextMeshProUGUI shipNameText;
    public Image shipPreviewImage;
    public Button prevShipButton;
    public Button nextShipButton;
    public TextMeshProUGUI shipStatusText;
    
    [Header("Upgrades Tab - Stats")]
    public Transform statsContainer;
    public GameObject statRowPrefab;
    
    [Header("Upgrades Tab - Ship Actions")]
    public Button purchaseShipButton;
    public Button equipShipButton;
    public TextMeshProUGUI purchaseButtonText;
    
    [Header("Main Buttons")]
    public Button retryButton;
    public Button watchAdButton;
    public Button mainMenuButton;
    public TextMeshProUGUI adBonusText;
    
    [Header("Ship Data")]
    public ShipData[] allShips;
    private int currentShipIndex = 0;
    
    void Start()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
        
        // Conectar tabs
        if (statsTabButton != null)
            statsTabButton.onClick.AddListener(() => ShowTab(true));
        
        if (upgradesTabButton != null)
            upgradesTabButton.onClick.AddListener(() => ShowTab(false));
        
        // Conectar navegación de naves
        if (prevShipButton != null)
            prevShipButton.onClick.AddListener(PreviousShip);
        
        if (nextShipButton != null)
            nextShipButton.onClick.AddListener(NextShip);
        
        // Conectar acciones de nave
        if (purchaseShipButton != null)
            purchaseShipButton.onClick.AddListener(PurchaseShip);
        
        if (equipShipButton != null)
            equipShipButton.onClick.AddListener(EquipShip);
        
        // Conectar botones principales
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);
        
        if (watchAdButton != null)
            watchAdButton.onClick.AddListener(OnWatchAd);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    
    public void ShowDeathScreen(int wave, int kills, int scrapEarned, int permanentScrap)
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);
        
        // Mostrar tab de stats por defecto
        ShowTab(true);
        
        // Actualizar stats tab
        if (waveReachedText != null)
            waveReachedText.text = $"WAVE {wave}";
        
        if (killsText != null)
            killsText.text = $"KILLS: {kills}";
        
        if (scrapEarnedText != null)
            scrapEarnedText.text = $"SCRAP EARNED: {scrapEarned}";
        
        if (permanentScrapText != null)
            permanentScrapText.text = $"SAVED: {permanentScrap}";
        
        int adBonus = Mathf.FloorToInt(scrapEarned * 0.5f);
        if (adBonusText != null)
            adBonusText.text = $"+{adBonus}";
        
        // Cargar nave equipada para upgrades tab
        LoadEquippedShip();
        UpdateUpgradesTab();
    }
    
    void ShowTab(bool showStats)
    {
        if (statsTab != null)
            statsTab.SetActive(showStats);
        
        if (upgradesTab != null)
            upgradesTab.SetActive(!showStats);
        
        // Cambiar apariencia de botones de tab
        if (statsTabButton != null)
        {
            ColorBlock colors = statsTabButton.colors;
            colors.normalColor = showStats ?   new Color(0.5f, 0.5f, 0.5f) :new Color(1f, 1f, 1f);
            statsTabButton.colors = colors;
        }
        
        if (upgradesTabButton != null)
        {
            ColorBlock colors = upgradesTabButton.colors;
            colors.normalColor = !showStats ?     new Color(0.5f, 0.5f, 0.5f) :new Color(1f, 1f, 1f);
            upgradesTabButton.colors = colors;
        }
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
        
        UpdateUpgradesTab();
    }
    
    void NextShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        currentShipIndex++;
        if (currentShipIndex >= allShips.Length)
            currentShipIndex = 0;
        
        UpdateUpgradesTab();
    }
    
    void UpdateUpgradesTab()
    {
        if (allShips == null || currentShipIndex >= allShips.Length) return;
        
        ShipData ship = allShips[currentShipIndex];
        ship.LoadProgress();
        
        // Scrap disponible
        if (scrapDisplayText != null && SaveManager.Instance != null)
        {
            scrapDisplayText.text = $"SCRAP: {SaveManager.Instance.GetScrap()}";
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
        if (statsContainer == null || statRowPrefab == null) return;
        
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
    
    // Buscar componentes con verificación
    Transform nameTransform = row.transform.Find("StatName");
    if (nameTransform == null)
    {
        Debug.LogError("StatRow prefab is missing 'StatName' child!");
        Destroy(row);
        return;
    }
    TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
    
    Transform valueTransform = row.transform.Find("ValueText");
    if (valueTransform == null)
    {
        Debug.LogError("StatRow prefab is missing 'ValueText' child!");
        Destroy(row);
        return;
    }
    TextMeshProUGUI valueText = valueTransform.GetComponent<TextMeshProUGUI>();
    
    Transform buttonTransform = row.transform.Find("UpgradeButton");
    if (buttonTransform == null)
    {
        Debug.LogError("StatRow prefab is missing 'UpgradeButton' child!");
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
        
        if (!SaveManager.Instance.SpendScrap(cost))
        {
            Debug.Log("Not enough scrap!");
            return;
        }
        
        stat.Upgrade();
        SaveManager.Instance.UpgradeShipStat(ship.shipName, stat.statName);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.upgradeSFX);
        
        UpdateUpgradesTab();
    }
    
    void UpdateActionButtons(ShipData ship)
    {
        bool isOwned = ship.IsOwned();
        bool isEquipped = ship.IsEquipped();
        
        // Botón Purchase
        if (purchaseShipButton != null)
        {
            purchaseShipButton.gameObject.SetActive(!isOwned);
            
            int currentScrap = SaveManager.Instance != null ? SaveManager.Instance.GetScrap() : 0;
            bool canAfford = currentScrap >= ship.purchaseCost;
            purchaseShipButton.interactable = canAfford;
            
            if (purchaseButtonText != null)
                purchaseButtonText.text = $"PURCHASE ({ship.purchaseCost})";
        }
        
        // Botón Equip
        if (equipShipButton != null)
        {
            equipShipButton.gameObject.SetActive(isOwned && !isEquipped);
        }
    }
    
    void PurchaseShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        ShipData ship = allShips[currentShipIndex];
        
        if (SaveManager.Instance != null && SaveManager.Instance.SpendScrap(ship.purchaseCost))
        {
            ship.Unlock();
            ship.Equip();
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.upgradeSFX);
            
            UpdateUpgradesTab();
        }
    }
    
    void EquipShip()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        allShips[currentShipIndex].Equip();
        UpdateUpgradesTab();
    }
    
    void OnRetry()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
    
    void OnWatchAd()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (GameManager.Instance != null && SaveManager.Instance != null)
        {
            int scrapBonus = Mathf.FloorToInt(GameManager.Instance.scrapThisRun * 0.5f);
            SaveManager.Instance.AddScrap(scrapBonus);
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
    
    void OnMainMenu()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}