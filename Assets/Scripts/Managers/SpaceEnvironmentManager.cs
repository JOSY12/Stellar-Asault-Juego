using UnityEngine;
using System.Collections.Generic;

public class SpaceEnvironmentManager : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;
    
    [Header("Load from Zone Data (Optional)")]
    public bool loadFromZoneData = true;
    
    [Header("Manual Layers (if not using Zone Data)")]
    public List<EnvironmentLayer> manualLayers = new List<EnvironmentLayer>();
    
    [Header("Planet Sprites (Global)")]
    public Sprite[] globalPlanetSprites;
    
    private List<EnvironmentLayer> activeLayers = new List<EnvironmentLayer>();
    
    [System.Serializable]
    public class EnvironmentLayer
    {
        [Header("Layer Type")]
        public LayerType type = LayerType.Stars;
        public string layerName = "Layer";
        public float depth = 0f;
        [Header("Nebula Visuals")]
[Range(16, 256)] public int nebulaResolution = 128; // Controla el pixelado
        [Header("Common Settings")]
        public float parallaxSpeed = 0.3f;
        public int sortingOrder = -10;
        
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
        
        [Header("Stars Settings")]
        public int starCount = 300;
        public float starSpawnRadius = 15f;
        public float minStarSize = 0.05f;
        public float maxStarSize = 0.2f;
        public bool enableTwinkle = true;
        public float twinkleSpeed = 1f;
        public Color starColor = Color.white;
        
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
        
        // Runtime
        [HideInInspector] public ParticleSystem particleSystem;
        [HideInInspector] public ParticleSystem.Particle[] particles;
        [HideInInspector] public GameObject[] planetObjects;
        [HideInInspector] public float[] planetRotationSpeeds;
        [HideInInspector] public float[] starTwinkleTimers; // NUEVO
        [HideInInspector] public Vector3 lastPlayerPosition;
        [HideInInspector] public float timer;
        [HideInInspector] public Vector3 driftOffset;
    }
    
    public enum LayerType
    {
        Nebula,
        Stars,
        Planets
    }
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("[SpaceEnvironment] Player not found!");
                return;
            }
        }
        
        if (loadFromZoneData && ZoneManager.Instance != null)
        {
            LoadFromZoneData();
        }
        else
        {
            activeLayers = manualLayers;
        }
        
        foreach (var layer in activeLayers)
        {
            layer.lastPlayerPosition = player.position;
            
            if (layer.type == LayerType.Planets && (layer.planetSprites == null || layer.planetSprites.Length == 0))
            {
                layer.planetSprites = globalPlanetSprites;
            }
            
            switch (layer.type)
            {
                case LayerType.Nebula:
                    SetupNebulaLayer(layer);
                    break;
                case LayerType.Stars:
                    SetupStarsLayer(layer);
                    break;
                case LayerType.Planets:
                    SetupPlanetsLayer(layer);
                    break;
            }
        }
        
        Debug.Log($"[SpaceEnvironment] {activeLayers.Count} layers initialized");
    }
    
    void LoadFromZoneData()
    {
        ZoneData currentZone = ZoneManager.Instance.GetCurrentZone();
        
        if (currentZone == null)
        {
            Debug.LogError("[SpaceEnvironment] No current zone!");
            activeLayers = manualLayers;
            return;
        }
        
        if (Camera.main != null)
            Camera.main.backgroundColor = currentZone.ambientColor;
        
        activeLayers.Clear();
        
        foreach (var zoneLayer in currentZone.layers)
        {
            var envLayer = new EnvironmentLayer
            {
                type = (LayerType)zoneLayer.type,
                layerName = zoneLayer.layerName,
                depth = zoneLayer.depth,
                parallaxSpeed = zoneLayer.parallaxSpeed,
                sortingOrder = zoneLayer.sortingOrder,
                
                nebulaColor = zoneLayer.nebulaColor,
                nebulaCount = zoneLayer.nebulaCount,
                pulseSpeed = zoneLayer.pulseSpeed,
                minAlpha = zoneLayer.minAlpha,
                maxAlpha = zoneLayer.maxAlpha,
                scaleVariation = zoneLayer.scaleVariation,
                noiseScale = zoneLayer.noiseScale,
                nebulaSpawnRadius = zoneLayer.nebulaSpawnRadius,
                enableDrift = zoneLayer.enableDrift,
                driftSpeed = zoneLayer.driftSpeed,
                driftScale = zoneLayer.driftScale,
                
                starCount = zoneLayer.starCount,
                starSpawnRadius = zoneLayer.starSpawnRadius,
                minStarSize = zoneLayer.minStarSize,
                maxStarSize = zoneLayer.maxStarSize,
                enableTwinkle = zoneLayer.enableTwinkle,
                twinkleSpeed = zoneLayer.twinkleSpeed,
                starColor = zoneLayer.starColor,
                
                planetSprites = zoneLayer.planetSprites,
                planetCount = zoneLayer.planetCount,
                planetSpawnRadius = zoneLayer.planetSpawnRadius,
                minPlanetSize = zoneLayer.minPlanetSize,
                maxPlanetSize = zoneLayer.maxPlanetSize,
                planetTint = zoneLayer.planetTint,
                rotatePlanets = zoneLayer.rotatePlanets,
                minRotationSpeed = zoneLayer.minRotationSpeed,
                maxRotationSpeed = zoneLayer.maxRotationSpeed
            };
            
            activeLayers.Add(envLayer);
        }
        
        Debug.Log($"[SpaceEnvironment] Loaded {activeLayers.Count} layers from zone: {currentZone.zoneName}");
    }
    
    void SetupNebulaLayer(EnvironmentLayer layer)
    {
        GameObject layerObj = new GameObject($"Nebula_{layer.layerName}");
        layerObj.transform.parent = transform;
        layerObj.transform.position = new Vector3(0, 0, layer.depth);
        
        layer.particleSystem = layerObj.AddComponent<ParticleSystem>();
        
        var main = layer.particleSystem.main;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = layer.nebulaCount;
        
        Shader targetShader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
        if (targetShader == null) targetShader = Shader.Find("Sprites/Default");
        
        var renderer = layer.particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(targetShader);
        renderer.material.mainTexture = CreateOrganicNebulaTexture(layer);
        renderer.sortingOrder = layer.sortingOrder;
        
        layer.particles = new ParticleSystem.Particle[layer.nebulaCount];
        Vector3 startPos = player.position;
        
        for (int i = 0; i < layer.nebulaCount; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * layer.nebulaSpawnRadius;
            layer.particles[i].position = new Vector3(
                startPos.x + randomPos.x,
                startPos.y + randomPos.y,
                layer.depth
            );
            
            layer.particles[i].startSize = Random.Range(25f, 45f);
            layer.particles[i].rotation = Random.Range(0f, 360f);
            layer.particles[i].startColor = layer.nebulaColor;
            layer.particles[i].startLifetime = Mathf.Infinity;
            layer.particles[i].remainingLifetime = Mathf.Infinity;
        }
        
        layer.particleSystem.SetParticles(layer.particles, layer.nebulaCount);
        
        Debug.Log($"[Nebula] {layer.layerName}: {layer.nebulaCount} particles");
    }
    
   Texture2D CreateOrganicNebulaTexture(EnvironmentLayer layer)
{
    int res = layer.nebulaResolution; // Ahora usa la variable
    Texture2D tex = new Texture2D(res, res);
    tex.filterMode = FilterMode.Point; // Importante para que se vea pixelado y no borroso
    Color[] colors = new Color[res * res];
    float seed = Random.Range(0f, 100f);
    
    for (int y = 0; y < res; y++)
    {
        for (int x = 0; x < res; x++)
        {
            float u = (float)x / res;
            float v = (float)y / res;
            float dist = Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f));
            float noise = Mathf.PerlinNoise(u * layer.noiseScale + seed, v * layer.noiseScale + seed);
            float alpha = Mathf.Clamp01((0.5f - dist) * 2.0f) * noise;
            colors[y * res + x] = new Color(1, 1, 1, alpha);
        }
    }
    tex.SetPixels(colors);
    tex.Apply();
    return tex;
}
    
    void SetupStarsLayer(EnvironmentLayer layer)
{
    GameObject layerObj = new GameObject($"Stars_{layer.layerName}");
    layerObj.transform.parent = transform;
    layerObj.transform.position = new Vector3(0, 0, layer.depth);
    
    layer.particleSystem = layerObj.AddComponent<ParticleSystem>();
    
    // 1. Configuración Básica
    var main = layer.particleSystem.main;
    main.startLifetime = Mathf.Infinity;
    main.startSpeed = 0f;
    main.simulationSpace = ParticleSystemSimulationSpace.World;
    main.maxParticles = layer.starCount;
    // Importante: No dejar que el sistema emita solo, nosotros ponemos las partículas
    var emission = layer.particleSystem.emission;
    emission.enabled = false; 

    // 2. Configuración del Renderer (El Material)
    var renderer = layer.particleSystem.GetComponent<ParticleSystemRenderer>();
    renderer.renderMode = ParticleSystemRenderMode.Billboard;
    
    // Usamos Sprites/Default que es el más confiable para 2D/UI
    Material starMaterial = new Material(Shader.Find("Sprites/Default"));
    renderer.material = starMaterial;
    renderer.sortingOrder = layer.sortingOrder;
    
    // 3. Crear las partículas físicamente
    layer.particles = new ParticleSystem.Particle[layer.starCount];
    layer.starTwinkleTimers = new float[layer.starCount];
    
    for (int i = 0; i < layer.starCount; i++)
    {
        // Usamos la función GetRandomPosition que evita el centro (que no te salgan en la cara)
        layer.particles[i].position = GetRandomPosition(player.position, layer.starSpawnRadius, layer.depth);
        layer.particles[i].startLifetime = Mathf.Infinity;
        layer.particles[i].remainingLifetime = Mathf.Infinity;
        layer.particles[i].startSize = Random.Range(layer.minStarSize, layer.maxStarSize);
        
        // Color inicial (asegúrate que el Alpha sea 1 al inicio)
        Color c = layer.starColor;
        c.a = 1f;
        layer.particles[i].startColor = c;
        
        layer.starTwinkleTimers[i] = Random.Range(0f, Mathf.PI * 2f);
    }
    
    // 4. Aplicar partículas y forzar el dibujado
    layer.particleSystem.SetParticles(layer.particles, layer.starCount);
}
    
    void SetupPlanetsLayer(EnvironmentLayer layer)
    {
        if (layer.planetSprites == null || layer.planetSprites.Length == 0)
        {
            Debug.LogWarning($"[Planets] {layer.layerName}: No sprites!");
            return;
        }
        
        layer.planetObjects = new GameObject[layer.planetCount];
        layer.planetRotationSpeeds = new float[layer.planetCount];
        
        for (int i = 0; i < layer.planetCount; i++)
        {
            GameObject planet = new GameObject($"Planet_{layer.layerName}_{i}");
            planet.transform.parent = transform;
            planet.transform.position = GetRandomPosition(player.position, layer.planetSpawnRadius, layer.depth);
            
            SpriteRenderer sr = planet.AddComponent<SpriteRenderer>();
            sr.sprite = layer.planetSprites[Random.Range(0, layer.planetSprites.Length)];
            sr.color = layer.planetTint;
            sr.sortingOrder = layer.sortingOrder;
            
            float size = Random.Range(layer.minPlanetSize, layer.maxPlanetSize);
            planet.transform.localScale = Vector3.one * size;
            
            layer.planetRotationSpeeds[i] = Random.Range(layer.minRotationSpeed, layer.maxRotationSpeed);
            layer.planetObjects[i] = planet;
        }
        
        Debug.Log($"[Planets] {layer.layerName}: {layer.planetCount} planets");
    }
    
    void LateUpdate()
    {
        if (player == null) return;
        
        foreach (var layer in activeLayers)
        {
            switch (layer.type)
            {
                case LayerType.Nebula:
                    UpdateNebulaLayer(layer);
                    break;
                case LayerType.Stars:
                    UpdateStarsLayer(layer);
                    break;
                case LayerType.Planets:
                    UpdatePlanetsLayer(layer);
                    break;
            }
        }
    }
    
    void UpdateNebulaLayer(EnvironmentLayer layer)
    {
        if (layer.particleSystem == null || layer.particles == null) return;
        
        layer.timer += Time.deltaTime * layer.pulseSpeed;
        float currentAlpha = Mathf.Lerp(layer.minAlpha, layer.maxAlpha, (Mathf.Sin(layer.timer) + 1f) / 2f);
        
        Vector3 playerDelta = player.position - layer.lastPlayerPosition;
        Vector3 parallaxMovement = -playerDelta * (1f - layer.parallaxSpeed);
        
        int numParticles = layer.particleSystem.GetParticles(layer.particles);
        
        for (int i = 0; i < numParticles; i++)
        {
            layer.particles[i].position += parallaxMovement;
            
            if (layer.enableDrift)
            {
                Vector3 drift = new Vector3(
                    Mathf.PerlinNoise(i * 0.1f, Time.time * layer.driftSpeed) - 0.5f,
                    Mathf.PerlinNoise(i * 0.1f + 100f, Time.time * layer.driftSpeed) - 0.5f,
                    0
                ) * layer.driftSpeed * Time.deltaTime;
                
                layer.particles[i].position += drift;
            }
            
            Color c = layer.nebulaColor;
            c.a = currentAlpha;
            layer.particles[i].startColor = c;
            
            float dist = Vector3.Distance(layer.particles[i].position, player.position);
            if (dist > layer.nebulaSpawnRadius)
            {
                Vector3 dir = (layer.particles[i].position - player.position).normalized;
                layer.particles[i].position = player.position - (dir * (layer.nebulaSpawnRadius - 5f));
            }
        }
        
        layer.particleSystem.SetParticles(layer.particles, numParticles);
        layer.lastPlayerPosition = player.position;
    }
   void UpdateStarsLayer(EnvironmentLayer layer)
{
    if (layer.particleSystem == null || layer.particles == null) return;
    
    int numParticles = layer.particleSystem.GetParticles(layer.particles);
    
    for (int i = 0; i < numParticles; i++)
    {
        // 1. Reposicionar si se alejan demasiado
        float distance = Vector3.Distance(layer.particles[i].position, player.position);
        if (distance > layer.starSpawnRadius * 1.5f)
        {
            layer.particles[i].position = GetRandomPosition(player.position, layer.starSpawnRadius, layer.depth);
        }
        
        // 2. Parpadeo (Twinkle)
        if (layer.enableTwinkle)
        {
            layer.starTwinkleTimers[i] += Time.deltaTime * layer.twinkleSpeed;
            // Usamos un seno para el brillo, con un desfase aleatorio por estrella
            float lerp = (Mathf.Sin(layer.starTwinkleTimers[i]) + 1f) / 2f;
            float alpha = Mathf.Lerp(0.1f, 1f, lerp); 
            
            Color c = layer.starColor;
            c.a = alpha;
            layer.particles[i].startColor = c;
        }
    }
    layer.particleSystem.SetParticles(layer.particles, numParticles);
}
    
    void UpdatePlanetsLayer(EnvironmentLayer layer)
    {
        if (layer.planetObjects == null) return;
        
        Vector3 playerDelta = player.position - layer.lastPlayerPosition;
        Vector3 parallaxOffset = -playerDelta * (1f - layer.parallaxSpeed);
        
        for (int i = 0; i < layer.planetObjects.Length; i++)
        {
            if (layer.planetObjects[i] == null) continue;
            
            layer.planetObjects[i].transform.position += parallaxOffset;
            
            if (layer.rotatePlanets)
            {
                layer.planetObjects[i].transform.Rotate(0, 0, layer.planetRotationSpeeds[i] * Time.deltaTime);
            }
            
            float distance = Vector3.Distance(layer.planetObjects[i].transform.position, player.position);
            
            if (distance > layer.planetSpawnRadius * 2f)
            {
                layer.planetObjects[i].transform.position = GetRandomPosition(player.position, layer.planetSpawnRadius, layer.depth);
            }
        }
        
        layer.lastPlayerPosition = player.position;
    }
    
   Vector3 GetRandomPosition(Vector3 center, float radius, float depth)
{
    // Genera un punto en un anillo entre el 40% y el 100% del radio
    // Esto evita que spawneen "en la cara"
    float minRadius = radius * 0.4f; 
    float randomRadius = Random.Range(minRadius, radius);
    float randomAngle = Random.Range(0f, Mathf.PI * 2f);
    
    return new Vector3(
        center.x + Mathf.Cos(randomAngle) * randomRadius,
        center.y + Mathf.Sin(randomAngle) * randomRadius,
        depth
    );
}
}