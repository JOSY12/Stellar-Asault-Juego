using UnityEngine;

[CreateAssetMenu(fileName = "Zone_New", menuName = "Space Shooter/Zone Data")]
public class ZoneData : ScriptableObject
{
    [Header("Zone Info")]
    public string zoneName = "New Zone";
    public int zoneIndex = 0;
    
    [Header("Economy")]
    public float scrapMultiplier = 1f;
    public int unlockCost = 0; // 0 = gratis (Zone_0)
    
    [Header("Visual Preview")]
    public Color ambientColor = new Color(0f, 0f, 0.04f, 1f);
    
    [Header("Environment Layers")]
    public EnvironmentLayerConfig[] layers = new EnvironmentLayerConfig[3];
    
    [System.Serializable]
    public class EnvironmentLayerConfig
    {
        [Header("Layer Type")]
        public LayerType type = LayerType.Stars;
        public string layerName = "Layer";
        public float depth = 0f;
        
        [Header("Common Settings")]
        public float parallaxSpeed = 0.3f;
        public int sortingOrder = -10;
        
        // NEBULA
        [Header("Nebula Settings")]
        public Color nebulaColor = new Color(0.1f, 0.3f, 0.8f, 0.4f);
        public int nebulaCount = 40;
        public float pulseSpeed = 0.5f;
        public float minAlpha = 0.1f;
        public float maxAlpha = 0.4f;
        public float scaleVariation = 2f;
        public float noiseScale = 3.5f;
        public float nebulaSpawnRadius = 50f;
        public bool enableDrift = true;
        public float driftSpeed = 0.5f;
        public float driftScale = 2f;
        
        // STARS
        [Header("Stars Settings")]
        public int starCount = 300;
        public float starSpawnRadius = 15f;
        public float minStarSize = 0.05f;
        public float maxStarSize = 0.2f;
        public bool enableTwinkle = true;
        public float twinkleSpeed = 1f;
        public Color starColor = Color.white;
        
        // PLANETS
        [Header("Planets Settings")]
        public Sprite[] planetSprites;
        public int planetCount = 5;
        public float planetSpawnRadius = 30f;
        public float minPlanetSize = 3f;
        public float maxPlanetSize = 8f;
        public Color planetTint = Color.white;
        public bool rotatePlanets = true;
        public float minRotationSpeed = 5f;
        public float maxRotationSpeed = 20f;
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