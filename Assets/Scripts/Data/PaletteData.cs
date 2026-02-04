using UnityEngine;

[CreateAssetMenu(fileName = "NewPalette", menuName = "Game/Palette")]
public class PaletteData : ScriptableObject
{
    [Header("Palette Info")]
    public string paletteName = "New Palette";
    
    [Header("Unlock Requirements")]
    public bool isFree = true;
    public int adCostToUnlock = 0; // Cuántos ads para desbloquear
    
    [Header("Environment Colors")]
    public Color backgroundColor = Color.black;
    
    [Header("Player Colors")]
    public Color playerColor = Color.cyan;
    public Color playerBulletColor = Color.white;
    
    [Header("Enemy Colors")]
    public Color enemyScoutColor = Color.green;
    public Color enemyGruntColor = Color.yellow;
    public Color enemyKamikazeColor = Color.red;
    public Color enemyTankColor = Color.gray;
    public Color enemySniperColor = new Color(0.7f, 0.4f, 1f); // Púrpura
    public Color enemySplitterColor = new Color(1f, 0.6f, 0f); // Naranja
    public Color enemyZigzagColor = Color.cyan;
    public Color enemyBossColor = new Color(0.8f, 0.1f, 0.1f); // Rojo oscuro
    
    [Header("UI Colors (Optional)")]
    public Color uiAccentColor = Color.cyan;
    public Color uiTextColor = Color.white;
    
    public Color GetEnemyColor(string enemyType)
    {
        switch (enemyType)
        {
            case "Scout": return enemyScoutColor;
            case "Grunt": return enemyGruntColor;
            case "Kamikaze": return enemyKamikazeColor;
            case "Tank": return enemyTankColor;
            case "Sniper": return enemySniperColor;
            case "Splitter": return enemySplitterColor;
            case "Zigzag": return enemyZigzagColor;
            case "Boss": return enemyBossColor;
            default: return Color.white;
        }
    }
}