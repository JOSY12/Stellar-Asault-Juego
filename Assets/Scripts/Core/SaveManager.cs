using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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
    
    // ============ PALETAS ============
    public bool IsPaletteUnlocked(int paletteIndex)
    {
        return PlayerPrefs.GetInt($"Palette_{paletteIndex}_Unlocked", 0) == 1;
    }
    
    public void UnlockPalette(int paletteIndex)
    {
        PlayerPrefs.SetInt($"Palette_{paletteIndex}_Unlocked", 1);
        PlayerPrefs.Save();
    }
    
    public int GetCurrentPalette()
    {
        return PlayerPrefs.GetInt("CurrentPalette", 0);

    }
    
    public void SetCurrentPalette(int paletteIndex)
    {
        PlayerPrefs.SetInt("CurrentPalette", paletteIndex);
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
    
    // ============ RESET (para testing) ============
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All save data cleared!");
    }
}