using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("Settings")]
    public GameObject heartPrefab; // Prefab de un corazón
    public Transform heartsContainer; // Parent donde van los corazones
    public Sprite fullHeartSprite; // Corazón lleno
    public Sprite emptyHeartSprite; // Corazón vacío
    
    private List<Image> hearts = new List<Image>();
    private int maxHealth = 3;
    
    public void Initialize(int maxHP)
    {
        maxHealth = maxHP;
        
        // Limpiar corazones existentes
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }
        hearts.Clear();
        
        // Crear corazones
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = heart.GetComponent<Image>();
            heartImage.sprite = fullHeartSprite;
            hearts.Add(heartImage);
        }
    }
    
    public void UpdateHealth(int currentHP)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHP)
            {
                hearts[i].sprite = fullHeartSprite;
                hearts[i].color = Color.white;
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite;
                hearts[i].color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparente
            }
        }
    }
}