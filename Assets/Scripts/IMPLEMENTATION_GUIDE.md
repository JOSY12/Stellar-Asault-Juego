# 🚀 GUÍA DE IMPLEMENTACIÓN PASO A PASO

## 📋 FASE 1: SETUP INICIAL (30 minutos)

### 1. Reemplazar Scripts Existentes

Reemplaza estos archivos en tu proyecto:

- ✅ `Enemy.cs` → `Enemy_Improved.cs`
- ✅ `PlayerController.cs` → `PlayerController_Improved.cs`
- ✅ `Bala.cs` → `Bala_Improved.cs`

### 2. Añadir Nuevos Scripts

Agrega estos scripts nuevos a tu proyecto:

- 📄 `CurrencyManager.cs`
- 📄 `UpgradeManager.cs`
- 📄 `WaveManager.cs`
- 📄 `PlayerHealth.cs`
- 📄 `UIManager.cs`

### 3. Configurar GameObjects en la Escena

#### A) Crear "GameManager" (Empty GameObject)

```
Jerarquía:
- GameManager
  ├── CurrencyManager (script)
  ├── UpgradeManager (script)
  └── WaveManager (script)
```

**Configuración:**

- Agregar componente `CurrencyManager`
- Agregar componente `UpgradeManager`
  - Asignar referencia al `PlayerController`
- Agregar componente `WaveManager`
  - Asignar referencia al `EnemySpawner`

#### B) Configurar Player

```
Player GameObject:
- PlayerController (script actualizado)
- PlayerHealth (nuevo script)
- Rigidbody2D
- Collider2D (con Is Trigger activado)
- Tag: "Player"
```

**Configuración PlayerHealth:**

- Max Health: 3
- Invulnerability Duration: 1
- Asignar Sprite Renderer

**Configuración PlayerController:**

- Mantener todo igual
- Nuevos campos aparecerán automáticamente

#### C) Configurar Enemy Prefab

```
Enemy Prefab:
- Enemy_Improved (script actualizado)
- Collider2D (con Is Trigger activado)
- Tag: "Enemy"
```

**Configuración Enemy:**

- Coin Drop Min: 5
- Coin Drop Max: 15
- Gem Drop Chance: 0.05 (5%)

---

## 📱 FASE 2: CREAR LA UI (1-2 horas)

### Canvas Setup

```
Canvas (Screen Space - Overlay)
└── UI_Container
    ├── HUD
    │   ├── CoinsText (TextMeshPro)
    │   ├── GemsText (TextMeshPro)
    │   ├── WaveText (TextMeshPro)
    │   └── HealthContainer
    │       ├── Heart1 (Image)
    │       ├── Heart2 (Image)
    │       └── Heart3 (Image)
    │
    ├── UpgradePanel (Panel - Desactivado por defecto)
    │   ├── Title: "Upgrades"
    │   ├── UpgradeButton_Damage
    │   ├── UpgradeButton_FireRate
    │   ├── UpgradeButton_BulletCount
    │   ├── UpgradeButton_Speed
    │   ├── CloseButton
    │   └── ScrollView (para más upgrades)
    │
    ├── OfflineEarningsPopup (Panel - Desactivado)
    │   ├── Title: "Welcome Back!"
    │   ├── EarningsText
    │   ├── ClaimButton
    │   └── ClaimX3Button (Watch AD)
    │
    ├── WaveCompletePopup (Panel - Desactivado)
    │   ├── WaveText
    │   └── ClaimBossAdButton
    │
    └── GameOverPanel (Panel - Desactivado)
        ├── GameOverText
        ├── WaveReachedText
        ├── ContinueAdButton
        └── RestartButton
```

### Crear Botón de Upgrade (Template)

Cada botón de upgrade debe tener:

```
UpgradeButton (Button)
├── Icon (Image)
├── NameText (TextMeshPro): "Damage"
├── LevelText (TextMeshPro): "Level 5"
├── CostText (TextMeshPro): "Cost: 1.2K"
└── NextValueText (TextMeshPro): "+8 → +16"
```

### Configurar UIManager

```
UIManager GameObject (en Canvas):
- UIManager (script)
  - Asignar todas las referencias de UI en Inspector
  - Coins Text
  - Gems Text
  - Wave Text
  - Health Hearts (array)
  - Todos los paneles y popups
```

---

## 🎮 FASE 3: IMPLEMENTAR MECÁNICAS CORE (2-3 horas)

### A) Sistema de Upgrades en UI

Crear script `UpgradeButton.cs`:

```csharp
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
```

### B) Conectar PlayerController con UpgradeManager

En `UpgradeManager.cs`, modificar `ApplyBulletCountUpgrade()`:

