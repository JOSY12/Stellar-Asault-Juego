using UnityEngine;
using System;

/// <summary>
/// Gestor central de economía del juego
/// Maneja coins (soft currency) y gems (premium currency)
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Monedas Actuales")]
    public int coins = 0;
    public int gems = 0;

    [Header("Multiplicadores")]
    public float coinMultiplier = 1f;
    public float offlineEarningRate = 0.5f; // 50% de lo que ganarías jugando
    public int maxOfflineHours = 2;

    [Header("Offline Earnings")]
    private DateTime lastPlayTime;
    private int offlineCoinsEarned = 0;
    
    // Eventos para actualizar UI
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CalculateOfflineEarnings();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    #region Currency Management

    /// <summary>
    /// Añade coins con multiplicador aplicado
    /// </summary>
    public void AddCoins(int amount)
    {
        int finalAmount = Mathf.RoundToInt(amount * coinMultiplier);
        coins += finalAmount;
        OnCoinsChanged?.Invoke(coins);
        
        // Mostrar número flotante en pantalla (implementar en UI)
        ShowFloatingNumber(finalAmount, NumberType.Coins);
    }

    /// <summary>
    /// Intenta gastar coins
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            OnCoinsChanged?.Invoke(coins);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Añade gems (premium currency)
    /// </summary>
    public void AddGems(int amount)
    {
        gems += amount;
        OnGemsChanged?.Invoke(gems);
        ShowFloatingNumber(amount, NumberType.Gems);
    }

    /// <summary>
    /// Intenta gastar gems
    /// </summary>
    public bool SpendGems(int amount)
    {
        if (gems >= amount)
        {
            gems -= amount;
            OnGemsChanged?.Invoke(gems);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Verifica si tienes suficientes coins
    /// </summary>
    public bool CanAfford(int amount)
    {
        return coins >= amount;
    }

    #endregion

    #region Offline Earnings

    /// <summary>
    /// Calcula ganancias mientras no jugabas
    /// </summary>
    void CalculateOfflineEarnings()
    {
        if (lastPlayTime == DateTime.MinValue)
        {
            lastPlayTime = DateTime.Now;
            return;
        }

        TimeSpan timePassed = DateTime.Now - lastPlayTime;
        
        // Limitar a máximo de horas configuradas
        float hoursOffline = Mathf.Min((float)timePassed.TotalHours, maxOfflineHours);
        
        // Calcular coins ganadas (asumiendo 100 coins/hora base)
        float baseCoinsPerHour = 100f * coinMultiplier;
        offlineCoinsEarned = Mathf.RoundToInt(hoursOffline * baseCoinsPerHour * offlineEarningRate);

        if (offlineCoinsEarned > 0)
        {
            // Mostrar popup de offline earnings (implementar en UI Manager)
            Debug.Log($"Ganaste {offlineCoinsEarned} coins mientras no jugabas!");
            // UIManager.Instance.ShowOfflineEarningsPopup(offlineCoinsEarned);
        }

        lastPlayTime = DateTime.Now;
    }

    /// <summary>
    /// Reclama earnings offline (sin AD)
    /// </summary>
    public void ClaimOfflineEarnings()
    {
        AddCoins(offlineCoinsEarned);
        offlineCoinsEarned = 0;
    }

    /// <summary>
    /// Reclama earnings offline con multiplicador de AD (x3)
    /// </summary>
    public void ClaimOfflineEarningsWithAd()
    {
        // Aquí llamarías a tu Ad Manager
        // AdManager.Instance.ShowRewardedAd(() => {
            AddCoins(offlineCoinsEarned * 3);
            offlineCoinsEarned = 0;
        // });
        
        Debug.Log("Mostrando AD para x3 offline earnings...");
    }

    #endregion

    #region Save/Load

    void SaveGame()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.SetFloat("CoinMultiplier", coinMultiplier);
        PlayerPrefs.SetString("LastPlayTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    void LoadGame()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        gems = PlayerPrefs.GetInt("Gems", 0);
        coinMultiplier = PlayerPrefs.GetFloat("CoinMultiplier", 1f);
        
        string lastTimeStr = PlayerPrefs.GetString("LastPlayTime", "");
        if (!string.IsNullOrEmpty(lastTimeStr))
        {
            DateTime.TryParse(lastTimeStr, out lastPlayTime);
        }
    }

    #endregion

    #region Visual Feedback

    enum NumberType { Coins, Gems, Damage }

    void ShowFloatingNumber(int amount, NumberType type)
    {
        // Implementar en UIManager con pooling de objetos
        // Por ahora solo debug
        Debug.Log($"+{amount} {type}");
    }

    #endregion

    #region Utility

    /// <summary>
    /// Formatea números grandes (1000 -> 1K, 1000000 -> 1M)
    /// </summary>
    public static string FormatNumber(int num)
    {
        if (num >= 1000000)
            return (num / 1000000f).ToString("0.0") + "M";
        if (num >= 1000)
            return (num / 1000f).ToString("0.0") + "K";
        return num.ToString();
    }

    #endregion
}