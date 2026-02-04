using UnityEngine;

[CreateAssetMenu(fileName = "NewBullet", menuName = "Game/Bullet Data")]
public class BulletData : ScriptableObject
{
    [Header("Visual")]
    public Sprite bulletSprite;
    public Color bulletColor = Color.white;
    public Vector2 bulletScale = Vector2.one;
    
    [Header("Trail Effect (Optional)")]
    public bool hasTrail = false;
    public Color trailColor = Color.white;
    public float trailTime = 0.3f;
    public float trailWidth = 0.1f;
}