using UnityEngine;

public class ManagerChecker : MonoBehaviour
{
    [Header("Managers Prefab (Optional)")]
    public GameObject managersPrefab;
    
    [Header("Testing Zone (Optional)")]
    public ZoneData testZone;
    
    void Awake()
    {
        CheckAndCreateManagers();
    }
    
    void CheckAndCreateManagers()
    {
        bool needsManagers = false;
        
        // Verificar si faltan managers
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager missing!");
            needsManagers = true;
        }
        
        if (ZoneManager.Instance == null)
        {
            Debug.LogWarning("ZoneManager missing!");
            needsManagers = true;
        }
        
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager missing!");
            needsManagers = true;
        }
        
        // Si faltan managers, crearlos
        if (needsManagers)
        {
            if (managersPrefab != null)
            {
                Debug.Log("Creating managers from prefab...");
                Instantiate(managersPrefab);
            }
            else
            {
                Debug.Log("Creating basic managers...");
                CreateBasicManagers();
            }
        }
        
        // Si hay zona de testing, usarla
        if (testZone != null && ZoneManager.Instance != null)
        {
            Debug.Log($"Using test zone: {testZone.zoneName}");
            
            // Crear un array temporal con solo esta zona
            ZoneManager.Instance.allZones = new ZoneData[] { testZone };
            ZoneManager.Instance.SetCurrentZone(0);
        }
    }
    
    void CreateBasicManagers()
    {
        // Crear SaveManager
        if (SaveManager.Instance == null)
        {
            GameObject smObj = new GameObject("SaveManager");
            smObj.AddComponent<SaveManager>();
            DontDestroyOnLoad(smObj);
        }
        
        // Crear ZoneManager
        if (ZoneManager.Instance == null)
        {
            GameObject zmObj = new GameObject("ZoneManager");
            zmObj.AddComponent<ZoneManager>();
            DontDestroyOnLoad(zmObj);
        }
        
        // Crear AudioManager
        if (AudioManager.Instance == null)
        {
            GameObject amObj = new GameObject("AudioManager");
            AudioManager am = amObj.AddComponent<AudioManager>();
            am.musicSource = amObj.AddComponent<AudioSource>();
            am.sfxSource = amObj.AddComponent<AudioSource>();
            DontDestroyOnLoad(amObj);
        }
        
        Debug.Log("Basic managers created!");
    }
}