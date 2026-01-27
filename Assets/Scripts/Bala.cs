using UnityEngine;

public class Bala : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 1;
    void Start()
    {
        Destroy(gameObject, lifeTime); // Se destruye sola para no llenar la memoria
    }

    void Update()
    {
        
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
  void OnTriggerEnter2D(Collider2D collision)
{
    // Esto imprimirá en consola el nombre de CUALQUIER cosa que toque la bala
    Debug.Log("La bala chocó con: " + collision.gameObject.name + " con el Tag: " + collision.tag);

    if (collision.CompareTag("Enemy"))
    {
        Debug.Log("¡Enemigo detectado! Aplicando daño...");
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
}