```csharp
void ApplyBulletCountUpgrade()
{
    int bulletCount = 1;
    if (bulletCountLevel >= 15) bulletCount = 8;
    else if (bulletCountLevel >= 10) bulletCount = 5;
    else if (bulletCountLevel >= 5) bulletCount = 3;
    else if (bulletCountLevel >= 3) bulletCount = 2;

    if (player != null)
    {
        player.SetBulletCount(bulletCount); // ← NUEVO
    }
}
```

### C) Testing Checklist

✅ Al matar enemigos, aparecen coins en el UI
✅ Puedo comprar upgrades cuando tengo suficiente dinero
✅ Los upgrades se aplican correctamente al player
✅ El daño escala exponencialmente
✅ Multi-shot funciona correctamente
✅ Las oleadas aumentan en dificultad
✅ Al morir aparece Game Over con opción de continuar
✅ Offline earnings se muestran al regresar

---

## 💎 FASE 4: INTEGRAR ADS (1-2 horas)

### Configurar Unity Ads o AdMob

#### Opción A: Unity Ads (Recomendado)

1. **Window → Package Manager → Unity Registry**
2. Buscar "Advertisement Legacy" e instalar
3. **Window → General → Services**
4. Crear proyecto Unity
5. Activar "Ads"
6. Obtener Game ID

#### Crear AdManager.cs:

```csharp
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdManager Instance { get; private set; }

    [SerializeField] private string androidGameId = "YOUR_ANDROID_ID";
    [SerializeField] private string iosGameId = "YOUR_IOS_ID";
    [SerializeField] private bool testMode = true;

    private string gameId;
    private string rewardedAdId = "Rewarded_Android"; // o "Rewarded_iOS"

    private System.Action onAdCompletedCallback;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAds()
    {
        #if UNITY_ANDROID
            gameId = androidGameId;
            rewardedAdId = "Rewarded_Android";
        #elif UNITY_IOS
            gameId = iosGameId;
            rewardedAdId = "Rewarded_iOS";
        #endif

        Advertisement.Initialize(gameId, testMode, this);
    }

    public void ShowRewardedAd(System.Action onCompleted)
    {
        onAdCompletedCallback = onCompleted;
        Advertisement.Load(rewardedAdId, this);
    }

    // Implementar interfaces...
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(rewardedAdId, this);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            onAdCompletedCallback?.Invoke();
            onAdCompletedCallback = null;
        }
    }

    // ... resto de métodos de interfaces
}
```

### Integrar AdManager en el código existente

Descomentar las líneas que dicen:

```csharp
// AdManager.Instance.ShowRewardedAd(() => {
//     // código aquí
// });
```

Por ejemplo, en `PlayerHealth.cs`:

```csharp
public void ContinueWithAd()
{
    AdManager.Instance.ShowRewardedAd(() => {
        currentHealth = maxHealth;
        isDead = false;
        Time.timeScale = 1f;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        StartCoroutine(ReviveEffect());
    });
}
```

---

## 🎨 FASE 5: POLISH & JUICE (2-3 horas)

### A) Particle Systems

Crear 3 prefabs de partículas:

1. **EnemyDeathParticles**
   - Particle System
   - Shape: Sphere
   - Start Lifetime: 0.5
   - Start Speed: 5
   - Start Size: 0.1-0.3
   - Color over Lifetime: Fade out
   - Emission: Burst 20

2. **CriticalHitParticles**
   - Similar pero dorado
   - Forma de estrella

3. **LevelUpParticles**
   - Más elaborado
   - Múltiples colores

### B) Sound Effects

Agregar AudioSource al GameManager:

```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip criticalSound;
    public AudioClip upgradeSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
```

### C) Floating Damage Numbers

Crear prefab `FloatingNumber`:

- TextMeshPro
- Animation subiendo y fading
- Auto-destruir después de 1 segundo

Object Pooling para performance:

