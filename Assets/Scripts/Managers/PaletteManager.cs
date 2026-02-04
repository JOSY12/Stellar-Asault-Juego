using UnityEngine;

public class PaletteManager : MonoBehaviour
{
    public static PaletteManager Instance;
    
    public PaletteData[] allPalettes; // 6 paletas
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Desbloquear paletas gratis por defecto
            UnlockFreePalettes();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void UnlockFreePalettes()
    {
        if (allPalettes == null) return;
        
        for (int i = 0; i < allPalettes.Length; i++)
        {
            if (allPalettes[i] != null && allPalettes[i].isFree)
            {
                if (SaveManager.Instance != null)
                    SaveManager.Instance.UnlockPalette(i);
            }
        }
    }
    
    public PaletteData GetCurrentPalette()
{
    if (allPalettes == null || allPalettes.Length == 0)
    {
        Debug.LogWarning("No palettes assigned!");
        return null;
    }
    
    int index = 0; // Default siempre es 0
    if (SaveManager.Instance != null)
        index = SaveManager.Instance.GetCurrentPalette();
    
    // Asegurar que el índice sea válido
    if (index < 0 || index >= allPalettes.Length)
    {
        Debug.LogWarning($"Invalid palette index {index}, using default (0)");
        index = 0;
    }
    
    return allPalettes[index];
}
    
    public void ApplyCurrentPalette()
    {
        PaletteData palette = GetCurrentPalette();
        if (palette == null) return;
        
        // Aplicar color de fondo
        if (Camera.main != null)
            Camera.main.backgroundColor = palette.backgroundColor;
        
        // Aplicar al jugador
        ApplyToPlayer(palette);
        
        // Aplicar a enemigos existentes
        ApplyToEnemies(palette);
        
        Debug.Log($"Palette applied: {palette.paletteName}");
    }
    
    void ApplyToPlayer(PaletteData palette)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = palette.playerColor;
        }
    }
    
    void ApplyToEnemies(PaletteData palette)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            ApplyColorToEnemy(enemy, palette);
        }
    }
    
    public void ApplyColorToEnemy(GameObject enemyObj, PaletteData palette)
    {
        if (enemyObj == null || palette == null) return;
        
        SpriteRenderer sr = enemyObj.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        
        // Determinar tipo de enemigo por componente
        string enemyType = GetEnemyType(enemyObj);
        Color color = palette.GetEnemyColor(enemyType);
        
        sr.color = color;
    }
    
    string GetEnemyType(GameObject enemyObj)
    {
        if (enemyObj.GetComponent<EnemyScout>() != null) return "Scout";
        if (enemyObj.GetComponent<EnemyGrunt>() != null) return "Grunt";
        if (enemyObj.GetComponent<EnemyKamikaze>() != null) return "Kamikaze";
        if (enemyObj.GetComponent<EnemyTank>() != null) return "Tank";
        if (enemyObj.GetComponent<EnemySniper>() != null) return "Sniper";
        if (enemyObj.GetComponent<EnemySplitter>() != null) return "Splitter";
        if (enemyObj.GetComponent<EnemyZigzag>() != null) return "Zigzag";
        if (enemyObj.GetComponent<EnemyBoss>() != null) return "Boss";
        
        return "Scout"; // Default
    }
    
    public bool IsPaletteUnlocked(int index)
    {
        if (index < 0 || index >= allPalettes.Length) return false;
        if (allPalettes[index] == null) return false;
        
        // Si es gratis, está desbloqueada
        if (allPalettes[index].isFree) return true;
        
        // Verificar en SaveManager
        if (SaveManager.Instance != null)
            return SaveManager.Instance.IsPaletteUnlocked(index);
        
        return false;
    }
    
    public void SetPalette(int index)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetCurrentPalette(index);
        }
        
        ApplyCurrentPalette();
    }
}