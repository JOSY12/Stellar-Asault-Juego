using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;
    
    [Header("All Zones")]
    public ZoneData[] allZones;
    
    private int currentZoneIndex = 0;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (SaveManager.Instance != null)
        {
            currentZoneIndex = SaveManager.Instance.GetCurrentZone();
        }
        
        Debug.Log($"ZoneManager initialized. Current zone: {currentZoneIndex}");
    }
    
    public ZoneData GetCurrentZone()
    {
        if (allZones == null || allZones.Length == 0)
        {
            Debug.LogError("No zones assigned!");
            return null;
        }
        
        if (currentZoneIndex < 0 || currentZoneIndex >= allZones.Length)
        {
            currentZoneIndex = 0;
        }
        
        return allZones[currentZoneIndex];
    }
    
    public void SetCurrentZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= allZones.Length)
        {
            Debug.LogError($"Invalid zone index: {zoneIndex}");
            return;
        }
        
        currentZoneIndex = zoneIndex;
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetCurrentZone(zoneIndex);
        }
        
        Debug.Log($"Zone changed to: {allZones[zoneIndex].zoneName}");
    }
    
    public float GetScrapMultiplier()
    {
        ZoneData zone = GetCurrentZone();
        return zone != null ? zone.scrapMultiplier : 1f;
    }
}