```csharp
public class FloatingNumberPool : MonoBehaviour
{
    public GameObject floatingNumberPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public void ShowNumber(Vector3 position, string text, Color color)
    {
        GameObject num = pool.Count > 0 ? pool.Dequeue() : Instantiate(floatingNumberPrefab);
        num.transform.position = position;
        num.GetComponent<TextMeshPro>().text = text;
        num.GetComponent<TextMeshPro>().color = color;
        num.SetActive(true);

        StartCoroutine(ReturnToPool(num, 1f));
    }

    IEnumerator ReturnToPool(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

---

## 📊 FASE 6: BALANCING (1-2 horas)

### Testing de Economía

Jugar 30 minutos y trackear:

- ¿Cuánto tiempo toma el primer upgrade?
- ¿Cuánto tiempo entre cada upgrade después?
- ¿Se siente la progresión?

**Meta ideal:**

- Primer upgrade: 30-60 segundos
- Upgrades tempranos (1-5): 1-3 minutos entre cada uno
- Upgrades medios (5-15): 5-10 minutos
- Upgrades tardíos (15+): 15-30 minutos

### Ajustar si es necesario:

**Si progresión muy lenta:**

- Aumentar coin drops de enemigos
- Reducir costos de upgrades
- Aumentar coin multiplier base

**Si progresión muy rápida:**

- Aumentar cost multiplier (de 1.5 a 1.7)
- Reducir coin drops
- Hacer upgrades menos potentes

---

## 🚀 FASE 7: BUILD & DEPLOY

### A) Preparar para Android

**Player Settings:**

- Company Name
- Product Name
- Package Name (com.tucompañia.tujuego)
- Icon
- Splash Screen

**Build Settings:**

- Platform: Android
- Minimum API Level: 22 (Android 5.1)
- Target API Level: Latest
- Scripting Backend: IL2CPP
- Target Architectures: ARM64

### B) Optimizaciones

En `Quality Settings`:

- VSync Count: Don't Sync
- Shadow Quality: Disable
- Anti-Aliasing: Disabled

En código, usar object pooling para:

- Bullets
- Enemies
- Particle effects
- Floating numbers

### C) Testing en Dispositivo

Checklist:

- ✅ Performance 60 FPS constante
- ✅ Touch controls responsivos
- ✅ Ads funcionan correctamente
- ✅ Save/Load funciona
- ✅ No crashes después de 30 min

---

## 📈 FASE 8: MONETIZACIÓN POST-LAUNCH

### A) Analytics Setup

Integrar Unity Analytics:

```csharp
using UnityEngine.Analytics;

// Track eventos importantes
Analytics.CustomEvent("level_up", new Dictionary<string, object>
{
    { "upgrade_type", "damage" },
    { "level", 5 }
});

Analytics.CustomEvent("ad_watched", new Dictionary<string, object>
{
    { "ad_type", "continue" },
    { "wave_reached", 15 }
});
```

### B) A/B Testing

Probar diferentes valores:

- Costo de upgrades
- Frecuencia de boss waves
- Rewards de ads (x2 vs x3)
- Dificultad de oleadas

### C) KPIs a Monitorear

**Día 1-7:**

- Retention D1: >40%
- Avg Session Length: >8 min
- Ads per Session: >2

**Después de 1 mes:**

- Retention D7: >20%
- ARPDAU: $0.05-0.15
- Ads per DAU: >5

---

## 🎯 CHECKLIST FINAL

### Pre-Launch

- [ ] Todos los scripts sin errores
- [ ] UI completa y funcional
- [ ] Ads integradas y funcionando
- [ ] Save/Load testeado
- [ ] Tutorial/Primera experiencia clara
- [ ] Optimizado para móvil (60 FPS)
- [ ] Audio implementado
- [ ] Particle effects agregados
- [ ] Testeado en múltiples dispositivos

### Launch

- [ ] Publicado en Google Play
- [ ] Screenshots atractivos
- [ ] Descripción optimizada ASO
- [ ] Video trailer (opcional)
- [ ] Privacy Policy (requerido para ads)

### Post-Launch

- [ ] Analytics configurados
- [ ] Monitoring de crashes
- [ ] Plan de updates mensual
- [ ] Community management (reviews)

---

## 💡 TIPS FINALES

### Performance

- Usar Object Pooling para todo lo que se instancia frecuentemente
- Limitar particle systems a 3-4 activos simultáneamente
- Desactivar shadows en móvil
- Usar Sprite Atlas para UI

### Monetización

- Nunca forzar ads - siempre opcionales
- Rewards deben sentirse "worth it" (mínimo x2)
- Testing constante de balance económico
- Observar donde los jugadores abandonan

### Engagement

- Daily rewards desde día 1
- Push notifications para cofres/eventos
- Actualizar contenido cada 2-4 semanas
- Escuchar feedback de jugadores

### Next Features (Fase 4)

1. Sistema de Logros (con gem rewards)
2. Lucky Spin (ruleta diaria)
3. Daily Missions
4. Prestige/Rebirth System
5. Pet System
6. Weapon Skins
7. Leaderboards
8. Events especiales

---

## 🆘 TROUBLESHOOTING COMÚN

### "Los upgrades no se aplican"

- Verificar que UpgradeManager tenga referencia a Player
- Revisar que ApplyAllUpgrades() se llame en Start()

### "No aparecen coins al matar enemigos"

- Verificar que Enemy tenga CurrencyManager.Instance activo
- Revisar que OnTriggerEnter2D esté detectando colisión

### "Game Over no muestra ads"

- Verificar que AdManager esté inicializado
- Revisar logs de Unity Ads
- Confirmar que Test Mode esté activado

### "FPS bajos en móvil"

- Implementar Object Pooling
- Reducir particle systems
- Desactivar post-processing
- Usar Sprite Atlas

---

¡Éxito con tu juego! 🚀
