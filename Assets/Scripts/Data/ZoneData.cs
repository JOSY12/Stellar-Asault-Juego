using UnityEngine;

[CreateAssetMenu(fileName = "Zone_New", menuName = "Space Shooter/Zone Data")]
public class ZoneData : ScriptableObject
{
    [Header("Zone Info")]
    public string zoneName = "New Zone";
    public int zoneIndex = 0;
    public float scrapMultiplier = 1f;
    public int unlockCost = 0; // 0 = gratis
    
    [Header("Visual Preview")]
    public Color ambientColor = new Color(0f, 0f, 0.04f, 1f);
    
    [Header("Environment Layers")]
    public EnvironmentLayer[] layers = new EnvironmentLayer[3];
    
    [System.Serializable]
    public class EnvironmentLayer
    {
        [Header("Layer Type")]
        public LayerType type = LayerType.Nebula;
        
        [Header("Layer Settings")]
        public bool enabled = true;
        public float parallaxSpeed = 0.3f;
        public float depth = -1f; // Z position
        
        [Header("Nebula Settings (if type = Nebula)")]
        public Color nebulaColor = new Color(0.2f, 0.5f, 0.8f, 0.5f);
        public float nebulaScale = 5f;
        public float nebulaAnimationSpeed = 0.5f;
        public float nebulaPixelation = 64f;
        public float nebulaContrast = 1.5f;
        
        [Header("Stars Settings (if type = Stars)")]
        public int starCount = 300;
        public Color starColor = Color.white;
        public float minStarSize = 0.05f;
        public float maxStarSize = 0.2f;
        
        [Header("Planets Settings (if type = Planets)")]
        public int planetCount = 3;
        public float minPlanetSize = 1f;
        public float maxPlanetSize = 3f;
        public Color[] planetColors = new Color[] { Color.gray, Color.red, Color.blue };
    }
    
    public enum LayerType
    {
        Nebula,
        Stars,
        Planets
    }
    
    public bool IsUnlocked()
    {
        if (SaveManager.Instance != null)
            return SaveManager.Instance.IsZoneUnlocked(zoneIndex);
        return zoneIndex == 0;
    }
    
    public void Unlock()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.UnlockZone(zoneIndex);
    }
}