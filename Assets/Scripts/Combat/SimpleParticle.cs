using UnityEngine;

public class SimpleParticle : MonoBehaviour
{
    public float lifeTime = 1f;
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
 