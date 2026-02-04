using UnityEngine;

public class PaletteDebug : MonoBehaviour
{
    void Update()
    {
        // Cambiar paleta con teclas numéricas
        if (Input.GetKeyDown(KeyCode.Alpha0)) ChangePalette(0); // DEFAULT
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangePalette(1); // NEON
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangePalette(2); // RETRO
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangePalette(3); // BLOOD
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangePalette(4); // OCEAN
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangePalette(5); // CYBERPUNK
        if (Input.GetKeyDown(KeyCode.Alpha6)) ChangePalette(6); // MONOCHROME
    }
    
    void ChangePalette(int index)
    {
        if (PaletteManager.Instance != null)
        {
            PaletteManager.Instance.SetPalette(index);
            
            string paletteName = "Unknown";
            if (index < PaletteManager.Instance.allPalettes.Length)
            {
                paletteName = PaletteManager.Instance.allPalettes[index].paletteName;
            }
            
            Debug.Log($"Palette changed to: {paletteName}");
        }
    }
}