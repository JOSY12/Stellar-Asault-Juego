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
    
    [Header("Stats Tab Content - NEW")]
    public TextMeshProUGUI survivalTimeText;
    
    [Header("Upgrades Tab - Ship Actions")]
    public Button purchaseShipButton;
    public Button equipShipButton;
    public TextMeshProUGUI purchaseButtonText;
    
    [Header("Death Options - NEW")]
    public Button continueButton;
    public Button saveScrapButton;
    public Button endRunButton;
    public TextMeshProUGUI saveScrapSubText;
    public TextMeshProUGUI endRunSubText;
    
    [Header("Main Buttons")]
    public Button retryButton;
    public Button watchAdButton;
    public Button mainMenuButton;
 
    [Header("Ad Scrap Button")]
    public Button adScrapButton;
    public TextMeshProUGUI adScrapButtonText;
    
    [Header("Ship Data")]
    public ShipData[] allShips;
    private int currentShipIndex = 0;
    
    // Threading fix
    private bool shouldExecuteBonusScrap = false;
    private bool shouldExecuteContinue = false;
    private bool shouldExecuteSaveScrap = false;
    private bool saveScrapSuccess = false;
    
    // ═══ NUEVO: Cooldown para evitar spam ═══
    private float bonusScrapCooldown = 0f;
    private bool hasContinued = false; // Para evitar múltiples continues
    // ═══════════════════════════════════════
    
    void Start()
    {
        if (adScrapButton != null)
            adScrapButton.onClick.AddListener(OnWatchAdForScrap);
        
        if (deathPanel != null)
            deathPanel.SetActive(false);
        
        if (statsTabButton != null)
            statsTabButton.onClick.AddListener(() => ShowTab(true));
        
        if (upgradesTabButton != null)
            upgradesTabButton.onClick.AddListener(() => ShowTab(false));
        
        if (prevShipButton != null)
            prevShipButton.onClick.AddListener(PreviousShip);
        
        if (nextShipButton != null)
            nextShipButton.onClick.AddListener(NextShip);
        
        if (purchaseShipButton != null)
            purchaseShipButton.onClick.AddListener(PurchaseShip);
        
        if (equipShipButton != null)
            equipShipButton.onClick.AddListener(EquipShip);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        
        if (saveScrapButton != null)
            saveScrapButton.onClick.AddListener(OnSaveScrap);
        
        if (endRunButton != null)
            endRunButton.onClick.AddListener(OnEndRun);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenu);
    }
    
    void Update()
    {
        // Threading fix
        if (shouldExecuteBonusScrap)
        {
            shouldExecuteBonusScrap = false;
            ExecuteBonusScrap();
        }
        
        if (shouldExecuteContinue)
        {
            shouldExecuteContinue = false;
            ExecuteContinue();
        }
        
        if (shouldExecuteSaveScrap)
        {
            shouldExecuteSaveScrap = false;
            ExecuteSaveScrap(saveScrapSuccess);
        }
        
        // ═══ NUEVO: Actualizar cooldown ═══
        if (bonusScrapCooldown > 0f)
        {
            bonusScrapCooldown -= Time.unscaledDeltaTime; // unscaled porque puede estar pausado
        }
        // ═════════════════════════════════
        
        // ═══ NUEVO: Actualizar estado de botones cada frame ═══
        UpdateButtonStates();
        // ═════════════════════════════════════════════════════
    }
    
    // ═══ NUEVO: Actualizar estado de botones según ads disponibles ═══
    void UpdateButtonStates()
    {
        bool adManagerExists = AdManager.Instance != null;
        bool rewardedAdReady = adManagerExists && AdManager.Instance.IsRewardedAdReady();
        
        // Botón de bonus scrap
        if (adScrapButton != null)
        {
            bool canGetBonus = bonusScrapCooldown <= 0f;
            adScrapButton.interactable = canGetBonus && rewardedAdReady;
            
            if (adScrapButtonText != null)
            {
                if (!canGetBonus)
                {
                    int secondsLeft = Mathf.CeilToInt(bonusScrapCooldown);
                    adScrapButtonText.text = $"Wait {secondsLeft}s";
                }
                else if (!rewardedAdReady)
                {
                    adScrapButtonText.text = "AD LOADING...";
                }
                else
                {
                    adScrapButtonText.text = "WATCH AD FOR +50 SCRAP";
                }
            }
        }
        
        // Botón de continue
        if (continueButton != null && continueButton.gameObject.activeSelf)
        {
            bool canContinue = !hasContinued && rewardedAdReady;
            continueButton.interactable = canContinue;
            
            TextMeshProUGUI continueText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (continueText != null)
            {
                if (hasContinued)
                {
                    continueText.text = "USED";
                }
                else if (!rewardedAdReady)
                {
                    continueText.text = "AD LOADING...";
                }
                else
                {
                    continueText.text = "WATCH AD TO CONTINUE";
                }
            }
        }
        
        // Botón de save scrap
        if (saveScrapButton != null)
        {
            saveScrapButton.interactable = rewardedAdReady;
            
            if (saveScrapSubText != null && !rewardedAdReady)
            {
                saveScrapSubText.text = "Ad loading...";
            }
        }
    }
    // ═════════════════════════════════════════════════════════════════
    
    void OnWatchAdForScrap()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        // ═══ VALIDACIÓN: Cooldown ═══
        if (bonusScrapCooldown > 0f)
        {
            Debug.Log($"Bonus scrap on cooldown: {bonusScrapCooldown}s left");
            return;
        }
        // ════════════════════════════════
        
        // ═══ VALIDACIÓN: AdManager y ad ready ═══
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("AdManager not found - bonus scrap disabled");
            return;
        }
        
        if (!AdManager.Instance.IsRewardedAdReady())
        {
            Debug.LogWarning("Rewarded ad not ready yet");
            return;
        }
        // ═══════════════════════════════════════
        
        Debug.Log("Showing bonus scrap ad...");
        
        AdManager.Instance.ShowRewardedAd("bonus_scrap", (success) =>
        {
            Debug.Log($"Bonus scrap ad callback: {success}");
            
            if (success)
            {
                shouldExecuteBonusScrap = true;
            }
        });
    }
    
    void ExecuteBonusScrap()
    {
        Debug.Log("Executing bonus scrap reward");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.AddScrap(50);
            UpdateUpgradesTab();
            
            // ═══ NUEVO: Cooldown de 3 segundos ═══
            bonusScrapCooldown = 3f;
            // ═════════════════════════════════════
        }
    }
    
    public void ShowDeathScreen(int wave, int kills, int scrapEarned, int scrapWithoutAd, float survivalTime, bool hasUsedContinue)
    {
        // ═══ RESET: Estado de continue ═══
        hasContinued = hasUsedContinue;
        // ═════════════════════════════════
        
        if (deathPanel != null)
            deathPanel.SetActive(true);
        
        ShowTab(true);
        
        if (waveReachedText != null)
            waveReachedText.text = $"WAVE {wave}";
        
        if (killsText != null)
            killsText.text = $"KILLS: {kills}";
        
        if (scrapEarnedText != null)
            scrapEarnedText.text = $"{scrapEarned}";
        
        if (permanentScrapText != null)
            permanentScrapText.text = $"WITHOUT AD: {scrapWithoutAd} (40%)";
        
        if (survivalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(survivalTime / 60f);
            int seconds = Mathf.FloorToInt(survivalTime % 60f);
            survivalTimeText.text = $"TIME: {minutes:00}:{seconds:00}";
        }
        
        if (saveScrapSubText != null)
            saveScrapSubText.text = $"WATCH AD TO SAVE ALL {scrapEarned}";
        
        if (endRunSubText != null)
            endRunSubText.text = $"Save {scrapWithoutAd} scrap (40%)";
        
        if (continueButton != null)
            continueButton.gameObject.SetActive(!hasUsedContinue);
        
        LoadEquippedShip();
        UpdateUpgradesTab();
    }
    
    void ShowTab(bool showStats)
    {
        if (statsTab != null)
            statsTab.SetActive(showStats);
        
        if (upgradesTab != null)
            upgradesTab.SetActive(!showStats);
        
        if (statsTabButton != null)
        {
            ColorBlock colors = statsTabButton.colors;
            colors.normalColor = showStats ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f);
            statsTabButton.colors = colors;
        }
        
        if (upgradesTabButton != null)
        {
            ColorBlock colors = upgradesTabButton.colors;
            colors.normalColor = !showStats ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f);
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
        
        if (scrapDisplayText != null && SaveManager.Instance != null)
        {
            scrapDisplayText.text = $"{SaveManager.Instance.GetScrap()}";
        }
        
        if (shipNameText != null)
            shipNameText.text = ship.shipName.ToUpper();
        
        if (shipPreviewImage != null && ship.shipSprite != null)
            shipPreviewImage.sprite = ship.shipSprite;
        
        UpdateShipStatus(ship);
        UpdateStats(ship);
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
        
        foreach (Transform child in statsContainer)
            Destroy(child.gameObject);
        
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
        
        if (nameText != null)
            nameText.text = stat.statName + ":";
        
        if (valueText != null)
            valueText.text = $"{stat.currentLevel}/{stat.maxLevel}";
        
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
        
        if (purchaseShipButton != null)
        {
            purchaseShipButton.gameObject.SetActive(!isOwned);
            
            int currentScrap = SaveManager.Instance != null ? SaveManager.Instance.GetScrap() : 0;
            bool canAfford = currentScrap >= ship.purchaseCost;
            purchaseShipButton.interactable = canAfford;
            
            if (purchaseButtonText != null)
                purchaseButtonText.text = $"BUY({ship.purchaseCost})";
        }
        
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
    
    void OnContinue()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        // ═══ VALIDACIÓN: Ya usó continue ═══
        if (hasContinued)
        {
            Debug.Log("Continue already used!");
            return;
        }
        // ═══════════════════════════════════
        
        // ═══ VALIDACIÓN: AdManager y ad ready ═══
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("AdManager not found - continue disabled");
            return;
        }
        
        if (!AdManager.Instance.IsRewardedAdReady())
        {
            Debug.LogWarning("Rewarded ad not ready for continue");
            return;
        }
        // ═══════════════════════════════════════
        
        Debug.Log("Showing continue ad...");
        
        // ═══ MARCAR como usado ANTES del ad ═══
        hasContinued = true;
        // ══════════════════════════════════════
        
        AdManager.Instance.ShowRewardedAd("continue", (success) =>
        {
            Debug.Log($"Continue ad callback: {success}");
            
            if (success)
            {
                shouldExecuteContinue = true;
            }
            else
            {
                // Si falló el ad, desmarcar
                hasContinued = false;
            }
        });
    }
    
    void ExecuteContinue()
    {
        Debug.Log("Executing continue");
        
        if (deathPanel != null)
            deathPanel.SetActive(false);
        
        if (GameManager.Instance != null)
            GameManager.Instance.ContinueRun();
    }

    void OnSaveScrap()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        // ═══ VALIDACIÓN: AdManager y ad ready ═══
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("AdManager not found - saving partial scrap");
            ExecuteSaveScrap(false);
            return;
        }
        
        if (!AdManager.Instance.IsRewardedAdReady())
        {
            Debug.LogWarning("Rewarded ad not ready - saving partial scrap");
            ExecuteSaveScrap(false);
            return;
        }
        // ═══════════════════════════════════════
        
        Debug.Log("Showing save scrap ad...");
        
        AdManager.Instance.ShowRewardedAd("full_scrap", (success) =>
        {
            Debug.Log($"Save scrap ad callback: {success}");
            
            saveScrapSuccess = success;
            shouldExecuteSaveScrap = true;
        });
    }
    
    void ExecuteSaveScrap(bool success)
    {
        Debug.Log($"Executing save scrap: {success}");
        
        if (GameManager.Instance != null)
            GameManager.Instance.EndRun(success);
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    void OnEndRun()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndRun(false);
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    void OnMainMenu()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndRun(false);
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}