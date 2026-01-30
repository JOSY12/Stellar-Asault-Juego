using UnityEngine;

/// <summary>
/// Script de testing para probar mecánicas sin necesidad de UI completa
/// Úsalo solo para desarrollo, elimínalo en la build final
/// </summary>
public class QuickTester : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebug = true;
    public int startingCoins = 10000;
    public int startingGems = 100;

    void Start()
    {
        if (!enableDebug) return;

        // Dar dinero inicial para testear upgrades
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(startingCoins);
            CurrencyManager.Instance.AddGems(startingGems);
            Debug.Log($"[QuickTester] Added {startingCoins} coins and {startingGems} gems");
        }
    }

    void Update()
    {
        if (!enableDebug) return;

        // Teclas de debug para upgrades
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (UpgradeManager.Instance != null)
            {
                bool success = UpgradeManager.Instance.UpgradeDamage();
                Debug.Log($"[QuickTester] Damage upgrade: {(success ? "SUCCESS" : "FAILED - Not enough coins")}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (UpgradeManager.Instance != null)
            {
                bool success = UpgradeManager.Instance.UpgradeFireRate();
                Debug.Log($"[QuickTester] Fire rate upgrade: {(success ? "SUCCESS" : "FAILED")}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (UpgradeManager.Instance != null)
            {
                bool success = UpgradeManager.Instance.UpgradeBulletCount();
                Debug.Log($"[QuickTester] Bullet count upgrade: {(success ? "SUCCESS" : "FAILED")}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (UpgradeManager.Instance != null)
            {
                bool success = UpgradeManager.Instance.UpgradeMoveSpeed();
                Debug.Log($"[QuickTester] Move speed upgrade: {(success ? "SUCCESS" : "FAILED")}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (UpgradeManager.Instance != null)
            {
                bool success = UpgradeManager.Instance.UpgradeMaxHealth();
                Debug.Log($"[QuickTester] Max health upgrade: {(success ? "SUCCESS" : "FAILED")}");
            }
        }

        // Teclas de debug para monedas
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCoins(1000);
                Debug.Log("[QuickTester] +1000 coins");
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGems(10);
                Debug.Log("[QuickTester] +10 gems");
            }
        }

        // Teclas de debug para vida
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
                Debug.Log("[QuickTester] Player took 1 damage");
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null)
            {
                ph.Heal(1);
                Debug.Log("[QuickTester] Player healed 1 HP");
            }
        }

        // Teclas de debug para oleadas
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.currentWave += 5;
                Debug.Log($"[QuickTester] Skipped to wave {WaveManager.Instance.currentWave}");
            }
        }
    }

    void OnGUI()
    {
        if (!enableDebug) return;

        // Mostrar stats en pantalla
        GUILayout.BeginArea(new Rect(10, 10, 350, 500));
        
        GUILayout.Box("=== DEBUG INFO ===");
        
        if (CurrencyManager.Instance != null)
        {
            GUILayout.Label($"Coins: {CurrencyManager.Instance.coins:N0}");
            GUILayout.Label($"Gems: {CurrencyManager.Instance.gems}");
            GUILayout.Label($"Coin Multiplier: x{CurrencyManager.Instance.coinMultiplier:F2}");
        }

        GUILayout.Space(10);

        if (WaveManager.Instance != null)
        {
            GUILayout.Label($"Current Wave: {WaveManager.Instance.GetCurrentWave()}");
            GUILayout.Label($"Wave Active: {WaveManager.Instance.IsWaveActive()}");
            GUILayout.Label($"Wave Progress: {WaveManager.Instance.GetWaveProgress():P0}");
        }

        GUILayout.Space(10);

        if (UpgradeManager.Instance != null)
        {
            GUILayout.Label($"Damage Level: {UpgradeManager.Instance.damageLevel}");
            GUILayout.Label($"Fire Rate Level: {UpgradeManager.Instance.fireRateLevel}");
            GUILayout.Label($"Bullet Count Level: {UpgradeManager.Instance.bulletCountLevel}");
            GUILayout.Label($"Move Speed Level: {UpgradeManager.Instance.moveSpeedLevel}");
            GUILayout.Label($"Max Health Level: {UpgradeManager.Instance.maxHealthLevel}");
        }

        GUILayout.Space(10);

        PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
        if (ph != null)
        {
            GUILayout.Label($"Health: {ph.currentHealth}/{ph.maxHealth}");
        }

        GUILayout.Space(10);

        GUILayout.Box("=== KEYBOARD CONTROLS ===");
        GUILayout.Label("1 = Upgrade Damage");
        GUILayout.Label("2 = Upgrade Fire Rate");
        GUILayout.Label("3 = Upgrade Bullet Count");
        GUILayout.Label("4 = Upgrade Move Speed");
        GUILayout.Label("5 = Upgrade Max Health");
        GUILayout.Label("M = +1000 Coins");
        GUILayout.Label("N = +10 Gems");
        GUILayout.Label("K = Take Damage");
        GUILayout.Label("H = Heal");
        GUILayout.Label("W = Skip +5 Waves");

        GUILayout.EndArea();
    }
}