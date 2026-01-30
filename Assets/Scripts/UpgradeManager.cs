using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Sistema de upgrades incrementales
/// Cada upgrade tiene costo exponencial y beneficio escalable
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Referencias")]
    public PlayerController player;

    // Niveles actuales de cada upgrade
    [Header("Niveles de Upgrades")]
    public int damageLevel = 1;
    public int fireRateLevel = 1;
    public int bulletCountLevel = 1;
    public int moveSpeedLevel = 1;
    public int maxHealthLevel = 1;
    public int coinMultiplierLevel = 1;
    public int critChanceLevel = 1;

    // Configs de balance
    [Header("Balance de Costos")]
    public float costMultiplier = 1.5f; // Cada nivel cuesta x1.5 más
    public int baseDamageCost = 50;
    public int baseFireRateCost = 75;
    public int baseBulletCountCost = 200;
    public int baseMoveSpeedCost = 60;
    public int baseHealthCost = 100;
    public int baseCoinMultCost = 150;
    public int baseCritCost = 250;

    // Stats base
    private int baseDamage = 1;
    private float baseFireRate = 0.5f;
    private float baseMoveSpeed = 8f;
    private int baseMaxHealth = 3;
    private float baseCoinMult = 1f;

    // Eventos para UI
    public event Action OnUpgradeChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyAllUpgrades();
    }

    #region Upgrade Methods

    /// <summary>
    /// Mejora el daño del player
    /// </summary>
    public bool UpgradeDamage()
    {
        int cost = GetUpgradeCost(damageLevel, baseDamageCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            damageLevel++;
            ApplyDamageUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyDamageUpgrade()
    {
        // Progresión exponencial: 1, 2, 4, 8, 16, 32...
        int newDamage = baseDamage * (int)Mathf.Pow(2, damageLevel - 1);
        
        if (player != null && player.bulletPrefab != null)
        {
            Bala bala = player.bulletPrefab.GetComponent<Bala>();
            if (bala != null) bala.damage = newDamage;
        }
        
        Debug.Log($"Daño mejorado a nivel {damageLevel}: {newDamage} damage");
    }

    /// <summary>
    /// Mejora la cadencia de disparo
    /// </summary>
    public bool UpgradeFireRate()
    {
        int cost = GetUpgradeCost(fireRateLevel, baseFireRateCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            fireRateLevel++;
            ApplyFireRateUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyFireRateUpgrade()
    {
        // Reduce delay: 0.5s -> 0.4s -> 0.3s -> 0.2s -> 0.15s...
        float newFireRate = baseFireRate / (1 + (fireRateLevel - 1) * 0.25f);
        newFireRate = Mathf.Max(0.05f, newFireRate); // Mínimo 0.05s
        
        if (player != null)
        {
            player.fireRate = newFireRate;
        }
        
        Debug.Log($"Fire Rate mejorado a nivel {fireRateLevel}: {newFireRate:F2}s delay");
    }

    /// <summary>
    /// Mejora cantidad de balas disparadas
    /// </summary>
    public bool UpgradeBulletCount()
    {
        int cost = GetUpgradeCost(bulletCountLevel, baseBulletCountCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            bulletCountLevel++;
            ApplyBulletCountUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyBulletCountUpgrade()
    {
        // Niveles especiales de bullets
        int bulletCount = 1;
        if (bulletCountLevel >= 15) bulletCount = 8;      // 360° círculo
        else if (bulletCountLevel >= 10) bulletCount = 5; // 180° fan
        else if (bulletCountLevel >= 5) bulletCount = 3;  // 90° spread
        else if (bulletCountLevel >= 3) bulletCount = 2;  // 45° doble
        
    if (player != null)
    {
        player.SetBulletCount(bulletCount); // ← NUEVO
    }
        // Modificar PlayerController para soportar multi-shot
        Debug.Log($"Bullet Count mejorado a nivel {bulletCountLevel}: {bulletCount} balas");
    }

 

    /// <summary>
    /// Mejora velocidad de movimiento
    /// </summary>
    public bool UpgradeMoveSpeed()
    {
        int cost = GetUpgradeCost(moveSpeedLevel, baseMoveSpeedCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            moveSpeedLevel++;
            ApplyMoveSpeedUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyMoveSpeedUpgrade()
    {
        float newSpeed = baseMoveSpeed + (moveSpeedLevel - 1) * 0.5f;
        newSpeed = Mathf.Min(20f, newSpeed); // Máximo 20
        
        if (player != null)
        {
            player.moveSpeed = newSpeed;
        }
        
        Debug.Log($"Move Speed mejorado a nivel {moveSpeedLevel}: {newSpeed:F1}");
    }

    /// <summary>
    /// Mejora vida máxima
    /// </summary>
    public bool UpgradeMaxHealth()
    {
        int cost = GetUpgradeCost(maxHealthLevel, baseHealthCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            maxHealthLevel++;
            ApplyMaxHealthUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyMaxHealthUpgrade()
    {
        int newMaxHealth = baseMaxHealth + (maxHealthLevel - 1);
        newMaxHealth = Mathf.Min(10, newMaxHealth); // Máximo 10 HP
        
        // Aplicar a PlayerHealth cuando lo implementes
        Debug.Log($"Max Health mejorado a nivel {maxHealthLevel}: {newMaxHealth} HP");
    }

    /// <summary>
    /// Mejora multiplicador de coins
    /// </summary>
    public bool UpgradeCoinMultiplier()
    {
        int cost = GetUpgradeCost(coinMultiplierLevel, baseCoinMultCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            coinMultiplierLevel++;
            ApplyCoinMultiplierUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyCoinMultiplierUpgrade()
    {
        float newMult = baseCoinMult + (coinMultiplierLevel - 1) * 0.1f;
        
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.coinMultiplier = newMult;
        }
        
        Debug.Log($"Coin Multiplier mejorado a nivel {coinMultiplierLevel}: x{newMult:F1}");
    }

    /// <summary>
    /// Mejora chance de crítico
    /// </summary>
    public bool UpgradeCritChance()
    {
        int cost = GetUpgradeCost(critChanceLevel, baseCritCost);
        
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            critChanceLevel++;
            ApplyCritChanceUpgrade();
            OnUpgradeChanged?.Invoke();
            SaveUpgrades();
            return true;
        }
        return false;
    }

    void ApplyCritChanceUpgrade()
    {
        // 0% -> 5% -> 10% -> 15% -> ... -> 50% max
        float newCritChance = Mathf.Min(0.5f, critChanceLevel * 0.05f);
        
        // Aplicar a Bala cuando implementes sistema de críticos
        Debug.Log($"Crit Chance mejorado a nivel {critChanceLevel}: {newCritChance * 100}%");
    }

    #endregion

    #region Cost Calculation

    /// <summary>
    /// Calcula costo de siguiente nivel usando fórmula exponencial
    /// </summary>
    public int GetUpgradeCost(int currentLevel, int baseCost)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel - 1));
    }

    /// <summary>
    /// Obtiene info completa de un upgrade
    /// </summary>
    public UpgradeInfo GetUpgradeInfo(UpgradeType type)
    {
        UpgradeInfo info = new UpgradeInfo();
        
        switch (type)
        {
            case UpgradeType.Damage:
                info.currentLevel = damageLevel;
                info.cost = GetUpgradeCost(damageLevel, baseDamageCost);
                info.currentValue = baseDamage * (int)Mathf.Pow(2, damageLevel - 1);
                info.nextValue = baseDamage * (int)Mathf.Pow(2, damageLevel);
                info.name = "Daño";
                break;
            case UpgradeType.FireRate:
                info.currentLevel = fireRateLevel;
                info.cost = GetUpgradeCost(fireRateLevel, baseFireRateCost);
                info.currentValue = baseFireRate / (1 + (fireRateLevel - 1) * 0.25f);
                info.nextValue = baseFireRate / (1 + fireRateLevel * 0.25f);
                info.name = "Cadencia";
                break;
            // Agregar resto de upgrades...
        }
        
        info.canAfford = CurrencyManager.Instance.CanAfford(info.cost);
        return info;
    }

    #endregion

    #region Apply All

    void ApplyAllUpgrades()
    {
        ApplyDamageUpgrade();
        ApplyFireRateUpgrade();
        ApplyBulletCountUpgrade();
        ApplyMoveSpeedUpgrade();
        ApplyMaxHealthUpgrade();
        ApplyCoinMultiplierUpgrade();
        ApplyCritChanceUpgrade();
    }

    #endregion

    #region Save/Load

    void SaveUpgrades()
    {
        PlayerPrefs.SetInt("DamageLevel", damageLevel);
        PlayerPrefs.SetInt("FireRateLevel", fireRateLevel);
        PlayerPrefs.SetInt("BulletCountLevel", bulletCountLevel);
        PlayerPrefs.SetInt("MoveSpeedLevel", moveSpeedLevel);
        PlayerPrefs.SetInt("MaxHealthLevel", maxHealthLevel);
        PlayerPrefs.SetInt("CoinMultLevel", coinMultiplierLevel);
        PlayerPrefs.SetInt("CritChanceLevel", critChanceLevel);
        PlayerPrefs.Save();
    }

    void LoadUpgrades()
    {
        damageLevel = PlayerPrefs.GetInt("DamageLevel", 1);
        fireRateLevel = PlayerPrefs.GetInt("FireRateLevel", 1);
        bulletCountLevel = PlayerPrefs.GetInt("BulletCountLevel", 1);
        moveSpeedLevel = PlayerPrefs.GetInt("MoveSpeedLevel", 1);
        maxHealthLevel = PlayerPrefs.GetInt("MaxHealthLevel", 1);
        coinMultiplierLevel = PlayerPrefs.GetInt("CoinMultLevel", 1);
        critChanceLevel = PlayerPrefs.GetInt("CritChanceLevel", 1);
    }

    #endregion
}

#region Helper Classes

public enum UpgradeType
{
    Damage,
    FireRate,
    BulletCount,
    MoveSpeed,
    MaxHealth,
    CoinMultiplier,
    CritChance
}

[System.Serializable]
public class UpgradeInfo
{
    public string name;
    public int currentLevel;
    public int cost;
    public float currentValue;
    public float nextValue;
    public bool canAfford;
}

#endregion