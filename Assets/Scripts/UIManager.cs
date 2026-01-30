using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestor central de UI del juego
/// Maneja todos los elementos de interfaz y popups
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Principal")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI waveText;
    public Image[] healthHearts; // Array de corazones para mostrar vida

    [Header("Pantallas")]
    public GameObject upgradePanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Popups")]
    public GameObject offlineEarningsPopup;
    public TextMeshProUGUI offlineEarningsText;
    public Button claimOfflineButton;
    public Button claimOfflineAdButton;

    [Header("Wave Complete")]
    public GameObject waveCompletePopup;
    public TextMeshProUGUI waveCompleteText;
    public Button claimBossAdButton;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverWaveText;
    public Button continueAdButton;
    public Button restartButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Suscribirse a eventos de CurrencyManager
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += UpdateCoinsUI;
            CurrencyManager.Instance.OnGemsChanged += UpdateGemsUI;
        }

        // Suscribirse a eventos de PlayerHealth
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
            playerHealth.OnPlayerDied += ShowGameOver;
        }

        // Inicializar UI
        if (CurrencyManager.Instance != null)
        {
            UpdateCoinsUI(CurrencyManager.Instance.coins);
            UpdateGemsUI(CurrencyManager.Instance.gems);
        }
        UpdateWaveUI();

        // Ocultar popups
        HideAllPopups();
    }

    void Update()
    {
        // Actualizar wave en tiempo real
        UpdateWaveUI();
    }

    #region HUD Updates

    void UpdateCoinsUI(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = CurrencyManager.FormatNumber(coins);
        }
    }

    void UpdateGemsUI(int gems)
    {
        if (gemsText != null)
        {
            gemsText.text = gems.ToString();
        }
    }

    void UpdateWaveUI()
    {
        if (waveText != null && WaveManager.Instance != null)
        {
            waveText.text = "Wave " + WaveManager.Instance.GetCurrentWave();
        }
    }

    void UpdateHealthUI(int current, int max)
    {
        if (healthHearts == null || healthHearts.Length == 0) return;

        for (int i = 0; i < healthHearts.Length; i++)
        {
            if (i < max)
            {
                healthHearts[i].gameObject.SetActive(true);
                healthHearts[i].enabled = (i < current);
            }
            else
            {
                healthHearts[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Offline Earnings Popup

    public void ShowOfflineEarningsPopup(int coins)
    {
        if (offlineEarningsPopup == null) return;

        offlineEarningsPopup.SetActive(true);
        
        if (offlineEarningsText != null)
        {
            offlineEarningsText.text = "Ganaste " + CurrencyManager.FormatNumber(coins) + " coins mientras no jugabas!";
        }

        // Configurar botones
        if (claimOfflineButton != null)
        {
            claimOfflineButton.onClick.RemoveAllListeners();
            claimOfflineButton.onClick.AddListener(() => {
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.ClaimOfflineEarnings();
                }
                offlineEarningsPopup.SetActive(false);
            });
        }

        if (claimOfflineAdButton != null)
        {
            claimOfflineAdButton.onClick.RemoveAllListeners();
            claimOfflineAdButton.onClick.AddListener(() => {
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.ClaimOfflineEarningsWithAd();
                }
                offlineEarningsPopup.SetActive(false);
            });
            
            // Mostrar claramente el x3
            TextMeshProUGUI adButtonText = claimOfflineAdButton.GetComponentInChildren<TextMeshProUGUI>();
            if (adButtonText != null)
            {
                adButtonText.text = "Claim x3\n(" + CurrencyManager.FormatNumber(coins * 3) + ")";
            }
        }
    }

    #endregion

    #region Wave Complete Popup

    public void ShowWaveComplete(int wave, int reward, bool isBossWave)
    {
        if (waveCompletePopup == null) return;

        waveCompletePopup.SetActive(true);
        
        if (waveCompleteText != null)
        {
            string message = isBossWave 
                ? "¡BOSS DERROTADO!\nWave " + wave + "\n+" + reward + " coins"
                : "Wave " + wave + " Complete!\n+" + reward + " coins";
            
            waveCompleteText.text = message;
        }

        // Mostrar botón de AD solo si fue boss wave
        if (claimBossAdButton != null)
        {
            claimBossAdButton.gameObject.SetActive(isBossWave);
            
            if (isBossWave)
            {
                claimBossAdButton.onClick.RemoveAllListeners();
                claimBossAdButton.onClick.AddListener(() => {
                    if (WaveManager.Instance != null)
                    {
                        WaveManager.Instance.ClaimBossRewardWithAd();
                    }
                    waveCompletePopup.SetActive(false);
                });
            }
        }

        // Auto-cerrar después de 3 segundos
        Invoke("HideWaveComplete", 3f);
    }

    void HideWaveComplete()
    {
        if (waveCompletePopup != null)
        {
            waveCompletePopup.SetActive(false);
        }
    }

    #endregion

    #region Game Over Screen

    void ShowGameOver()
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true);

        // Mostrar wave alcanzada
        if (gameOverWaveText != null && WaveManager.Instance != null)
        {
            gameOverWaveText.text = "Llegaste a la Wave " + WaveManager.Instance.GetCurrentWave();
        }

        // Configurar botón de continue con AD
        if (continueAdButton != null)
        {
            bool canContinue = WaveManager.Instance != null && WaveManager.Instance.GetCurrentWave() >= 5;
            continueAdButton.gameObject.SetActive(canContinue);

            if (canContinue)
            {
                continueAdButton.onClick.RemoveAllListeners();
                continueAdButton.onClick.AddListener(() => {
                    PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.ContinueWithAd();
                        gameOverPanel.SetActive(false);
                    }
                });
            }
        }

        // Configurar botón de restart
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => {
                PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.RestartGame();
                }
            });
        }
    }

    #endregion

    #region Panel Management

    public void ShowUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }
    }

    public void HideUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (pausePanel == null) return;

        bool isPaused = !pausePanel.activeSelf;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    void HideAllPopups()
    {
        if (offlineEarningsPopup != null) offlineEarningsPopup.SetActive(false);
        if (waveCompletePopup != null) waveCompletePopup.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    #endregion
}