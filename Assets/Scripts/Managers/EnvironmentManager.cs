using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Material nebulaMaterial; // Asignar el shader material en Unity
    
    [Header("Runtime")]
    private GameObject[] layerObjects;
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        if (ZoneManager.Instance != null)
        {
            ZoneData currentZone = ZoneManager.Instance.GetCurrentZone();
            if (currentZone != null)
            {
                CreateEnvironment(currentZone);
            }
            else
            {
                Debug.LogError("No current zone!");
            }
        }
        else
        {
            Debug.LogError("ZoneManager not found!");
        }
    }
    
    void CreateEnvironment(ZoneData zone)
    {
        if (Camera.main != null)
            Camera.main.backgroundColor = zone.ambientColor;
        
        if (zone.layers == null || zone.layers.Length == 0)
        {
            Debug.LogWarning("Zone has no layers!");
            return;
        }
        
        layerObjects = new GameObject[zone.layers.Length];
        
        for (int i = 0; i < zone.layers.Length; i++)
        {
            ZoneData.EnvironmentLayer layerConfig = zone.layers[i];
            
            if (!layerConfig.enabled) continue;
            
            GameObject layerObj = new GameObject($"Layer_{i}_{layerConfig.type}");
            layerObj.transform.parent = transform;
            layerObj.transform.position = new Vector3(0, 0, layerConfig.depth);
            
            EnvironmentLayer layer = layerObj.AddComponent<EnvironmentLayer>();
            layer.Initialize(layerConfig, player, nebulaMaterial);
            
            layerObjects[i] = layerObj;
            
            Debug.Log($"✓ Layer {i} created: {layerConfig.type} at depth {layerConfig.depth}");
        }
        
        Debug.Log($"Environment created for zone: {zone.zoneName}");
    }
    
    public void PauseEnvironment()
    {
        if (layerObjects == null) return;
        
        foreach (GameObject layerObj in layerObjects)
        {
            if (layerObj != null)
            {
                EnvironmentLayer layer = layerObj.GetComponent<EnvironmentLayer>();
                if (layer != null)
                    layer.enabled = false;
            }
        }
    }
    
    public void ResumeEnvironment()
    {
        if (layerObjects == null) return;
        
        foreach (GameObject layerObj in layerObjects)
        {
            if (layerObj != null)
            {
                EnvironmentLayer layer = layerObj.GetComponent<EnvironmentLayer>();
                if (layer != null)
                    layer.enabled = true;
            }
        }
    }
}