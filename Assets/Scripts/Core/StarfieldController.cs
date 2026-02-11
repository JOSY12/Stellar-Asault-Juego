using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class StarfieldController : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    
    [Header("Star Settings")]
    public int starCount = 300;
    public float spawnRadius = 15f; // Radio alrededor del player
    public float minStarSize = 0.05f;
    public float maxStarSize = 0.2f;
    
    [Header("Twinkle")]
    public bool enableTwinkle = true;
    public float twinkleSpeed = 1f;
    
    [Header("Color")]
    public Color starColor = Color.white;
    
    private ParticleSystem starSystem;
    private ParticleSystem.Particle[] particles;
    
    void Start()
    {
        // Encontrar player
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Debug.Log("[Starfield] Player found");
            }
            else
            {
                Debug.LogError("[Starfield] Player not found!");
                enabled = false;
                return;
            }
        }
        
        starSystem = GetComponent<ParticleSystem>();
        SetupStarfield();
    }
    
    void SetupStarfield()
    {
        var main = starSystem.main;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(minStarSize, maxStarSize);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = starCount;
        main.startColor = starColor;
        
        // Sin emisiÃ³n automÃ¡tica
        var emission = starSystem.emission;
        emission.rateOverTime = 0;
        
        // Sin shape (las creamos manualmente)
        var shape = starSystem.shape;
        shape.enabled = false;
        
        // Parpadeo
        if (enableTwinkle)
        {
            var sizeOverLifetime = starSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(0.5f, 0.6f);
            curve.AddKey(1f, 1f);
            curve.preWrapMode = WrapMode.Loop;
            curve.postWrapMode = WrapMode.Loop;
            
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }
        
        // Crear partÃ­culas iniciales
        particles = new ParticleSystem.Particle[starCount];
        
        for (int i = 0; i < starCount; i++)
        {
            particles[i].position = GetRandomPositionAroundPlayer();
            particles[i].startLifetime = Mathf.Infinity;
            particles[i].remainingLifetime = Mathf.Infinity;
            particles[i].startSize = Random.Range(minStarSize, maxStarSize);
            particles[i].startColor = starColor;
            particles[i].velocity = Vector3.zero;
        }
        
        starSystem.SetParticles(particles, starCount);
        
        Debug.Log($"Starfield created: {starCount} stars");
    }
    
    void LateUpdate()
    {
        if (playerTransform == null || starSystem == null) return;
        
        // Las estrellas SIGUEN al player (se mueven con Ã©l)
        // No necesitamos hacer nada extra, el World Space las mantiene fijas
        // PERO podrÃ­amos reposicionar las que quedan muy lejos
        
        int numParticles = starSystem.GetParticles(particles);
        
        for (int i = 0; i < numParticles; i++)
        {
            // Si una estrella estÃ¡ muy lejos del player, reposicionarla
            float distance = Vector3.Distance(particles[i].position, playerTransform.position);
            
            if (distance > spawnRadius * 2f)
            {
                particles[i].position = GetRandomPositionAroundPlayer();
            }
        }
        
        starSystem.SetParticles(particles, numParticles);
    }
    
    Vector3 GetRandomPositionAroundPlayer()
    {
        if (playerTransform == null)
            return Random.insideUnitCircle * spawnRadius;
        
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            playerTransform.position.x + randomOffset.x,
            playerTransform.position.y + randomOffset.y,
            0
        );
    }
    
    public void SetStarCount(int count)
    {
        starCount = count;
        SetupStarfield();
    }
    
    public void SetStarColor(Color color)
    {
        starColor = color;
        var main = starSystem.main;
        main.startColor = color;
    }
}