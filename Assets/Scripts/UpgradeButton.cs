using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public UpgradeType upgradeType;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nextValueText;
    public Button button;

    void Start()
    {
        button.onClick.AddListener(OnUpgradeClicked);
        UpdateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        UpgradeInfo info = UpgradeManager.Instance.GetUpgradeInfo(upgradeType);

        levelText.text = $"Level {info.currentLevel}";
        costText.text = CurrencyManager.FormatNumber(info.cost);
        nextValueText.text = $"{info.currentValue:F1} → {info.nextValue:F1}";

        // Cambiar color según si puede comprar
        button.interactable = info.canAfford;
    }

    void OnUpgradeClicked()
    {
        bool success = false;

        switch (upgradeType)
        {
            case UpgradeType.Damage:
                success = UpgradeManager.Instance.UpgradeDamage();
                break;
            case UpgradeType.FireRate:
                success = UpgradeManager.Instance.UpgradeFireRate();
                break;
            // ... otros casos
        }

        if (success)
        {
            // Efecto de sonido
            // AudioManager.Instance.PlayUpgradeSound();
        }
    }
}