using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnlockStarterShip();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void UnlockStarterShip()
    {
        if (!PlayerPrefs.HasKey("FirstTimePlayed"))
        {
            PlayerPrefs.SetInt("Ship_Starter_Owned", 1);
            PlayerPrefs.SetString("EquippedShip", "Starter");
            PlayerPrefs.SetInt("Zone_0_Unlocked", 1);
            PlayerPrefs.SetInt("FirstTimePlayed", 1);
            PlayerPrefs.Save();
            
            Debug.Log("Starter ship and default zone unlocked!");
        }
    }
    
    // ============ ECONOMÍA ============
    public int GetScrap()
    {
        return PlayerPrefs.GetInt("TotalScrap", 0);
    }
    
    public void AddScrap(int amount)
    {
        int current = GetScrap();
        PlayerPrefs.SetInt("TotalScrap", current + amount);
        PlayerPrefs.Save();
    }
    
    public bool SpendScrap(int amount)
    {
        int current = GetScrap();
        if (current >= amount)
        {
            PlayerPrefs.SetInt("TotalScrap", current - amount);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
    
    // ============ NAVES ============
    public bool IsShipOwned(string shipName)
    {
        return PlayerPrefs.GetInt($"Ship_{shipName}_Owned", 0) == 1;
    }
    
    public void UnlockShip(string shipName)
    {
        PlayerPrefs.SetInt($"Ship_{shipName}_Owned", 1);
        PlayerPrefs.Save();
    }
    
    public string GetEquippedShip()
    {
        return PlayerPrefs.GetString("EquippedShip", "Starter");
    }
    
    public void EquipShip(string shipName)
    {
        PlayerPrefs.SetString("EquippedShip", shipName);
        PlayerPrefs.Save();
    }
    
    // ============ MEJORAS DE NAVE ============
    public int GetShipStatLevel(string shipName, string statName)
    {
        return PlayerPrefs.GetInt($"{shipName}_{statName}_Level", 1);
    }
    
    public void UpgradeShipStat(string shipName, string statName)
    {
        int currentLevel = GetShipStatLevel(shipName, statName);
        PlayerPrefs.SetInt($"{shipName}_{statName}_Level", currentLevel + 1);
        PlayerPrefs.Save();
    }
    
    // ============ ZONAS ============
    public bool IsZoneUnlocked(int zoneIndex)
    {
        return PlayerPrefs.GetInt($"Zone_{zoneIndex}_Unlocked", 0) == 1;
    }
    
    public void UnlockZone(int zoneIndex)
    {
        PlayerPrefs.SetInt($"Zone_{zoneIndex}_Unlocked", 1);
        PlayerPrefs.Save();
    }
    
    public int GetCurrentZone()
    {
        return PlayerPrefs.GetInt("CurrentZone", 0);
    }
    
    public void SetCurrentZone(int zoneIndex)
    {
        PlayerPrefs.SetInt("CurrentZone", zoneIndex);
        PlayerPrefs.Save();
    }
    
    // ============ ESTADÍSTICAS ============
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
    
    public void SetHighScore(int score)
    {
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }
    
    public int GetTotalKills()
    {
        return PlayerPrefs.GetInt("TotalKills", 0);
    }
    
    public void AddKills(int amount)
    {
        int current = GetTotalKills();
        PlayerPrefs.SetInt("TotalKills", current + amount);
        PlayerPrefs.Save();
    }
    
    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat("BestTime", 0f);
    }
    
    public void SetBestTime(float time)
    {
        PlayerPrefs.SetFloat("BestTime", time);
        PlayerPrefs.Save();
    }
    
    // ============ CONFIGURACIÓN ============
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }
    
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 0.7f);
    }
    
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    
    public bool IsMusicEnabled()
    {
        return PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
    }
    
    public void SetMusicEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public bool IsSFXEnabled()
    {
        return PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }
    
    public void SetSFXEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("SFXEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public bool IsCameraShakeEnabled()
    {
        return PlayerPrefs.GetInt("CameraShakeEnabled", 1) == 1;
    }
    
    public void SetCameraShakeEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("CameraShakeEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public bool UseTouchZones()
    {
        return PlayerPrefs.GetInt("UseTouchZones", 0) == 1;
    }
    
    public void SetUseTouchZones(bool value)
    {
        PlayerPrefs.SetInt("UseTouchZones", value ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    // ============ RESET ============
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All save data cleared!");
    }
}