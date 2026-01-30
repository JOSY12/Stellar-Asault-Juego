using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Pool de números flotantes para mostrar daño/coins
/// Usa object pooling para mejor performance
/// </summary>
public class FloatingNumberPool : MonoBehaviour
{
    public static FloatingNumberPool Instance { get; private set; }

    [Header("Prefab")]
    public GameObject floatingNumberPrefab;
    
    [Header("Pool Settings")]
    public int poolSize = 20;
    
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Pre-crear objetos del pool
        if (floatingNumberPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(floatingNumberPrefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
    }
    
    /// <summary>
    /// Muestra un número flotante en la posición especificada
    /// </summary>
    public void ShowNumber(Vector3 position, string text, Color color)
    {
        GameObject numberObj = GetFromPool();
        if (numberObj == null) return;

        numberObj.transform.position = position;
        numberObj.SetActive(true);

        // Configurar texto
        TextMeshPro tmp = numberObj.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
        }

        // Retornar al pool después de la animación
        StartCoroutine(ReturnToPool(numberObj, 1f));
    }

    /// <summary>
    /// Muestra número de daño
    /// </summary>
    public void ShowDamage(Vector3 position, int damage, bool isCritical = false)
    {
        string text = damage.ToString();
        Color color = isCritical ? Color.yellow : new Color(1f, 0.5f, 0f); // Naranja para normal, amarillo para crítico
        
        if (isCritical)
        {
            text = damage + "!"; // Agregar ! para críticos
        }

        ShowNumber(position, text, color);
    }

    /// <summary>
    /// Muestra coins ganadas
    /// </summary>
    public void ShowCoins(Vector3 position, int coins)
    {
        string text = "+" + coins;
        Color color = new Color(0f, 1f, 0.5f); // Verde brillante
        ShowNumber(position, text, color);
    }

    /// <summary>
    /// Muestra gems ganadas
    /// </summary>
    public void ShowGems(Vector3 position, int gems)
    {
        string text = "+" + gems + " 💎";
        Color color = new Color(0.3f, 0.7f, 1f); // Azul diamante
        ShowNumber(position, text, color);
    }

    GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else if (floatingNumberPrefab != null)
        {
            // Si el pool está vacío, crear nuevo objeto
            GameObject obj = Instantiate(floatingNumberPrefab, transform);
            return obj;
        }
        
        return null;
    }
    
    IEnumerator ReturnToPool(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (obj != null)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}