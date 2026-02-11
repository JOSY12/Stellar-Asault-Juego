using UnityEngine;

public class EnvironmentLayer : MonoBehaviour
{
    [Header("Settings")]
    public ZoneData.LayerType layerType;
    public float parallaxSpeed = 0.5f;
    public Transform player;
    
    private Vector3 lastPlayerPosition;
    private Material layerMaterial;
    private GameObject[] tiles;
    
    // Para Nebula
    private int tilesX = 5;
    private int tilesY = 5;
    private float tileWidth;
    private float tileHeight;
    private Camera mainCamera;
    
    // Para Stars
    private ParticleSystem starSystem;
    private ParticleSystem.Particle[] particles;
    
    // Para Planets
    private GameObject[] planets;
    
    public void Initialize(ZoneData.EnvironmentLayer config, Transform playerTransform, Material nebulaMat = null)
    {
        layerType = config.type;
        parallaxSpeed = config.parallaxSpeed;
        player = playerTransform;
        mainCamera = Camera.main;
        
        switch (layerType)
        {
            case ZoneData.LayerType.Nebula:
                InitializeNebula(config, nebulaMat);
                break;
            case ZoneData.LayerType.Stars:
                InitializeStars(config);
                break;
            case ZoneData.LayerType.Planets:
                InitializePlanets(config);
                break;
        }
        
        if (player != null)
            lastPlayerPosition = player.position;
    }
    
    void InitializeNebula(ZoneData.EnvironmentLayer config, Material nebulaMat)
    {
        if (nebulaMat == null || mainCamera == null) return;
        
        tileHeight = mainCamera.orthographicSize * 2f;
        tileWidth = tileHeight * mainCamera.aspect;
        
        layerMaterial = new Material(nebulaMat);
        layerMaterial.SetColor("_MainColor", config.nebulaColor);
        layerMaterial.SetColor("_SecondaryColor", config.nebulaColor * 0.7f);
        layerMaterial.SetFloat("_Scale", config.nebulaScale);
        layerMaterial.SetFloat("_Speed", config.nebulaAnimationSpeed);
        layerMaterial.SetFloat("_Alpha", config.nebulaColor.a);
        layerMaterial.SetFloat("_Pixelation", config.nebulaPixelation);
        layerMaterial.SetFloat("_Contrast", config.nebulaContrast);
        
        tiles = new GameObject[tilesX * tilesY];
        int index = 0;
        
        for (int x = 0; x < tilesX; x++)
        {
            for (int y = 0; y < tilesY; y++)
            {
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.parent = transform;
                
                float offsetX = (x - tilesX / 2) * tileWidth;
                float offsetY = (y - tilesY / 2) * tileHeight;
                tile.transform.position = new Vector3(
                    player.position.x + offsetX,
                    player.position.y + offsetY,
                    transform.position.z
                );
                
                tile.transform.localScale = new Vector3(tileWidth, tileHeight, 1f);
                
                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                renderer.material = layerMaterial;
                renderer.sortingOrder = Mathf.RoundToInt(transform.position.z * -10);
                
                Destroy(tile.GetComponent<Collider>());
                
                tiles[index] = tile;
                index++;
            }
        }
        
        Debug.Log($"[{gameObject.name}] Nebula created: {tilesX}x{tilesY} tiles");
    }
    
    void InitializeStars(ZoneData.EnvironmentLayer config)
    {
        starSystem = gameObject.AddComponent<ParticleSystem>();
        
        var main = starSystem.main;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(config.minStarSize, config.maxStarSize);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = config.starCount;
        main.startColor = config.starColor;
        
        var emission = starSystem.emission;
        emission.rateOverTime = 0;
        
        var shape = starSystem.shape;
        shape.enabled = false;
        
        particles = new ParticleSystem.Particle[config.starCount];
        float spawnRadius = 15f;
        
        for (int i = 0; i < config.starCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            particles[i].position = new Vector3(
                player.position.x + randomOffset.x,
                player.position.y + randomOffset.y,
                0
            );
            particles[i].startLifetime = Mathf.Infinity;
            particles[i].remainingLifetime = Mathf.Infinity;
            particles[i].startSize = Random.Range(config.minStarSize, config.maxStarSize);
            particles[i].startColor = config.starColor;
            particles[i].velocity = Vector3.zero;
        }
        
        starSystem.SetParticles(particles, config.starCount);
        
        Debug.Log($"[{gameObject.name}] Stars created: {config.starCount}");
    }
    
    void InitializePlanets(ZoneData.EnvironmentLayer config)
    {
        planets = new GameObject[config.planetCount];
        float spawnRadius = 20f;
        
        for (int i = 0; i < config.planetCount; i++)
        {
            GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.name = $"Planet_{i}";
            planet.transform.parent = transform;
            
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            planet.transform.position = new Vector3(
                player.position.x + randomOffset.x,
                player.position.y + randomOffset.y,
                transform.position.z
            );
            
            float size = Random.Range(config.minPlanetSize, config.maxPlanetSize);
            planet.transform.localScale = Vector3.one * size;
            
            MeshRenderer renderer = planet.GetComponent<MeshRenderer>();
            Color planetColor = config.planetColors[Random.Range(0, config.planetColors.Length)];
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.material.color = planetColor;
            renderer.sortingOrder = Mathf.RoundToInt(transform.position.z * -10);
            
            Destroy(planet.GetComponent<Collider>());
            
            planets[i] = planet;
        }
        
        Debug.Log($"[{gameObject.name}] Planets created: {config.planetCount}");
    }
    
    void LateUpdate()
    {
        if (player == null) return;
        
        Vector3 playerDelta = player.position - lastPlayerPosition;
        
        if (playerDelta.sqrMagnitude > 0.0001f)
        {
            Vector3 parallaxOffset = playerDelta * (1f - parallaxSpeed);
            transform.position -= parallaxOffset;
        }
        
        if (layerType == ZoneData.LayerType.Nebula && tiles != null)
        {
            RepositionNebulaTiles();
        }
        
        lastPlayerPosition = player.position;
    }
    
    void RepositionNebulaTiles()
    {
        int index = 0;
        for (int x = 0; x < tilesX; x++)
        {
            for (int y = 0; y < tilesY; y++)
            {
                GameObject tile = tiles[index];
                if (tile == null)
                {
                    index++;
                    continue;
                }
                
                Vector3 tilePos = tile.transform.position;
                Vector3 playerPos = player.position;
                
                float distX = tilePos.x - playerPos.x;
                float distY = tilePos.y - playerPos.y;
                
                if (Mathf.Abs(distX) > tileWidth * (tilesX / 2 + 0.5f))
                {
                    float direction = Mathf.Sign(distX);
                    tilePos.x -= direction * tileWidth * tilesX;
                }
                
                if (Mathf.Abs(distY) > tileHeight * (tilesY / 2 + 0.5f))
                {
                    float direction = Mathf.Sign(distY);
                    tilePos.y -= direction * tileHeight * tilesY;
                }
                
                tile.transform.position = tilePos;
                index++;
            }
        }
    }
}