using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    
    [Header("Planet Settings")]
    public Sprite[] planetSprites; // Array de sprites de planetas
    public int planetCount = 5;
    public float spawnRadius = 30f;
    public float minPlanetSize = 3f;
    public float maxPlanetSize = 8f;
    
    [Header("Parallax")]
    public float parallaxSpeed = 0.4f; // 0 = no se mueve, 1 = se mueve con player
    
    [Header("Appearance")]
    public Color planetTint = Color.white;
    public int sortingOrder = -15;
    
    [Header("Rotation")]
    public bool rotatePlanets = true;
    public float minRotationSpeed = 5f;
    public float maxRotationSpeed = 20f;
    
    private GameObject[] planets;
    private Vector3 lastPlayerPosition;
    
    void Start()
    {
        // Encontrar player
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                lastPlayerPosition = playerTransform.position;
            }
        }
        
        SpawnPlanets();
    }
    
    void SpawnPlanets()
    {
        if (planetSprites == null || planetSprites.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No planet sprites assigned!");
            return;
        }
        
        planets = new GameObject[planetCount];
        
        for (int i = 0; i < planetCount; i++)
        {
            // Crear GameObject
            GameObject planet = new GameObject($"Planet_{i}");
            planet.transform.parent = transform;
            
            // Posición aleatoria alrededor del player
            Vector3 randomPos = GetRandomPositionAroundPlayer();
            planet.transform.position = randomPos;
            
            // Sprite random
            Sprite randomSprite = planetSprites[Random.Range(0, planetSprites.Length)];
            
            // SpriteRenderer
            SpriteRenderer sr = planet.AddComponent<SpriteRenderer>();
            sr.sprite = randomSprite;
            sr.color = planetTint;
            sr.sortingOrder = sortingOrder;
            
            // Tamaño aleatorio
            float size = Random.Range(minPlanetSize, maxPlanetSize);
            planet.transform.localScale = Vector3.one * size;
            
            // Rotación
            if (rotatePlanets)
            {
                PlanetRotator rotator = planet.AddComponent<PlanetRotator>();
                rotator.rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            }
            
            planets[i] = planet;
        }
        
        Debug.Log($"[{gameObject.name}] Spawned {planetCount} planets");
    }
    
    void LateUpdate()
    {
        if (playerTransform == null || planets == null) return;
        
        // Calcular movimiento del player
        Vector3 playerDelta = playerTransform.position - lastPlayerPosition;
        
        // Aplicar parallax a todos los planetas
        Vector3 parallaxOffset = -playerDelta * (1f - parallaxSpeed);
        
        foreach (GameObject planet in planets)
        {
            if (planet != null)
            {
                planet.transform.position += parallaxOffset;
                
                // Reposicionar si está muy lejos
                float distance = Vector3.Distance(planet.transform.position, playerTransform.position);
                
                if (distance > spawnRadius * 2f)
                {
                    planet.transform.position = GetRandomPositionAroundPlayer();
                }
            }
        }
        
        lastPlayerPosition = playerTransform.position;
    }
    
    Vector3 GetRandomPositionAroundPlayer()
    {
        if (playerTransform == null)
            return Random.insideUnitCircle * spawnRadius;
        
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            playerTransform.position.x + randomOffset.x,
            playerTransform.position.y + randomOffset.y,
            transform.position.z
        );
    }
    
    public void SetPlanetTint(Color color)
    {
        planetTint = color;
        
        if (planets != null)
        {
            foreach (GameObject planet in planets)
            {
                if (planet != null)
                {
                    SpriteRenderer sr = planet.GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.color = color;
                }
            }
        }
    }
}

// Componente auxiliar para rotar planetas
public class PlanetRotator : MonoBehaviour
{
    public float rotationSpeed = 10f;
    
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}