using UnityEngine;

[System.Serializable]
public class ShipStat
{
    public string statName;
    public float baseValue;
    public float maxValue;
    public int maxLevel = 5;
    public int baseUpgradeCost = 10;
    
    
    [HideInInspector] public float currentValue;
    [HideInInspector] public int currentLevel = 1;
    
    public void Initialize()
    {
        currentValue = baseValue;
        currentLevel = 1;
    }
    
    public int GetUpgradeCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(1.5f, currentLevel - 1));
    }
    
    public bool CanUpgrade()
    {
        return currentLevel < maxLevel;
    }
    
    public void Upgrade()
    {
        if (!CanUpgrade()) return;
        
        currentLevel++;
        float increment = (maxValue - baseValue) / (maxLevel - 1);
        currentValue += increment;
    }
    
    public void LoadFromSave(string shipName)
    {
        if (SaveManager.Instance != null)
        {
            currentLevel = SaveManager.Instance.GetShipStatLevel(shipName, statName);
            
            // Calcular valor actual basado en nivel guardado
            float increment = (maxValue - baseValue) / (maxLevel - 1);
            currentValue = baseValue + (increment * (currentLevel - 1));
        }
        else
        {
            Initialize();
        }
    }
}

[CreateAssetMenu(fileName = "NewShip", menuName = "Game/Ship Data")]
public class ShipData : ScriptableObject
{
    [Header("Ship Info")]
    public string shipName = "Starter";
    public Sprite shipSprite;
    public int purchaseCost = 0;
        [Header("Bullet Type")]  // ← NUEVO
    public BulletData bulletData;  // ← NUEVO
    
    [Header("Ship Stats")]
    public ShipStat damage;
    public ShipStat fireRate;
    public ShipStat moveSpeed;
    public ShipStat health;
    public ShipStat bulletSpeed;
    
    public void InitializeStats()
    {
        damage.Initialize();
        fireRate.Initialize();
        moveSpeed.Initialize();
        health.Initialize();
        bulletSpeed.Initialize();
    }
    
    public void LoadProgress()
    {
        damage.LoadFromSave(shipName);
        fireRate.LoadFromSave(shipName);
        moveSpeed.LoadFromSave(shipName);
        health.LoadFromSave(shipName);
        bulletSpeed.LoadFromSave(shipName);
    }
    
    public bool IsOwned()
    {
        if (SaveManager.Instance == null) return shipName == "Starter";
        return SaveManager.Instance.IsShipOwned(shipName);
    }
    
    public void Unlock()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnlockShip(shipName);
    }
    
    public bool IsEquipped()
    {
        if (SaveManager.Instance == null) return shipName == "Starter";
        return SaveManager.Instance.GetEquippedShip() == shipName;
    }
    
    public void Equip()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.EquipShip(shipName);
    }
}